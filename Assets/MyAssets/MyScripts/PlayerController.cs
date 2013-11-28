using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour,
OuyaSDK.IJoystickCalibrationListener,
OuyaSDK.IPauseListener, OuyaSDK.IResumeListener,
OuyaSDK.IMenuButtonUpListener,
OuyaSDK.IMenuAppearingListener
{

		public TextMesh textMesh;
		private GameObject grabIndicator;
		private bool keyboardDebugMode = true;

		public OuyaSDK.OuyaPlayer index = OuyaSDK.OuyaPlayer.player1;
	
		private bool m_useSDKForInput = false;
		
		[HideInInspector]
		public bool
				facingRight = true;			// For determining which way the player is currently facing.
		
		public bool jump = false;				// Condition for whether the player should jump.
		public bool jumping = false;
		public int jumpedFromPlayer = -1;
	
		public float moveForce = 365f;			// Amount of force added to move the player left and right.
		public float maxSpeed = 3.0f;				// The fastest the player can travel in the x axis.
		public float jumpForce = 1000f;			// Amount of force added when the player jumps.

		private Transform[] groundChecks = new Transform[3];			// A position marking where to check if the player is grounded.
		private bool grounded = false;			// Whether or not the player is grounded.
		
		private float debounceDistance = 0.25f;
		private float extraGrabRadius = 0.2f;
		
		public bool splitController = false;
		public bool leftSplit = false;
		
		private float score;
		
		private float holdingDistance = 4.0f;
		private GameObject carriedPlayer;
		private GameObject carryingPlayer;
		private DistanceJoint2D jointToCarriedPlayer;
	
		void Awake ()
		{
				OuyaSDK.registerJoystickCalibrationListener (this);
				OuyaSDK.registerMenuButtonUpListener (this);
				OuyaSDK.registerMenuAppearingListener (this);
				OuyaSDK.registerPauseListener (this);
				OuyaSDK.registerResumeListener (this);
		
				grabIndicator = transform.Find ("grabIndicator").gameObject;
		
				groundChecks [0] = transform.Find ("groundChecka");
				groundChecks [1] = transform.Find ("groundCheckb");
				groundChecks [2] = transform.Find ("groundCheckc");
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
		}
		
		void OnTriggerStay (Collider other)
		{
				if (other.gameObject.CompareTag ("Points"))
						score += Time.deltaTime;
		}
		
		void DestroyConnection ()
		{
				if (carryingPlayer != null) {
						carryingPlayer.GetComponent<PlayerController> ().PlayerLaunched ();
						carryingPlayer = null;
				} else if (carriedPlayer != null) {
						carriedPlayer.GetComponent<PlayerController> ().carryingPlayer = null;
						carriedPlayer = null;
						PlayerLaunched ();
				}
		}
	
		void Update ()
		{
				grounded = PlayerIsGrounded ();
				
				jumping &= !grounded;
				if (grounded)
						jumpedFromPlayer = -1;
					
				bool shouldJump = JumpPressed ();
				shouldJump &= !jumping;
				
				if (shouldJump && carryingPlayer != null && jumpedFromPlayer != carryingPlayer.GetInstanceID ()) {
						jump = true;
						jumpedFromPlayer = carryingPlayer.GetInstanceID ();
						DestroyConnection ();
				} else if (shouldJump && grounded) {
						jump = true;
						DestroyConnection ();
				}
			
				bool grabPressed = GrabPressed ();
				
				if (carriedPlayer != null)
						grabIndicator.SetActive (true);
				else
						grabIndicator.SetActive (false);
					
				if (carriedPlayer == null && grabPressed) {
						Collider2D[] hitColliders = Physics2D.OverlapCircleAll (gameObject.transform.position, holdingDistance); // gameObject.transform.localScale.y + extraGrabRadius);
						foreach (Collider2D collider in hitColliders) {
								if (collider.gameObject != gameObject && collider.gameObject != carryingPlayer && collider.gameObject.CompareTag ("Player")) {		
										
										PlayerController playerController = collider.gameObject.GetComponent<PlayerController> ();
										if (!(playerController.jumpedFromPlayer == gameObject.GetInstanceID ())) {
												playerController.carryingPlayer = gameObject;
												carriedPlayer = collider.gameObject;
												
												
												jointToCarriedPlayer = gameObject.AddComponent ("DistanceJoint2D") as DistanceJoint2D;
												jointToCarriedPlayer.collideConnected = true;
												jointToCarriedPlayer.connectedBody = collider.gameObject.rigidbody2D;
												jointToCarriedPlayer.distance = holdingDistance;
												break;
										}
								}
						}
				} else if (carriedPlayer != null && !grabPressed) 
						DestroyConnection ();
		}
		
		public void PlayerLaunched ()
		{
				carriedPlayer = null;
				Destroy (jointToCarriedPlayer);
		}
	
		public void OuyaMenuButtonUp ()
		{
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
				if (jump) {
						rigidbody2D.AddForce (new Vector2 (0f, jumpForce));
			
						// Make sure the player can't jump again until the jump conditions from Update are satisfied.
						jump = false;
						jumping = true;
				}
		
				// Cache the horizontal input.
				if (grounded || (jumping && carryingPlayer == null)) {
						float h = GetHorizontalMovement ();
									
						// If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
						if (h * rigidbody2D.velocity.x < maxSpeed)
						// ... add a force to the player.
								rigidbody2D.AddForce (Vector2.right * h * moveForce);
	
						//		if (h > 0 && !facingRight)
						//				Flip ();
						//		else if (h < 0 && facingRight)
						//				Flip ();
				}
					
				// If the player's horizontal velocity is greater than the maxSpeed...
				if (Mathf.Abs (rigidbody2D.velocity.x) > maxSpeed)
			// ... set the player's velocity to the maxSpeed in the x axis.
						rigidbody2D.velocity = new Vector2 (Mathf.Sign (rigidbody2D.velocity.x) * maxSpeed, rigidbody2D.velocity.y);
		
		}
	
		bool JumpPressed ()
		{
				if (!keyboardDebugMode) {
						if (!splitController) {
								return GetButton (OuyaSDK.KeyEnum.BUTTON_O, index) || 
										GetButton (OuyaSDK.KeyEnum.BUTTON_RB, index) ||
										GetButton (OuyaSDK.KeyEnum.BUTTON_DPAD_UP, index) || 
										(GetAxis (OuyaSDK.KeyEnum.AXIS_LSTICK_Y, index) > debounceDistance);
						} else {
								if (leftSplit) {
										return GetButton (OuyaSDK.KeyEnum.BUTTON_LB, index) ||
												GetButton (OuyaSDK.KeyEnum.BUTTON_DPAD_UP, index) || 
												(GetAxis (OuyaSDK.KeyEnum.AXIS_LSTICK_Y, index) > debounceDistance);
								} else {
										return GetButton (OuyaSDK.KeyEnum.BUTTON_O, index) || 
												GetButton (OuyaSDK.KeyEnum.BUTTON_RB, index) ||
												(GetAxis (OuyaSDK.KeyEnum.AXIS_RSTICK_Y, index) > debounceDistance);
								}
						}
				} else if (index == OuyaSDK.OuyaPlayer.player1)   
						return Input.GetKey (KeyCode.W);
				else if (index == OuyaSDK.OuyaPlayer.player2)   
						return Input.GetKey (KeyCode.I);
				else
						return false;
		}
		
		bool GrabPressed ()
		{
				if (!keyboardDebugMode) {
						if (leftSplit)
								return GetButton (OuyaSDK.KeyEnum.BUTTON_LT, index);
						else
								return GetButton (OuyaSDK.KeyEnum.BUTTON_RT, index);
				} else if (index == OuyaSDK.OuyaPlayer.player2)   
						return Input.GetKey (";");
				else if (index == OuyaSDK.OuyaPlayer.player1)   
						return Input.GetKey (KeyCode.LeftShift);
				else
						return false;
	
		}
		
		float GetHorizontalMovement ()
		{
				float horizontal = 0.0f;
		

				if (!keyboardDebugMode) {
						if (!splitController || leftSplit)
								horizontal = GetAxis (OuyaSDK.KeyEnum.AXIS_LSTICK_X, index);
						else
								horizontal = GetAxis (OuyaSDK.KeyEnum.AXIS_LSTICK_X, index);
					
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
		
		bool PlayerIsGrounded ()
		{
				bool playerGrounded = false;
				int layerMask = 1 << LayerMask.NameToLayer ("Ground");
				layerMask |= 1 << LayerMask.NameToLayer ("Player");
				Vector3 startRaycast = transform.position;
				startRaycast.y -= transform.localScale.y / 1.99f;
				
				foreach (Transform groundCheck in groundChecks) {
						RaycastHit2D[] hits = Physics2D.LinecastAll (startRaycast, groundCheck.position, layerMask);
						foreach (RaycastHit2D raycastInfo in hits) 
								playerGrounded |= !(raycastInfo.collider.isTrigger || raycastInfo.collider.gameObject == gameObject);

				}
				
				return playerGrounded;
		}
	
		void Flip ()
		{
				// Switch the way the player is labelled as facing.
				facingRight = !facingRight;
	
				// Multiply the player's x local scale by -1.
				Vector3 theScale = transform.localScale;
				theScale.x *= -1;
				transform.localScale = theScale;
		}

}