using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour,
OuyaSDK.IJoystickCalibrationListener,
OuyaSDK.IPauseListener, OuyaSDK.IResumeListener,
OuyaSDK.IMenuButtonUpListener,
OuyaSDK.IMenuAppearingListener
{		
		//Delegates
		public delegate void AchievementReceived (Achievement achievement,int playerIndex,int newScore);
		public static AchievementReceived AchievementReceivedListeners; //check if null
		
		//Type
		public bool isNPC = false;
		
		//Control variables
		public bool grabHeld;
		public bool jumpDisabled;
		public bool jointDisabled;
		public bool pickUpDisabled;
		public bool slingShotDisabled;
		public bool jointLengthChangeDisabled;
		
		//Debounce variables
		// public int jumpDebounce;
		private float debounceY = 0.85f;
		private bool jumpNotReleased;
		private bool prevGrabPressed;
	
		//Grab indicators
		private GameObject grabIndicator;
		private GameObject grabRadiusIndicator;
		
		//Spawn parameters
		public  Color spawnColor;
		public  GameObject spawnLocation;
	
		//Controller info
		private bool keyboardDebugMode = false;
		public OuyaSDK.OuyaPlayer playerIndex = OuyaSDK.OuyaPlayer.player1;
		public bool splitController = false;
		public bool leftSplit = false;
		private bool m_useSDKForInput = false;

		//Jump status variables
		public bool jump = false;				// Condition for whether the player should jump.
	
		//Movement variables
		public float moveDrag = 0.1f;
		public float maxSpeed = 5.0f;				// The fastest the player can travel in the x axis.
		public float backJumpForce = -100.0f;
		private float slingshotForce = 130.0f; //130.0f
		private float npcSlingshotForce = 250.0f;
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
		private float pickUpDistance = 1.5f;
		private float jointChangeAmount = 0.1f;
		private GameObject slingshottedByPlayer;
		private GameObject pastSlingshottedByPlayer;
		private bool  slingshotJoint;
		private float slingshotDistance = 0.5f;
		private  int lastSlingshotId;
		private  int lastThrowId;
		private DistanceJoint2D joint;
		private PlayerController connectedPlayerController;
		public  bool pickedUp;
		
		//Grounding variables
		public GameObject possibleGroundedByPlayer;
		public GameObject groundedByPlayer;
		
		//private bool facingRight = true;			// For determining which way the player is currently facing.
	
		void Awake ()
		{
				OuyaSDK.registerJoystickCalibrationListener (this);
				OuyaSDK.registerMenuButtonUpListener (this);
				OuyaSDK.registerMenuAppearingListener (this);
				OuyaSDK.registerPauseListener (this);
				OuyaSDK.registerResumeListener (this);
				
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
				keyboardDebugMode = OuyaSDK.GetSupportedController (OuyaSDK.OuyaPlayer.player1) == null;
		
				Color grabRadiusIndicatorColor = gameObject.renderer.material.color;
				grabRadiusIndicatorColor.a = 0.5f;
				grabRadiusIndicator.renderer.material.color = grabRadiusIndicatorColor; 
				
				if (isNPC) {
						spawnLocation = new GameObject ();
						spawnLocation.transform.position = gameObject.transform.position;
						spawnLocation.transform.parent = gameObject.transform;
				} else
						Debug.Log ("Player Index: " + (int)playerIndex + " has instance id: " + gameObject.GetInstanceID ());
		}
	
		void OnDestroy ()
		{
				OuyaSDK.unregisterJoystickCalibrationListener (this);
				OuyaSDK.unregisterMenuButtonUpListener (this);
				OuyaSDK.unregisterMenuAppearingListener (this);
				OuyaSDK.unregisterPauseListener (this);
				OuyaSDK.unregisterResumeListener (this);
		}
		
		void Update ()
		{		
				UpdateGrounded ();
				pickedUp &= (joint != null && joint.distance < pickUpDistance) || (connectedPlayerController != null && connectedPlayerController.joint.distance < pickUpDistance);
				bool connected = Connected ();
				bool grabPressed = true;
				
				if (!isNPC) {
						if (!jointLengthChangeDisabled)
								AdjustJointLength ();
						CheckForKeyboardPause ();
				
						bool jumpPressed = JumpPressed () && !jumpDisabled;
						jumpNotReleased &= jumpPressed;
						//		jumpPressed &= !jumpNotReleased;
		
						if (jumpPressed && grounded && !pickedUp && !jumpNotReleased) {
								jump = true;
								jumpNotReleased = true;
								possibleGroundedByPlayer = groundedByPlayer;
						}
		
						grabPressed = GrabPressed () || grabHeld;

						if (!connected && (grabPressed || jumpPressed)) {
								Collider2D[] hitColliders = Physics2D.OverlapCircleAll (gameObject.transform.position, holdingDistance);
								foreach (Collider2D collider in hitColliders) {
										if (collider.gameObject != gameObject && (collider.gameObject.CompareTag ("Player") || collider.gameObject.CompareTag ("NPC"))) {		
												PlayerController playerController = collider.gameObject.GetComponent<PlayerController> ();
					
												if (playerController.AcceptConnectionOrSlingshot ()) {
														if (jumpPressed && lastSlingshotId != collider.gameObject.GetInstanceID () && !jumpNotReleased && !slingShotDisabled) {
																lastSlingshotId = collider.gameObject.GetInstanceID ();
						
																jump = true;
																jumpNotReleased = true;
																slingshottedByPlayer = collider.gameObject;
														
																StopMovement (gameObject);
																StopMovement (slingshottedByPlayer);
																break;
														} else if (grabPressed && !jointDisabled) {
																MakeConnection (collider.gameObject, playerController, true);
																break;
														}
												}
										}
								}
						} else if (connected && !grabPressed && (!slingshotJoint || ReleaseSlingshot ())) {
								ThrowPlayer ();
								DestroyConnection ();
						}
				}
				grabIndicator.SetActive (connected);
				grabRadiusIndicator.SetActive (grabPressed && !connected);
				grabRadiusIndicator.transform.localScale = new Vector3 (2 * holdingDistance + gameObject.transform.localScale.x / 2.0f, 0.1f, 2 * holdingDistance + gameObject.transform.localScale.y / 2.0f);
		}
		
		bool ReleaseSlingshot ()
		{
				return gameObject.transform.position.y > pastSlingshottedByPlayer.transform.position.y && Mathf.Abs (gameObject.transform.position.x - pastSlingshottedByPlayer.transform.position.x) < slingshotDistance;
		}
		
		void FixedUpdate ()
		{
				Vector2 velocity = rigidbody2D.velocity;
				if (!isNPC) {
						if (grounded || !Connected ()) {
								float h = GetHorizontalMovement ();
								if (grounded)
										velocity.x = maxSpeed * h;
								else if (Mathf.Abs (velocity.x) <= maxSpeed)
										velocity.x = maxSpeed * h;
								else if (h * velocity.x < 0)
										velocity.x = velocity.x + h * maxSpeed;
						}
		
						if (jump) {
								if (slingshottedByPlayer == null) {
										velocity.y = jumpVelocity;
										if (groundedByPlayer != null)
												groundedByPlayer.transform.rigidbody2D.AddForce (new Vector2 (0f, backJumpForce));
								} else {
										if (gameObject.transform.position.y < slingshottedByPlayer.transform.position.y)
												Slingshot ();
										pastSlingshottedByPlayer = slingshottedByPlayer;
										slingshottedByPlayer = null;   
								}
			
								jump = false;
						}		
				} else if (grounded)
						velocity.x = 0.0f;
					
				rigidbody2D.velocity = velocity;
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
		
		void Slingshot ()
		{
				if (slingshottedByPlayer.GetComponent<PlayerController> ().isNPC) {
						slingshotJoint = true;
						MakeConnection (slingshottedByPlayer, slingshottedByPlayer.GetComponent<PlayerController> (), false);
						float direction = gameObject.transform.position.x - slingshottedByPlayer.transform.position.x;
						Vector2 forceVector = new Vector2 (0.0f, 0.5f);
						if (direction > 0)
								forceVector.x = 0.5f;
						else if (direction < 0)
								forceVector.x = -0.5f;
		
						forceVector *= npcSlingshotForce;
						rigidbody2D.AddForce (forceVector);
				} else {
						float direction = gameObject.transform.position.x - slingshottedByPlayer.transform.position.x;
						Vector2 endPosition = slingshottedByPlayer.transform.position;
						if (direction > 0)
								endPosition.x += slingshottedByPlayer.transform.localScale.x;// * 1.5f;
						else if (direction < 0)
								endPosition.x -= slingshottedByPlayer.transform.localScale.x;// * 1.5f;
			
						endPosition.y += slingshottedByPlayer.transform.localScale.y * 2.0f;
			
						Vector2 forceVector = new Vector2 ((endPosition.x - gameObject.transform.position.x), (endPosition.y - gameObject.transform.position.y)).normalized;
						forceVector *= slingshotForce;
						rigidbody2D.AddForce (forceVector);
						slingshottedByPlayer.rigidbody2D.AddForce (-forceVector);
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
						if (joint != null) {
								jointToUse = joint;
								otherTransform = joint.connectedBody.gameObject.transform;
						} else {
								jointToUse = connectedPlayerController.joint;
								otherTransform = connectedPlayerController.gameObject.transform;
						}
						
						otherPlayerIsNPC = otherTransform.gameObject.GetComponent<PlayerController> ().isNPC;
						bool increase = IncreaseJointLength ();
						bool decrease = DecreaseJointLength ();
						
						if (increase || decrease) {
						
								float halfJointDistance = jointChangeAmount / 2.0f;
								Vector3 originalVectorToOther = otherTransform.position - gameObject.transform.position;
								Vector3 vectorToOtherPlayer = originalVectorToOther.normalized;
								vectorToOtherPlayer *= halfJointDistance;
				
								if (increase) {
										if (jointToUse.distance < holdingDistance - jointChangeAmount) {
												if (!CollisionIfGameObjectMovedAlongVector (gameObject, -vectorToOtherPlayer)) {
														jointToUse.distance += halfJointDistance;
														if (Vector3.SqrMagnitude (originalVectorToOther) < jointToUse.distance * jointToUse.distance)
																gameObject.transform.position -= vectorToOtherPlayer;
												}
					
												if (!otherPlayerIsNPC && !CollisionIfGameObjectMovedAlongVector (otherTransform.gameObject, vectorToOtherPlayer)) {
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
					
												if (!otherPlayerIsNPC && !CollisionIfGameObjectMovedAlongVector (otherTransform.gameObject, -vectorToOtherPlayer)) {
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
				if (joint.distance <= pickUpDistance && !pickUpDisabled && otherPlayerController.lastThrowId != gameObject.GetInstanceID () && !otherPlayerController.isNPC) {
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
		
				if (throwPlayer) {
						float fractionX = Mathf.Abs (gameObject.rigidbody2D.velocity.x / maxSpeed);//maxSpeed;
						float direction = gameObject.rigidbody2D.velocity.x < 0 ? -1 : 1;
						if (fractionX == 0)
								direction = 0;
						Vector2 forceVector = new Vector2 ((1.0f + fractionX) * throwForceX * direction, (2.0f - fractionX) * throwForceY);
						otherPlayer.rigidbody2D.AddForce (forceVector);
						otherPlayerController.lastThrowId = gameObject.GetInstanceID ();
				}
		}
		
		//Collision check
		bool CollisionIfGameObjectMovedAlongVector (GameObject movingObject, Vector3 vector)
		{
				Collider2D[] colliders = Physics2D.OverlapAreaAll (BottomLeft (movingObject) + vector, 
		                                                   TopRight (movingObject) + vector);
		
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
						DestroyConnection ();
						StopMovement (gameObject);
						StartCoroutine ("RespawnPlayer");
				} else if (other.gameObject.CompareTag ("Coin")) {
						Achievement newAchievement = other.gameObject.GetComponent<CoinScript> ().getAchievement ();
						achievements.Add (newAchievement);
						score += newAchievement.points;
						if (AchievementReceivedListeners != null)
								AchievementReceivedListeners (newAchievement, (int)playerIndex, score);
						Destroy (other.gameObject);
				}
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
		
		bool AcceptConnectionOrSlingshot ()
		{
				return (!Connected () && (GrabPressed () || isNPC));
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
				slingshotJoint = false;
		}
	
		public void OuyaMenuButtonUp ()
		{
				GameLogic.Instance.PausePressed ((int)playerIndex);
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
	
		void CheckForKeyboardPause ()
		{
				if (Input.GetKeyUp (KeyCode.P) && playerIndex == OuyaSDK.OuyaPlayer.player1) {
						if (Time.timeScale != 0.0f)
								Time.timeScale = 0.0f;
						else
								Time.timeScale = 1.0f;
				}
		}
	
		bool JumpPressed ()
		{
				if (!keyboardDebugMode) {
						if (!splitController) {
								return GetButton (OuyaSDK.KeyEnum.BUTTON_O, playerIndex) || 
										GetButton (OuyaSDK.KeyEnum.BUTTON_RB, playerIndex) ||
										GetButton (OuyaSDK.KeyEnum.BUTTON_DPAD_UP, playerIndex); // ||
								//	(-1 * GetAxis (OuyaSDK.KeyEnum.AXIS_LSTICK_Y, playerIndex) > debounceY);
						} else {
								if (leftSplit) {
										return GetButton (OuyaSDK.KeyEnum.BUTTON_LB, playerIndex) ||
												GetButton (OuyaSDK.KeyEnum.BUTTON_DPAD_UP, playerIndex);// || 
										//	(-1 * GetAxis (OuyaSDK.KeyEnum.AXIS_LSTICK_Y, playerIndex) > debounceY);
								} else {
										return GetButton (OuyaSDK.KeyEnum.BUTTON_O, playerIndex) || 
												GetButton (OuyaSDK.KeyEnum.BUTTON_RB, playerIndex);// ||
										//	(-1 * GetAxis (OuyaSDK.KeyEnum.AXIS_RSTICK_Y, playerIndex) > debounceY);
								}
						}
				} else if (playerIndex == OuyaSDK.OuyaPlayer.player1)   
						return Input.GetKey (KeyCode.W);
				else if (playerIndex == OuyaSDK.OuyaPlayer.player2)   
						return Input.GetKey (KeyCode.I);
				else
						return false;
		}
		
		bool IncreaseJointLength ()
		{
				bool increase = false;
		
				if (!keyboardDebugMode) {
						if (leftSplit)
								increase = (-1 * GetAxis (OuyaSDK.KeyEnum.AXIS_LSTICK_Y, playerIndex)) > debounceY;
						else
								increase = (-1 * GetAxis (OuyaSDK.KeyEnum.AXIS_RSTICK_Y, playerIndex)) > debounceY;
				} else if (playerIndex == OuyaSDK.OuyaPlayer.player1)   
						increase = Input.GetKey (KeyCode.E);
				else if (playerIndex == OuyaSDK.OuyaPlayer.player2)   
						increase = Input.GetKey (KeyCode.O);

		
				return increase;
		}
	
		bool DecreaseJointLength ()
		{
				bool decrease = false;
		
				if (!keyboardDebugMode) {
						if (leftSplit)
								decrease = (-1 * GetAxis (OuyaSDK.KeyEnum.AXIS_LSTICK_Y, playerIndex)) < -debounceY;
						else
								decrease = (-1 * GetAxis (OuyaSDK.KeyEnum.AXIS_RSTICK_Y, playerIndex)) < -debounceY;
				} else if (playerIndex == OuyaSDK.OuyaPlayer.player1)   
						decrease = Input.GetKey (KeyCode.Q);
				else if (playerIndex == OuyaSDK.OuyaPlayer.player2)   
						decrease = Input.GetKey (KeyCode.U);

				return decrease;
		}
		
		bool GrabPressed ()
		{
				bool pressed = false;
		
				if (!keyboardDebugMode) {
						if (leftSplit)
								pressed = GetButton (OuyaSDK.KeyEnum.BUTTON_LT, playerIndex);
						else
								pressed = GetButton (OuyaSDK.KeyEnum.BUTTON_RT, playerIndex);
				} else if (playerIndex == OuyaSDK.OuyaPlayer.player2)   
						pressed = Input.GetKey (KeyCode.Space);
				else if (playerIndex == OuyaSDK.OuyaPlayer.player1)   
						pressed = Input.GetKey (KeyCode.LeftShift);
				
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
		

				if (!keyboardDebugMode) {
						if (!splitController || leftSplit)
								horizontal = GetAxis (OuyaSDK.KeyEnum.AXIS_LSTICK_X, playerIndex);
						else
								horizontal = GetAxis (OuyaSDK.KeyEnum.AXIS_RSTICK_X, playerIndex);
					
						if (horizontal == 0.0f && (!splitController || leftSplit)) {
								if (GetButton (OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT, playerIndex))
										return - 1.0f;
								else if (GetButton (OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT, playerIndex))
										return 1.0f;
						}
				} else if (Input.GetKey (KeyCode.A) && playerIndex == OuyaSDK.OuyaPlayer.player1)
						horizontal = -1.0f;
				else if (Input.GetKey (KeyCode.D) && playerIndex == OuyaSDK.OuyaPlayer.player1)
						horizontal = 1.0f;
				else if (Input.GetKey (KeyCode.J) && playerIndex == OuyaSDK.OuyaPlayer.player2)
						horizontal = -1.0f;
				else if (Input.GetKey (KeyCode.L) && playerIndex == OuyaSDK.OuyaPlayer.player2)
						horizontal = 1.0f;	
				
				return horizontal;
		}
		
		void UpdateGrounded ()
		{
				grounded = PlayerIsGrounded (gameObject.GetInstanceID (), true);
		}
	
		bool PlayerIsGrounded (int originalId, bool originalCall)
		{
				bool playerGrounded = false;
				int layerMask = 1 << LayerMask.NameToLayer ("Ground");
				layerMask |= 1 << LayerMask.NameToLayer ("Player");
				layerMask |= 1 << LayerMask.NameToLayer ("NPC");
				Vector3 startRaycast = transform.position;
				startRaycast.y -= transform.localScale.y / 1.99f;
		
				foreach (Transform groundCheck in groundChecks) {
						RaycastHit2D[] hits = Physics2D.LinecastAll (startRaycast, groundCheck.position, layerMask);
						foreach (RaycastHit2D raycastInfo in hits) {
								if (!(raycastInfo.collider.isTrigger || raycastInfo.collider.gameObject == gameObject)) {
										int colliderInstanceId = raycastInfo.collider.gameObject.GetInstanceID ();
										playerGrounded = true;		
										possibleGroundedByPlayer = null;
										
										if (!raycastInfo.collider.CompareTag ("Player") && !raycastInfo.collider.CompareTag ("NPC")) {
												lastThrowId = 0;
												lastSlingshotId = 0;	
										}
										
										if (groundedByPlayer != null && colliderInstanceId != groundedByPlayer.GetInstanceID ())
												groundedByPlayer = null;
										
										if ((raycastInfo.collider.CompareTag ("Player") || raycastInfo.collider.CompareTag ("NPC")) && (originalCall || gameObject.GetInstanceID () != originalId) && 
												raycastInfo.collider.gameObject.GetComponent<PlayerController> ().PlayerIsGrounded (originalId, false)) {
												lastThrowId = 0;
												lastSlingshotId = 0;
												possibleGroundedByPlayer = raycastInfo.collider.gameObject;
										}
	
								}
						}
				}
				
				return playerGrounded;
		}
		
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
								if (OuyaSDK.GetAxisInverted (keyCode, player)) {
										return -axisValue;
								} else {
										return axisValue;
								}
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
	
}