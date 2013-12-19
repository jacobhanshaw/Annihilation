using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour,
OuyaSDK.IJoystickCalibrationListener,
OuyaSDK.IPauseListener, OuyaSDK.IResumeListener,
OuyaSDK.IMenuButtonUpListener,
OuyaSDK.IMenuAppearingListener
{
		//private bool facingRight = true;			// For determining which way the player is currently facing.
		//private float debounceX = 0.25f; //unused
		private float debounceY = 0.85f;

		//Grab indicators
		private GameObject grabIndicator;
		private GameObject grabRadiusIndicator;
		
		//Spawn parameters
		public  Color spawnColor;
		public  GameObject spawnLocation;
	
		//Controller info
		private bool keyboardDebugMode = false;
		public OuyaSDK.OuyaPlayer index = OuyaSDK.OuyaPlayer.player1;
		public bool splitController = false;
		public bool leftSplit = false;
		private bool m_useSDKForInput = false;
		
		//Debounce variables
		// public int jumpDebounce;
		private bool jumpNotReleased;
		private bool prevGrabPressed;
		
		//Jump status variables
		public bool jump = false;				// Condition for whether the player should jump.
		public bool jumping = false;
	
		//Movement variables
		public float moveForce = 365f;			// Amount of force added to move the player left and right.
		public float moveDrag = 0.1f;
		public float maxSpeed = 5.0f;				// The fastest the player can travel in the x axis.
		public float backJumpForce = -100.0f;
		public float slingShotForce = 50.0f;
		private float throwForceX = 100.0f;
		private float throwForceY = 50.0f;
		public float jumpVelocity = 7.5f;			// Amount of force added when the player jumps.

		//Grounding variables
		private Transform[] groundChecks = new Transform[3];			// A position marking where to check if the player is grounded.
		public  bool grounded = false;			// Whether or not the player is grounded.
		private GameObject slingshottedByPlayer;
		
		//Achievement variables
		public int score;
		public  List<Achievement> achievements;
	
		private float holdingDistance = 4.0f;
		private float pickUpDistance = 1.5f;
		private float jointChangeAmount = 0.1f;
		private  int lastSlingshotId;
		private DistanceJoint2D joint;
		private PlayerController connectedPlayerController;
		public  bool pickedUp;
		
		public GameObject possibleGroundedByPlayer;
		public GameObject groundedByPlayer;
	
		void Awake ()
		{
				OuyaSDK.registerJoystickCalibrationListener (this);
				OuyaSDK.registerMenuButtonUpListener (this);
				OuyaSDK.registerMenuAppearingListener (this);
				OuyaSDK.registerPauseListener (this);
				OuyaSDK.registerResumeListener (this);
				
				grabIndicator = transform.Find ("grabIndicator").gameObject;
				grabRadiusIndicator = transform.Find ("grabRadiusIndicator").gameObject;
				
				groundChecks [0] = transform.Find ("groundChecka");
				groundChecks [1] = transform.Find ("groundCheckb");
				groundChecks [2] = transform.Find ("groundCheckc");
				
				achievements = new List<Achievement> ();
		}
	
		void OnDestroy ()
		{
				OuyaSDK.unregisterJoystickCalibrationListener (this);
				OuyaSDK.unregisterMenuButtonUpListener (this);
				OuyaSDK.unregisterMenuAppearingListener (this);
				OuyaSDK.unregisterPauseListener (this);
				OuyaSDK.unregisterResumeListener (this);
		}

		void Start ()
		{
				Input.ResetInputAxes ();
				keyboardDebugMode = OuyaSDK.GetSupportedController (OuyaSDK.OuyaPlayer.player1) == null;
				Color grabRadiusIndicatorColor = gameObject.renderer.material.color;
				grabRadiusIndicatorColor.a = 0.5f;
				grabRadiusIndicator.renderer.material.color = grabRadiusIndicatorColor;
		}
		/*
		void OnCollisionStay2D (Collision2D collisionInfo)
		{
				foreach (ContactPoint2D contact in collisionInfo.contacts) {
						Debug.Log (contact);
						Debug.DrawRay (contact.point, contact.normal, Color.black);
				}
		}
	*/
		Vector3 BottomLeft (GameObject box)
		{
				return box.transform.position - new Vector3 (box.transform.localScale.x / 2.0f, box.transform.localScale.y / 2.05f, 0);
		}
	
		Vector3 TopRight (GameObject box)
		{
				return box.transform.position + new Vector3 (box.transform.localScale.x / 2.0f, box.transform.localScale.y / 2.0f, 0);
		}
	
		void AdjustJointLength ()
		{
				if (Connected ()) {
						DistanceJoint2D jointToUse;
						Transform otherTransform;
						if (joint != null) {
								jointToUse = joint;
								otherTransform = joint.connectedBody.gameObject.transform;
						} else {
								jointToUse = connectedPlayerController.joint;
								otherTransform = connectedPlayerController.gameObject.transform;
						}
						
						if (IncreaseJointLength ()) {
								if (jointToUse.distance < holdingDistance - jointChangeAmount) {
										Vector3 vectorToOtherPlayer = (otherTransform.position - gameObject.transform.position).normalized;
										vectorToOtherPlayer *= (jointChangeAmount / 2.0f);
										Collider2D[] colliders = Physics2D.OverlapAreaAll (BottomLeft (gameObject) - vectorToOtherPlayer, 
																			       TopRight (gameObject) - vectorToOtherPlayer);

										bool otherObjectFound = false;
										foreach (Collider2D aCollider in colliders) {
												if (aCollider.gameObject != gameObject) {
														otherObjectFound = true;
														break;
												}
										}
								
										if (!otherObjectFound) {
												//	gameObject.transform.position -= vectorToOtherPlayer;
												jointToUse.distance += jointChangeAmount / 2.0f;
										}
								
										colliders = Physics2D.OverlapAreaAll (BottomLeft (otherTransform.gameObject) + vectorToOtherPlayer, 
				                                                   TopRight (otherTransform.gameObject) + vectorToOtherPlayer);
										otherObjectFound = false;
										foreach (Collider2D aCollider in colliders) {
												if (aCollider.gameObject != otherTransform.gameObject) {
														otherObjectFound = true;
														break;
												}
										}
				
										if (!otherObjectFound) {
												//	otherTransform.position += vectorToOtherPlayer;
												jointToUse.distance += jointChangeAmount / 2.0f;
										}
								
										if (jointToUse.distance > holdingDistance)
												jointToUse.distance = holdingDistance;
								}
						}
		
						if (DecreaseJointLength ()) {
								Vector3 vectorToOtherPlayer = (otherTransform.position - gameObject.transform.position).normalized;
								vectorToOtherPlayer *= (jointChangeAmount / 2.0f);
								Collider2D[] colliders = Physics2D.OverlapAreaAll (BottomLeft (gameObject) + vectorToOtherPlayer, 
				                                                   TopRight (gameObject) + vectorToOtherPlayer);
								bool otherObjectFound = false;
								foreach (Collider2D aCollider in colliders) {
										if (aCollider.gameObject != gameObject) {
												otherObjectFound = true;
												break;
										}
								}
				
								if (!otherObjectFound) {
										jointToUse.distance -= jointChangeAmount / 2.0f;
										if (Vector3.SqrMagnitude (otherTransform.position - gameObject.transform.position) > jointToUse.distance * jointToUse.distance)
												gameObject.transform.position += vectorToOtherPlayer;
										
								}
				
								colliders = Physics2D.OverlapAreaAll (BottomLeft (otherTransform.gameObject) - vectorToOtherPlayer, 
				                                      TopRight (otherTransform.gameObject) - vectorToOtherPlayer);
								otherObjectFound = false;
								foreach (Collider2D aCollider in colliders) {
										if (aCollider.gameObject != otherTransform.gameObject) {
												otherObjectFound = true;
												break;
										}
								}
				
								if (!otherObjectFound) {
										jointToUse.distance -= jointChangeAmount / 2.0f;
										if (Vector3.SqrMagnitude (otherTransform.position - gameObject.transform.position) > jointToUse.distance * jointToUse.distance)
												otherTransform.position -= vectorToOtherPlayer;

								}
		
								if (jointToUse.distance < pickUpDistance)
										PickUpPlayer (jointToUse, otherTransform);
						}
						
				}	
		}
	
		void PickUpPlayer (DistanceJoint2D joint, Transform otherPlayer)
		{
				otherPlayer.position = gameObject.transform.position + new Vector3 (0.0f, gameObject.transform.localScale.y, 0.0f);
				joint.distance = gameObject.transform.localScale.y;
				otherPlayer.GetComponent<PlayerController> ().pickedUp = true;
		}
	
		void ThrowPlayer ()
		{
				if ((connectedPlayerController != null && connectedPlayerController.pickedUp) || (joint != null && joint.connectedBody.GetComponent<PlayerController> ().pickedUp)) {
						Debug.Log ("Tried Throw");
						float fractionX = Mathf.Abs (gameObject.rigidbody2D.velocity.x / maxSpeed);//maxSpeed;
						float direction = gameObject.rigidbody2D.velocity.x < 0 ? -1 : 1;
						if (fractionX == 0)
								direction = 0;
						Vector2 forceVector = new Vector2 ((1.0f + fractionX) * throwForceX * direction, (2.0f - fractionX) * throwForceY);
						Debug.Log ("Force Vector: " + forceVector);
						OtherPlayer ().rigidbody2D.AddForce (forceVector);
				}
			
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
						StartCoroutine ("RespawnPlayer");
				} else if (other.gameObject.CompareTag ("Coin")) {
						Achievement newAchievement = other.gameObject.GetComponent<CoinScript> ().getAchievement ();
						achievements.Add (newAchievement);
						score += newAchievement.points;
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
		}
	
		void Update ()
		{		
				UpdateGrounded ();
				AdjustJointLength ();
				
				bool jumpPressed = JumpPressed ();
				jumpNotReleased &= jumpPressed;
				pickedUp &= (joint != null && joint.distance < pickUpDistance) || (connectedPlayerController != null && connectedPlayerController.joint.distance < pickUpDistance);
		
				if (jumpPressed && grounded && !pickedUp && !jumping && !jumpNotReleased) {
						jump = true;
						jumpNotReleased = true;
						possibleGroundedByPlayer = groundedByPlayer;
				}
		
				bool grabPressed = GrabPressed ();
		
				if (!Connected () && (grabPressed || jumpPressed)) {
						Collider2D[] hitColliders = Physics2D.OverlapCircleAll (gameObject.transform.position, holdingDistance);
						foreach (Collider2D collider in hitColliders) {
								if (collider.gameObject != gameObject && !Connected () && collider.gameObject.CompareTag ("Player")) {		
										PlayerController playerController = collider.gameObject.GetComponent<PlayerController> ();
										
										if (playerController.AllowSlingShot () && (lastSlingshotId != collider.gameObject.GetInstanceID ()) && jumpPressed && !jumpNotReleased) {
												lastSlingshotId = collider.gameObject.GetInstanceID ();
												
												jump = true;
												jumpNotReleased = true;
												slingshottedByPlayer = collider.gameObject;
										} else if (playerController.AcceptConnection () && grabPressed) {
												playerController.jumping = false;
												jumping = false;
												grabIndicator.SetActive (true);
												grabIndicator.renderer.material.color = collider.gameObject.renderer.material.color;
												playerController.grabIndicator.SetActive (true);
												playerController.grabIndicator.renderer.material.color = gameObject.renderer.material.color;
						
												joint = gameObject.AddComponent ("DistanceJoint2D") as DistanceJoint2D;
												joint.collideConnected = true;
												joint.connectedBody = collider.gameObject.rigidbody2D;
												joint.distance = holdingDistance;
												
												playerController.connectedPlayerController = this;
												break;
										}
								}
						}
				} else if (Connected () && !grabPressed) {
						ThrowPlayer ();
						DestroyConnection ();
				}
				
				grabIndicator.SetActive (Connected ());
				grabRadiusIndicator.SetActive (grabPressed && !Connected ());
		}
		
		bool AllowSlingShot ()
		{
				return (!Connected () && GrabPressed ());
		}
		
		bool AcceptConnection ()
		{
				return (!Connected () && GrabPressed ());
		}
	
		void UpdateGrounded ()
		{
				grounded = PlayerIsGrounded (gameObject.GetInstanceID (), true);
				jumping &= !grounded;
		}
	
		public void OuyaMenuButtonUp ()
		{
		}
	
		public void OuyaMenuAppearing ()
		{
		}
	
		//NOT TESTED!!!!!!!!!
		public void OuyaOnPause ()
		{
				Time.timeScale = 0.0f;
		}
	
		public void OuyaOnResume ()
		{
				Time.timeScale = 1.0f;
		}
	
		public void OuyaOnJoystickCalibration ()
		{
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
		else {
						return OuyaExampleCommon.GetAxis (keyCode, player);
				}
				return 0f;
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
		else {
						return OuyaExampleCommon.GetButton (keyCode, player);
				}
				return false;
		}
	
		void FixedUpdate ()
		{
				Vector2 velocity = rigidbody2D.velocity;
			
				float h = GetHorizontalMovement ();
		
				if (grounded)// || (jumping && !Connected ()))
						velocity.x = maxSpeed * h;
				else if (!Connected ()) {
						float originalSpeed = velocity.x;
						if (Mathf.Abs (originalSpeed) <= maxSpeed || h * originalSpeed < 0) {
								velocity.x = originalSpeed + h * maxSpeed;
								if (Mathf.Abs (velocity.x) > maxSpeed) {
										if (velocity.x < 0)
												velocity.x = -maxSpeed;
										else
												velocity.x = maxSpeed;
								} else if (h == 0) {
										if (velocity.x < 0) {
												velocity.x += moveDrag;
												if (velocity.x > 0)
														velocity.x = 0;
										} else {
												velocity.x -= moveDrag;
												if (velocity.x < 0)
														velocity.x = 0;
										}
								}
						}
						
				}
	
				if (jump) {
						if (slingshottedByPlayer == null) {
								velocity.y = jumpVelocity;
								if (groundedByPlayer != null) {
										//velocity.y = jumpVelocity / 1.1f;
										groundedByPlayer.transform.rigidbody2D.AddForce (new Vector2 (0f, backJumpForce));
								}	
						} else {
								
								float direction = gameObject.transform.position.x - slingshottedByPlayer.transform.position.x;
								Vector2 endPosition = slingshottedByPlayer.transform.position;
								if (direction > 0)
										endPosition.x += slingshottedByPlayer.transform.localScale.x * 1.5f;
								else if (direction < 0)
										endPosition.x -= slingshottedByPlayer.transform.localScale.x * 1.5f;
										
								endPosition.y += slingshottedByPlayer.transform.localScale.y * 2.0f;
								
								float distance = Vector2.Distance (gameObject.transform.position, endPosition);
								
						
								float percentMaxX = (endPosition.x - gameObject.transform.position.x) / distance;
								float percentMaxY = (endPosition.y - gameObject.transform.position.y) / distance;
								
								rigidbody2D.AddForce (new Vector2 (percentMaxX * slingShotForce, 
				                                   percentMaxY * slingShotForce));
								slingshottedByPlayer.rigidbody2D.AddForce (new Vector2 (-percentMaxX * slingShotForce, 
				                                                        -percentMaxY * slingShotForce));
								slingshottedByPlayer = null;                   
						}

						// Make sure the player can't jump again until the jump conditions from Update are satisfied.
						jump = false;
						jumping = true;
						
				}

		
				rigidbody2D.velocity = velocity;
		}
	
		bool JumpPressed ()
		{
				if (!keyboardDebugMode) {
						if (!splitController) {
								return GetButton (OuyaSDK.KeyEnum.BUTTON_O, index) || 
										GetButton (OuyaSDK.KeyEnum.BUTTON_RB, index) ||
										GetButton (OuyaSDK.KeyEnum.BUTTON_DPAD_UP, index); // ||
								//	(-1 * GetAxis (OuyaSDK.KeyEnum.AXIS_LSTICK_Y, index) > debounceY);
						} else {
								if (leftSplit) {
										return GetButton (OuyaSDK.KeyEnum.BUTTON_LB, index) ||
												GetButton (OuyaSDK.KeyEnum.BUTTON_DPAD_UP, index);// || 
										//	(-1 * GetAxis (OuyaSDK.KeyEnum.AXIS_LSTICK_Y, index) > debounceY);
								} else {
										return GetButton (OuyaSDK.KeyEnum.BUTTON_O, index) || 
												GetButton (OuyaSDK.KeyEnum.BUTTON_RB, index);// ||
										//	(-1 * GetAxis (OuyaSDK.KeyEnum.AXIS_RSTICK_Y, index) > debounceY);
								}
						}
				} else if (index == OuyaSDK.OuyaPlayer.player1)   
						return Input.GetKey (KeyCode.W);
				else if (index == OuyaSDK.OuyaPlayer.player2)   
						return Input.GetKey (KeyCode.I);
				else
						return false;
		}
		
		bool IncreaseJointLength ()
		{
				bool increase = false;
		
				if (!keyboardDebugMode) {
						if (leftSplit)
								increase = (-1 * GetAxis (OuyaSDK.KeyEnum.AXIS_LSTICK_Y, index)) > debounceY;
						else
								increase = (-1 * GetAxis (OuyaSDK.KeyEnum.AXIS_RSTICK_Y, index)) > debounceY;
				} else if (index == OuyaSDK.OuyaPlayer.player1)   
						increase = Input.GetKey (KeyCode.E);
				else if (index == OuyaSDK.OuyaPlayer.player2)   
						increase = Input.GetKey (KeyCode.O);

		
				return increase;
		}
	
		bool DecreaseJointLength ()
		{
				bool decrease = false;
		
				if (!keyboardDebugMode) {
						if (leftSplit)
								decrease = (-1 * GetAxis (OuyaSDK.KeyEnum.AXIS_LSTICK_Y, index)) < -debounceY;
						else
								decrease = (-1 * GetAxis (OuyaSDK.KeyEnum.AXIS_RSTICK_Y, index)) < -debounceY;
				} else if (index == OuyaSDK.OuyaPlayer.player1)   
						decrease = Input.GetKey (KeyCode.Q);
				else if (index == OuyaSDK.OuyaPlayer.player2)   
						decrease = Input.GetKey (KeyCode.U);

				return decrease;
		}
		
		bool GrabPressed ()
		{
				bool pressed = false;
		
				if (!keyboardDebugMode) {
						if (leftSplit)
								pressed = GetButton (OuyaSDK.KeyEnum.BUTTON_LT, index);
						else
								pressed = GetButton (OuyaSDK.KeyEnum.BUTTON_RT, index);
				} else if (index == OuyaSDK.OuyaPlayer.player2)   
						pressed = Input.GetKey (KeyCode.Space);
				else if (index == OuyaSDK.OuyaPlayer.player1)   
						pressed = Input.GetKey (KeyCode.LeftShift);
				
				if (prevGrabPressed != pressed) {
						prevGrabPressed = pressed;
						return !prevGrabPressed;
				}
				
				prevGrabPressed = pressed;
				return pressed;
		}
		
		float GetHorizontalMovement ()
		{
				float horizontal = 0.0f;
		

				if (!keyboardDebugMode) {
						if (!splitController || leftSplit)
								horizontal = GetAxis (OuyaSDK.KeyEnum.AXIS_LSTICK_X, index);
						else
								horizontal = GetAxis (OuyaSDK.KeyEnum.AXIS_RSTICK_X, index);
					
						if (horizontal == 0.0f && (!splitController || leftSplit)) {
								if (GetButton (OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT, index))
										return - 1.0f;
								else if (GetButton (OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT, index))
										return 1.0f;
						}
				} else if (Input.GetKey (KeyCode.A) && index == OuyaSDK.OuyaPlayer.player1)
						horizontal = -1.0f;
				else if (Input.GetKey (KeyCode.D) && index == OuyaSDK.OuyaPlayer.player1)
						horizontal = 1.0f;
				else if (Input.GetKey (KeyCode.J) && index == OuyaSDK.OuyaPlayer.player2)
						horizontal = -1.0f;
				else if (Input.GetKey (KeyCode.L) && index == OuyaSDK.OuyaPlayer.player2)
						horizontal = 1.0f;	
				
				return horizontal;
		}
		
		bool PlayerIsGrounded (int originalId, bool originalCall)
		{
				bool playerGrounded = false;
				int layerMask = 1 << LayerMask.NameToLayer ("Ground");
				layerMask |= 1 << LayerMask.NameToLayer ("Player");
				Vector3 startRaycast = transform.position;
				startRaycast.y -= transform.localScale.y / 1.99f;
		
				foreach (Transform groundCheck in groundChecks) {
						RaycastHit2D[] hits = Physics2D.LinecastAll (startRaycast, groundCheck.position, layerMask);
						foreach (RaycastHit2D raycastInfo in hits) {
								if (!(raycastInfo.collider.isTrigger || raycastInfo.collider.gameObject == gameObject)) {
										int colliderInstanceId = raycastInfo.collider.gameObject.GetInstanceID ();
										playerGrounded = true;		
										possibleGroundedByPlayer = null;
										
										if (!raycastInfo.collider.CompareTag ("Player"))
												lastSlingshotId = 0;
										
										if (groundedByPlayer != null && colliderInstanceId != groundedByPlayer.GetInstanceID ())
												groundedByPlayer = null;
										
										if (raycastInfo.collider.CompareTag ("Player") && (originalCall || gameObject.GetInstanceID () != originalId) && 
												raycastInfo.collider.gameObject.GetComponent<PlayerController> ().PlayerIsGrounded (originalId, false)) {
												lastSlingshotId = 0;
												possibleGroundedByPlayer = raycastInfo.collider.gameObject;
										}
	
								}
						}
				}
				
				return playerGrounded;
		}
	
}