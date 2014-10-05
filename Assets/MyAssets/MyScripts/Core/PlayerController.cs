﻿//INPUT MODE
#define KEYBOARD 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
#if OUYA
,OuyaSDK.IJoystickCalibrationListener, 
OuyaSDK.IPauseListener, 
OuyaSDK.IResumeListener, 
OuyaSDK.IMenuButtonUpListener,
OuyaSDK.IMenuAppearingListener
#endif
{		
		//Delegates
		public delegate void AchievementReceived (Achievement achievement,int playerIndex,int newScore);
		public static AchievementReceived AchievementReceivedListeners; //check if null
		
		public delegate void PlayerDied (int layer);
		public static PlayerDied PlayerDiedListeners; //check if null
		
		//Type
		public bool isNPC = false;
		
		//Control variables
		public bool grabHeld;
		public bool respawnWithPlayer;
		public bool positionLocked;
		public bool jumpDisabled;
		public bool jointDisabled;
		public bool pickUpDisabled;
		public bool slingShotDisabled;
		public bool jointLengthChangeDisabled;
		
		//Debounce variables
		// public int jumpDebounce;
		private float debounceY = 0.85f;
		private bool jumpReleased;
		private bool prevGrabPressed;
	
		//Grab indicators
		private GameObject grabIndicator;
		private GameObject grabRadiusIndicator;

		//Spawn parameters
		public  Color spawnColor;
		public  GameObject spawnLocation;
	
		//Controller info
		public int playerIndex = 1;
		public OuyaSDK.OuyaPlayer controllerIndex = OuyaSDK.OuyaPlayer.player1;
	
#if KEYBOARD
		private bool oddController;
#elif OUYA
		public bool leftSplit = false;
		public bool splitController = false;
		private bool m_useSDKForInput = false;
#endif

		//Jump status variables
		//public bool jump = false;				// Condition for whether the player should jump.
		public bool jumping = false;  
		
		//Movement variables
		public float maxSpeed = 5.0f;				// The fastest the player can travel in the x axis.
		public float jumpForce = 18.0f;
		public float jumpMaxOut = 7.0f;
		private float slingShotForce = 130.0f; //130.0f
		private float npcSlingShotForce = 130.0f;
		private float throwForceX = 100.0f;
		private float throwForceY = 50.0f;
		public float jumpVelocity = 7.5f;			// Amount of force added when the player jumps.

		//Grounding variables
		private Transform[] groundChecks = new Transform[3];			// A position marking where to check if the player is grounded.
		public  bool grounded = false;			// Whether or not the player is grounded.
		
		//Achievement variables
		public int score;
		public  List<Achievement> achievements;
	
		//Joint variables
		private float holdingDistance = 4.0f;
		private float pickUpDistance = 1.15f;
		private float jointChangeAmount = 0.1f;
		private GameObject slingShottedByPlayer;
		private GameObject pastSlingShottedByPlayer;
		private bool  slingShotJoint;
		private float slingShotDistance = 0.5f;
		private  int lastSlingShotId;
		private  int lastThrowId;
		private DistanceJoint2D joint;
		private PlayerController connectedPlayerController;
		public  bool pickedUp;
		
		//Grounding variables
		public GameObject possibleGroundedByPlayer;
		public GameObject groundedByPlayer;
		
		//private bool facingRight = true;			// For determining which way the player is currently facing.
		
		private int layerMask;
		private int groundMask;
	
		void Awake ()
		{
#if OUYA
				OuyaSDK.registerJoystickCalibrationListener (this);
				OuyaSDK.registerMenuButtonUpListener (this);
				OuyaSDK.registerMenuAppearingListener (this);
				OuyaSDK.registerPauseListener (this);
				OuyaSDK.registerResumeListener (this);
#endif
				
				grabIndicator = transform.FindChild ("grabIndicator").gameObject;
				grabRadiusIndicator = transform.FindChild ("grabRadiusIndicator").gameObject;
				
				groundChecks [0] = transform.FindChild ("groundChecka");
				groundChecks [1] = transform.FindChild ("groundCheckb");
				groundChecks [2] = transform.FindChild ("groundCheckc");
				
				achievements = new List<Achievement> ();
		}
	
		void Start ()
		{
				Input.ResetInputAxes ();
#if KEYBOARD
				oddController = controllerIndex == OuyaSDK.OuyaPlayer.player1 || controllerIndex == OuyaSDK.OuyaPlayer.player3;
#endif
		
				Color grabRadiusIndicatorColor = gameObject.renderer.material.color;
				grabRadiusIndicatorColor.a = 0.5f;
				grabRadiusIndicator.renderer.material.color = grabRadiusIndicatorColor; 
				
				if (isNPC) {
						spawnLocation = new GameObject ();
						spawnLocation.name = "AutogeneratedNPCSpawnLocation";
						spawnLocation.transform.position = gameObject.transform.position;
				} else
						Debug.Log ("Player Index: " + playerIndex + " has instance id: " + gameObject.GetInstanceID ());
				
				int other;
				if (playerIndex % 2 == 0) {
						other = (playerIndex - 1);
						layerMask |= 1 << LayerMask.NameToLayer ("Interact" + other.ToString () + playerIndex.ToString ());
						layerMask |= 1 << LayerMask.NameToLayer ("Versus" + other.ToString () + playerIndex.ToString ());
				} else {
						other = (playerIndex + 1);
						layerMask |= 1 << LayerMask.NameToLayer ("Interact" + playerIndex.ToString () + other.ToString ());
						layerMask |= 1 << LayerMask.NameToLayer ("Versus" + playerIndex.ToString () + other.ToString ());
				}

				layerMask |= 1 << LayerMask.NameToLayer ("Player" + other.ToString ());
				layerMask |= 1 << LayerMask.NameToLayer ("Player" + playerIndex.ToString ());
				layerMask |= 1 << LayerMask.NameToLayer ("Interact" + playerIndex.ToString ());
				groundMask |= 1 << LayerMask.NameToLayer ("Default");
				groundMask |= layerMask;
				
				PlayerDiedListeners += PlayerDiedEvent;
		}
	
		void OnDestroy ()
		{
#if OUYA
				OuyaSDK.unregisterJoystickCalibrationListener (this);
				OuyaSDK.unregisterMenuButtonUpListener (this);
				OuyaSDK.unregisterMenuAppearingListener (this);
				OuyaSDK.unregisterPauseListener (this);
				OuyaSDK.unregisterResumeListener (this);
#endif
		}
		
		void PlayerDiedEvent (int layerMask)
		{
				if (respawnWithPlayer && (layerMask & 1 << gameObject.layer) != 0 && gameObject.activeInHierarchy)
						KillPlayer ();
		}
		
		void Update ()
		{		
				UpdateGrounded ();
				pickedUp &= (joint != null && joint.distance < pickUpDistance) || (connectedPlayerController != null && connectedPlayerController.joint.distance < pickUpDistance);
				bool connected = Connected ();
				bool grabPressed = grabHeld;
				
				if (!isNPC && !GameLogic.Instance.paused) {
						if (!jointLengthChangeDisabled)
								AdjustJointLength ();
#if KEYBOARD
						CheckForKeyboardPause ();
#endif				
						bool jumpPressed = JumpPressed () && !jumpDisabled;
						jumpReleased |= !jumpPressed;
						jumping &= !jumpReleased;
						if (!jumping)
								slingShottedByPlayer = null;
		
						if (jumpPressed && grounded && !pickedUp && jumpReleased && !jumping) {
								jumping = true;
								jumpReleased = false;
								possibleGroundedByPlayer = groundedByPlayer;
						}
		
						grabPressed |= GrabPressed () && (!jointDisabled || !slingShotDisabled);

						if (!connected && (grabPressed || jumpPressed)) {
								Collider2D[] hitColliders = Physics2D.OverlapCircleAll (gameObject.transform.position, holdingDistance, layerMask);
								foreach (Collider2D collider in hitColliders) {
										if (collider.gameObject != gameObject && (collider.gameObject.CompareTag ("Player") || collider.gameObject.CompareTag ("NPC"))) {		
												PlayerController playerController = collider.gameObject.GetComponent<PlayerController> ();
					
												if (playerController && playerController.AcceptConnectionOrslingShot ()) {
														if (jumpPressed && lastSlingShotId != collider.gameObject.GetInstanceID () && !jumping && jumpReleased && !slingShotDisabled && !playerController.slingShotDisabled) {
																lastSlingShotId = collider.gameObject.GetInstanceID ();
						
																jumping = true;
																jumpReleased = false;
																slingShottedByPlayer = collider.gameObject;
														
																StopMovement (gameObject);
																StopMovement (slingShottedByPlayer);
																break;
														} else if (grabPressed && !jointDisabled) {
																MakeConnection (collider.gameObject, playerController, true);
																break;
														}
												}
										}
								}
						} else if (connected && !grabPressed && (!slingShotJoint || ReleaseSlingShot ())) {
								ThrowPlayer ();
								DestroyConnection ();
						}
				}
				
				grabIndicator.SetActive (connected);
				grabRadiusIndicator.SetActive (grabPressed && !connected);
				grabRadiusIndicator.transform.localScale = new Vector3 (2 * holdingDistance + gameObject.transform.localScale.x / 2.0f, 0.1f, 2 * holdingDistance + gameObject.transform.localScale.y / 2.0f);
		}
		
		bool ReleaseSlingShot ()
		{
				return true;		
				//return gameObject.transform.position.y > pastSlingShottedByPlayer.transform.position.y && Mathf.Abs (gameObject.transform.position.x - pastSlingShottedByPlayer.transform.position.x) < slingShotDistance;
		}
		
		void FixedUpdate ()
		{
				Vector2 velocity = rigidbody2D.velocity;
				if (!isNPC) {
						float h = GetHorizontalMovement ();

						if (grounded)
								velocity.x = maxSpeed * h;
						else if (!Connected () && slingShottedByPlayer == null) {
								if (Mathf.Abs (velocity.x) <= maxSpeed)
										velocity.x = maxSpeed * h;
								else if (h * velocity.x < 0)
										velocity.x = velocity.x + h * maxSpeed;
						} else if (Mathf.Abs (velocity.x) <= maxSpeed || h * velocity.x < 0)
								rigidbody2D.AddForce (new Vector2 (h * 1.0f, 0.0f));
						
		
						if (jumping) {
								if (slingShottedByPlayer == null) {
										if (gameObject.rigidbody2D.velocity.y < jumpMaxOut) {
												gameObject.rigidbody2D.AddForce (Vector2.up * jumpForce);// (10 + 6 * gameObject.rigidbody2D.velocity.y));
												if (groundedByPlayer != null)
														groundedByPlayer.transform.rigidbody2D.AddForce (Vector2.up * -jumpForce);
										} else
												jumping = false;
										/*velocity.y = jumpVelocity;
										if (groundedByPlayer != null)
												groundedByPlayer.transform.rigidbody2D.AddForce (new Vector2 (0f, backJumpForce)); */
								} else {								
										/*if (gameObject.transform.position.y < slingShottedByPlayer.transform.position.y + gameObject.transform.localScale.y)
												gameObject.rigidbody2D.AddForce (PerpendicularUpVector ((slingShottedByPlayer.transform.position - gameObject.transform.position).normalized * 5));
										else {
												jumping = false;
												slingShottedByPlayer = null; 
										} */
										slingShot ();
										pastSlingShottedByPlayer = slingShottedByPlayer;
										jumping = false;
										slingShottedByPlayer = null; 
								}
			
						}		
				} else if (grounded)
						velocity.x = 0.0f;
					
				rigidbody2D.velocity = velocity;
		}
		
		Vector2 PerpendicularUpVector (Vector2 vector)
		{
				Vector2 flop = new Vector2 (vector.y, vector.x);
				if (flop.y < 0)
						return new Vector2 (flop.x, flop.y * -1);
				else if (flop.y > 0)
						return new Vector2 (flop.x * -1, flop.y);
				
				return new Vector2 (gameObject.rigidbody2D.velocity.x, 0);
		}
		
		void MakeConnection (GameObject otherPlayer, PlayerController otherController, bool pickUp)
		{
				grabIndicator.SetActive (true);
				grabIndicator.renderer.material.color = otherPlayer.renderer.material.color;
				otherController.grabIndicator.SetActive (true);
				otherController.grabIndicator.renderer.material.color = gameObject.renderer.material.color;
		
				joint = gameObject.AddComponent ("DistanceJoint2D") as DistanceJoint2D;
				joint.collideConnected = true;
				joint.connectedBody = otherPlayer.rigidbody2D;
				joint.distance = Mathf.Min (Vector2.Distance (gameObject.transform.position, otherPlayer.transform.position), holdingDistance);
		
				otherController.connectedPlayerController = this;
		
				if (pickUp)
						PickUpPlayer (joint, otherPlayer.transform);
		}
		
		void slingShot ()
		{
				if (slingShottedByPlayer.GetComponent<PlayerController> ().isNPC) {
						slingShotJoint = true;
						//MakeConnection (slingShottedByPlayer, slingShottedByPlayer.GetComponent<PlayerController> (), false); //Temp Connection to make the player rotate around
						//float direction = gameObject.transform.position.x - slingShottedByPlayer.transform.position.x;
						//	Vector2 forceVector = new Vector2 (0.0f, 0.5f);
						//	if (direction > 0)
						//			forceVector.x = 0.5f;
						//	else if (direction < 0)
						//			forceVector.x = -0.5f;
						Vector2 forceVector = new Vector2 (0.0f, 1.0f);
						forceVector *= npcSlingShotForce;
						rigidbody2D.AddForce (forceVector);
				} else {
						float direction = gameObject.transform.position.x - slingShottedByPlayer.transform.position.x;
						Vector2 endPosition = slingShottedByPlayer.transform.position;
						if (direction > 0)
								endPosition.x += slingShottedByPlayer.transform.localScale.x;// * 1.5f;
						else if (direction < 0)
								endPosition.x -= slingShottedByPlayer.transform.localScale.x;// * 1.5f;
			
						endPosition.y += slingShottedByPlayer.transform.localScale.y * 2.0f;
			
						Vector2 forceVector = new Vector2 (0.0f, 1.0f);//new Vector2 ((endPosition.x - gameObject.transform.position.x), (endPosition.y - gameObject.transform.position.y)).normalized;
						forceVector *= slingShotForce;
						rigidbody2D.AddForce (forceVector);
						slingShottedByPlayer.rigidbody2D.AddForce (-forceVector);
				}
		}
		
		void StopMovement (GameObject stopObject)
		{
				stopObject.rigidbody2D.velocity = Vector2.zero;
				stopObject.rigidbody2D.angularVelocity = 0.0f;
		}
	
		//Adjust joint length
		void AdjustJointLength ()
		{
				if (Connected ()) {
						DistanceJoint2D jointToUse;
						Transform otherTransform;
						bool otherPlayerIsNPC = false;
						bool otherPlayerLocked = false;
						if (joint != null) {
								jointToUse = joint;
								otherTransform = joint.connectedBody.gameObject.transform;
						} else {
								jointToUse = connectedPlayerController.joint;
								otherTransform = connectedPlayerController.gameObject.transform;
						}
						
						PlayerController otherController = otherTransform.gameObject.GetComponent<PlayerController> ();
						
						otherPlayerIsNPC = otherController.isNPC;
						otherPlayerLocked = otherController.positionLocked;
						bool increase = IncreaseJointLength ();
						bool decrease = DecreaseJointLength ();
						
						if (increase || decrease) {
						
								float halfJointDistance = jointChangeAmount / 2.0f;
								Vector3 originalVectorToOther = otherTransform.position - gameObject.transform.position;
								Vector3 vectorToOtherPlayer = originalVectorToOther.normalized;
								vectorToOtherPlayer *= halfJointDistance;
				
								if (increase) {
										if (jointToUse.distance < holdingDistance - halfJointDistance) {
												if (!CollisionIfGameObjectMovedAlongVector (gameObject, -vectorToOtherPlayer)) {
														jointToUse.distance += halfJointDistance;
														if (Vector3.SqrMagnitude (originalVectorToOther) < jointToUse.distance * jointToUse.distance)
																gameObject.transform.position -= vectorToOtherPlayer;
												}
																								
												if (!otherPlayerLocked && !CollisionIfGameObjectMovedAlongVector (otherTransform.gameObject, vectorToOtherPlayer)) {
														jointToUse.distance += halfJointDistance;
														if (Vector3.SqrMagnitude (originalVectorToOther) < jointToUse.distance * jointToUse.distance)
																otherTransform.position += vectorToOtherPlayer;
												}
				
												if (jointToUse.distance > holdingDistance)
														jointToUse.distance = holdingDistance;
										}
								}
		
								if (decrease) {
										if (jointToUse.distance > pickUpDistance) {
												if (!CollisionIfGameObjectMovedAlongVector (gameObject, vectorToOtherPlayer)) {
														jointToUse.distance -= halfJointDistance;
														if (Vector3.SqrMagnitude (originalVectorToOther) > jointToUse.distance * jointToUse.distance)
																gameObject.transform.position += vectorToOtherPlayer;
												}
					
												if (!otherPlayerLocked && !CollisionIfGameObjectMovedAlongVector (otherTransform.gameObject, -vectorToOtherPlayer)) {
														jointToUse.distance -= halfJointDistance;
														if (Vector3.SqrMagnitude (originalVectorToOther) > jointToUse.distance * jointToUse.distance)
																otherTransform.position -= vectorToOtherPlayer;
												}
												
												PickUpPlayer (jointToUse, otherTransform);
										}
								}
						}
				}	
		}
	
		void PickUpPlayer (DistanceJoint2D joint, Transform otherPlayer)
		{
				PlayerController otherPlayerController = otherPlayer.GetComponent<PlayerController> ();
				if (joint.distance <= pickUpDistance && !pickUpDisabled && otherPlayerController.lastThrowId != gameObject.GetInstanceID () && !otherPlayerController.pickUpDisabled) {
						otherPlayer.position = gameObject.transform.position + new Vector3 (0.0f, gameObject.transform.localScale.y, 0.0f);
						joint.distance = gameObject.transform.localScale.y;
						otherPlayerController.pickedUp = true;
				}
		}
	
		void ThrowPlayer ()
		{


				bool throwPlayer = false;
				GameObject otherPlayer = null;
				PlayerController otherPlayerController = null;
				if (connectedPlayerController != null && connectedPlayerController.pickedUp) {
						throwPlayer = true;
						otherPlayer = connectedPlayerController.gameObject;
						otherPlayerController = connectedPlayerController;
				} else if (joint != null) {
						otherPlayerController = joint.connectedBody.gameObject.GetComponent<PlayerController> ();
						if (otherPlayerController.pickedUp) {
								throwPlayer = true;
								otherPlayer = joint.connectedBody.gameObject;
						}
				}


				//	if (isNPC || otherPlayerController.isNPC)
				//				return;

				if (throwPlayer) {
						float fractionX = Mathf.Abs (gameObject.rigidbody2D.velocity.x / maxSpeed);
						float direction = gameObject.rigidbody2D.velocity.x < 0 ? -1 : 1;
						if (fractionX == 0)
								direction = 0;
						Vector2 forceVector = new Vector2 ((1.0f + fractionX) * throwForceX * direction, (2.0f - fractionX) * throwForceY);
						Debug.Log ("F");
						otherPlayer.rigidbody2D.AddForce (forceVector);
						otherPlayerController.lastThrowId = gameObject.GetInstanceID ();
				}
		}
		
		//Collision check
		bool CollisionIfGameObjectMovedAlongVector (GameObject movingObject, Vector3 vector)
		{
				Collider2D[] colliders = Physics2D.OverlapAreaAll (BottomLeft (movingObject) + vector, 
		                                                   TopRight (movingObject) + vector, groundMask);
		
				foreach (Collider2D aCollider in colliders) {
						if (!aCollider.isTrigger && aCollider.gameObject != movingObject)
								return true;
				}
		
				return false;
		}
	
		//Box corners
		Vector3 TopRight (GameObject box)
		{
				return box.transform.position + new Vector3 (box.transform.localScale.x / 2.0f, box.transform.localScale.y / 2.0f, 0);
		}
	
		Vector3 BottomLeft (GameObject box)
		{
				return box.transform.position - new Vector3 (box.transform.localScale.x / 2.0f, box.transform.localScale.y / 2.05f, 0);
		}
	
		GameObject OtherPlayer ()
		{
				if (joint != null) 
						return joint.connectedBody.gameObject;

				return connectedPlayerController.gameObject;
		}
	
		void OnTriggerEnter2D (Collider2D other)
		{
				if (other.gameObject.CompareTag ("Respawn") && other.gameObject.GetInstanceID () != spawnLocation.GetInstanceID ()) {
						SpawnScript oldSpawnScript = spawnLocation.GetComponent<SpawnScript> ();
						if (oldSpawnScript != null)
								oldSpawnScript.removeColor (spawnColor);
						SpawnScript newSpawnScript = other.gameObject.GetComponent<SpawnScript> ();
						if (newSpawnScript != null)
								newSpawnScript.addColor (spawnColor);
						spawnLocation = other.gameObject;
				} else if (other.gameObject.CompareTag ("Deadly")) {
						PlayerDiedListeners (layerMask);
						KillPlayer ();
				} else if (other.gameObject.CompareTag ("Coin")) {
						Achievement newAchievement = other.gameObject.GetComponent<CoinScript> ().getAchievement ();
						achievements.Add (newAchievement);
						score += newAchievement.points;
						if (AchievementReceivedListeners != null)
								AchievementReceivedListeners (newAchievement, playerIndex, score);
						Destroy (other.gameObject);
				}
		}
		
		void KillPlayer ()
		{
				DestroyConnection ();
				StopMovement (gameObject);
				StartCoroutine ("RespawnPlayer");
		}

		IEnumerator RespawnPlayer ()
		{
				gameObject.transform.position = new Vector3 (spawnLocation.transform.position.x, spawnLocation.transform.position.y, -20); //move behind camera
				
				yield return new WaitForSeconds (0.5f);
				
				gameObject.transform.position = spawnLocation.transform.position;
		}
		
		void OnTriggerStay2D (Collider2D other)
		{
				//	if (other.gameObject.CompareTag ("Points"))
				//			score += Time.deltaTime;
		}
		
		bool AcceptConnectionOrslingShot ()
		{
				return (!Connected () && (GrabPressed () || grabHeld));
		}
	
		public bool Connected ()
		{
				return (joint != null || connectedPlayerController != null);
		}
		
		void DestroyConnection ()
		{
				if (connectedPlayerController != null) {
						connectedPlayerController.DestroyConnection ();
						connectedPlayerController = null;
				} else if (joint != null) {
						joint.connectedBody.gameObject.GetComponent<PlayerController> ().connectedPlayerController = null;
						Destroy (joint);
						joint = null;
				} 
				slingShotJoint = false;
		}

#if OUYA	
		public void OuyaMenuButtonUp ()
		{
				GameLogic.Instance.PausePressed (playerIndex);
		}
	
		public void OuyaMenuAppearing ()
		{
		}
	
		public void OuyaOnPause ()
		{	
		}
	
		public void OuyaOnResume ()
		{
		}
	
		public void OuyaOnJoystickCalibration ()
		{
		}
#elif KEYBOARD
		void CheckForKeyboardPause ()
		{
				if (Input.GetKeyUp (KeyCode.P) && controllerIndex == OuyaSDK.OuyaPlayer.player1) {
						if (Time.timeScale != 0.0f)
								Time.timeScale = 0.0f;
						else
								Time.timeScale = 1.0f;
				}
		}
#endif
		bool JumpPressed ()
		{
#if OUYA
			if (!splitController) {
				return GetButton (OuyaSDK.KeyEnum.BUTTON_O, controllerIndex) || 
						GetButton (OuyaSDK.KeyEnum.BUTTON_RB, controllerIndex) ||
						GetButton (OuyaSDK.KeyEnum.BUTTON_DPAD_UP, controllerIndex);
			} else {
				if (leftSplit) {
						return GetButton (OuyaSDK.KeyEnum.BUTTON_LB, controllerIndex) ||
								GetButton (OuyaSDK.KeyEnum.BUTTON_DPAD_UP, controllerIndex);
				} else {
						return GetButton (OuyaSDK.KeyEnum.BUTTON_O, controllerIndex) || 
								GetButton (OuyaSDK.KeyEnum.BUTTON_RB, controllerIndex);
				}
			}
#elif KEYBOARD
				if (oddController)   
						return Input.GetKey (KeyCode.W);
				else 
						return Input.GetKey (KeyCode.I);
#endif
		}
		
		bool IncreaseJointLength ()
		{
				bool increase = false;
		
#if OUYA
				if (leftSplit)
						increase = (-1 * GetAxis (OuyaSDK.KeyEnum.AXIS_LSTICK_Y, controllerIndex)) > debounceY;
				else
						increase = (-1 * GetAxis (OuyaSDK.KeyEnum.AXIS_RSTICK_Y, controllerIndex)) > debounceY;
#elif KEYBOARD			
				if (oddController)   
						increase = Input.GetKey (KeyCode.E);
				else  
						increase = Input.GetKey (KeyCode.O);
#endif

				return increase;
		}
	
		bool DecreaseJointLength ()
		{
				bool decrease = false;

#if OUYA
				if (leftSplit)
						decrease = (-1 * GetAxis (OuyaSDK.KeyEnum.AXIS_LSTICK_Y, controllerIndex)) < -debounceY;
				else
						decrease = (-1 * GetAxis (OuyaSDK.KeyEnum.AXIS_RSTICK_Y, controllerIndex)) < -debounceY;
#elif KEYBOARD
				if (oddController)   
						decrease = Input.GetKey (KeyCode.Q);
				else 
						decrease = Input.GetKey (KeyCode.U);
#endif

				return decrease;
		}
		
		bool GrabPressed ()
		{
				bool pressed = false;
		
#if OUYA
				if (leftSplit)
					pressed = GetButton (OuyaSDK.KeyEnum.BUTTON_LT, controllerIndex);
				else
					pressed = GetButton (OuyaSDK.KeyEnum.BUTTON_RT, controllerIndex);
#elif KEYBOARD
				if (oddController)   
						pressed = Input.GetKey (KeyCode.LeftShift);
				else 
						pressed = Input.GetKey (KeyCode.Space);
#endif
				//	if (prevGrabPressed != pressed) {
				//			prevGrabPressed = pressed;
				//			return !prevGrabPressed;
				//	}
				
				//	prevGrabPressed = pressed;
				return pressed;
		}
		
		float GetHorizontalMovement ()
		{
				float horizontal = 0.0f;

#if OUYA
				if (!splitController || leftSplit)
					horizontal = GetAxis (OuyaSDK.KeyEnum.AXIS_LSTICK_X, controllerIndex);
				else
					horizontal = GetAxis (OuyaSDK.KeyEnum.AXIS_RSTICK_X, controllerIndex);
					
				if (horizontal == 0.0f && (!splitController || leftSplit)) {
					if (GetButton (OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT, controllerIndex))
						return - 1.0f;
					else if (GetButton (OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT, controllerIndex))
						return 1.0f;
				}
#elif KEYBOARD
				if (Input.GetKey (KeyCode.A) && oddController)
						horizontal = -1.0f;
				else if (Input.GetKey (KeyCode.D) && oddController)
						horizontal = 1.0f;
				else if (Input.GetKey (KeyCode.J) && !oddController)
						horizontal = -1.0f;
				else if (Input.GetKey (KeyCode.L) && !oddController)
						horizontal = 1.0f;	
#endif
				return horizontal;
		}
		
		void UpdateGrounded ()
		{
				grounded = PlayerIsGrounded (gameObject.GetInstanceID (), true);
		}
	
		bool PlayerIsGrounded (int originalId, bool originalCall)
		{
				bool playerGrounded = false;
				Vector3 startRaycast = transform.position;
				startRaycast.y -= transform.localScale.y / 1.99f;
		
				foreach (Transform groundCheck in groundChecks) {
						RaycastHit2D[] hits = Physics2D.LinecastAll (startRaycast, groundCheck.position, groundMask);
						foreach (RaycastHit2D raycastInfo in hits) {
								if (!(raycastInfo.collider.isTrigger || raycastInfo.collider.gameObject == gameObject)) {
										int colliderInstanceId = raycastInfo.collider.gameObject.GetInstanceID ();
										playerGrounded = true;		
										possibleGroundedByPlayer = null;
										
										if (!raycastInfo.collider.CompareTag ("Player") && !raycastInfo.collider.CompareTag ("NPC")) {
												lastThrowId = 0;
												lastSlingShotId = 0;	
										}
										
										if (groundedByPlayer != null && colliderInstanceId != groundedByPlayer.GetInstanceID ())
												groundedByPlayer = null;
										
										if ((raycastInfo.collider.CompareTag ("Player") || raycastInfo.collider.CompareTag ("NPC")) && (originalCall || gameObject.GetInstanceID () != originalId) && 
												raycastInfo.collider.gameObject.GetComponent<PlayerController> ().PlayerIsGrounded (originalId, false)) {
												lastThrowId = 0;
												lastSlingShotId = 0;
												possibleGroundedByPlayer = raycastInfo.collider.gameObject;
										}
	
								}
						}
				}
				
				return playerGrounded;
		}
#if OUYA		
		private float GetAxis (OuyaSDK.KeyEnum keyCode, OuyaSDK.OuyaPlayer player)
		{
				// Check if we want the *new* SDK input or the example common
				if (m_useSDKForInput) {
						// Get the Unity Axis Name for the Unity API
						string axisName = OuyaSDK.GetUnityAxisName (keyCode, player);
			
						// Check if the axis name is valid
						if (!string.IsNullOrEmpty (axisName)) {
								//use the Unity API to get the axis value, raw or otherwise
								float axisValue = Input.GetAxisRaw (axisName);
								//check if the axis should be inverted
								if (OuyaSDK.GetAxisInverted (keyCode, player))
										return -axisValue;
								
								return axisValue;
								
						}
				}
				// moving the common code into the sdk via above
				return OuyaExampleCommon.GetAxis (keyCode, player);
		}
	
		private bool GetButton (OuyaSDK.KeyEnum keyCode, OuyaSDK.OuyaPlayer player)
		{
				// Check if we want the *new* SDK input or the example common
				if (m_useSDKForInput) {
						// Get the Unity KeyCode for the Unity API
						KeyCode unityKeyCode = OuyaSDK.GetUnityKeyCode (keyCode, player);
			
						// Check if the KeyCode is valid
						if (unityKeyCode != (KeyCode)(-1)) {
								//use the Unity API to get the button value
								bool buttonState = Input.GetKey (unityKeyCode);
								return buttonState;
						}
				}
				// moving the common code into the sdk via aboveUs
				return OuyaExampleCommon.GetButton (keyCode, player);	
		}
#endif	
}