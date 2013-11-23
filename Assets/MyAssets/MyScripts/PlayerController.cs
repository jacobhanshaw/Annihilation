using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour,
OuyaSDK.IJoystickCalibrationListener,
OuyaSDK.IPauseListener, OuyaSDK.IResumeListener,
OuyaSDK.IMenuButtonUpListener,
OuyaSDK.IMenuAppearingListener
{
	
		public static OuyaSDK.OuyaPlayer Index = OuyaSDK.OuyaPlayer.player1;
	
	#region New Input API - in progress
	
		private bool m_useSDKForInput = false;
	
	#endregion
	
		[HideInInspector]
		public bool
				facingRight = true;			// For determining which way the player is currently facing.
		[HideInInspector]
		public bool
				jump = false;				// Condition for whether the player should jump.	
	
		public float moveForce = 365f;			// Amount of force added to move the player left and right.
		public float maxSpeed = 5f;				// The fastest the player can travel in the x axis.
		public float jumpForce = 1000f;			// Amount of force added when the player jumps.

		private Transform groundCheck;			// A position marking where to check if the player is grounded.
		private bool grounded = false;			// Whether or not the player is grounded.
	
	
		void Awake ()
		{
				OuyaSDK.registerJoystickCalibrationListener (this);
				OuyaSDK.registerMenuButtonUpListener (this);
				OuyaSDK.registerMenuAppearingListener (this);
				OuyaSDK.registerPauseListener (this);
				OuyaSDK.registerResumeListener (this);
		
				groundCheck = transform.Find ("groundCheck");
		}
	
		void OnDestroy ()
		{
				OuyaSDK.unregisterJoystickCalibrationListener (this);
				OuyaSDK.unregisterMenuButtonUpListener (this);
				OuyaSDK.unregisterMenuAppearingListener (this);
				OuyaSDK.unregisterPauseListener (this);
				OuyaSDK.unregisterResumeListener (this);
		}

		// Use this for initialization
		void Start ()
		{
				Input.ResetInputAxes ();
		}
	
		// Update is called once per frame
		void Update ()
		{
				//GetAxis(OuyaSDK.KeyEnum.AXIS_LSTICK_Y, Index) GetAxis(OuyaSDK.KeyEnum.AXIS_LSTICK_X, Index)
				//GetAxis(OuyaSDK.KeyEnum.AXIS_RSTICK_Y, Index) GetAxis(OuyaSDK.KeyEnum.AXIS_RSTICK_X, Index)
	
				// The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
				grounded = Physics2D.Linecast (transform.position, groundCheck.position, 1 << LayerMask.NameToLayer ("Ground"));  
		
				// If the jump button is pressed and the player is grounded then the player should jump.
				if (GetButton (OuyaSDK.KeyEnum.BUTTON_O, Index) && grounded)
						jump = true;
	
		
				if (Input.GetButtonDown ("Jump"))
						Debug.Log ("Jump");
				if (GetButton (OuyaSDK.KeyEnum.BUTTON_L3, Index))
						Debug.Log ("L3");
				if (GetButton (OuyaSDK.KeyEnum.BUTTON_R3, Index))
						Debug.Log ("R3");
				if (GetButton (OuyaSDK.KeyEnum.BUTTON_O, Index)) {
						Debug.Log ("O");

				} 
				if (GetButton (OuyaSDK.KeyEnum.BUTTON_U, Index)) {
						Debug.Log ("U");
						Vector3 position = gameObject.transform.position;
						position.y -= 5;
						gameObject.transform.position = position;
				}
				if (GetButton (OuyaSDK.KeyEnum.BUTTON_Y, Index))
						Debug.Log ("Y");
				if (GetButton (OuyaSDK.KeyEnum.BUTTON_A, Index)) {
						Debug.Log ("A");
						Vector3 position = gameObject.transform.position;
						position.y += 5;
						gameObject.transform.position = position;	
				}
				if (GetButton (OuyaSDK.KeyEnum.BUTTON_LB, Index))
						Debug.Log ("LB");
				if (GetButton (OuyaSDK.KeyEnum.BUTTON_RB, Index))
						Debug.Log ("RB");
				if (GetButton (OuyaSDK.KeyEnum.BUTTON_LT, Index))
						Debug.Log ("LT");
				if (GetButton (OuyaSDK.KeyEnum.BUTTON_RT, Index))
						Debug.Log ("RT");
				if (GetButton (OuyaSDK.KeyEnum.BUTTON_DPAD_UP, Index))
						Debug.Log ("D_UP");
				if (GetButton (OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN, Index))
						Debug.Log ("D_DOWN");
				if (GetButton (OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT, Index))
						Debug.Log ("D_LEFT");
				if (GetButton (OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT, Index))
						Debug.Log ("D_RIGHT");
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
				// Cache the horizontal input.
				float h = GetAxis (OuyaSDK.KeyEnum.AXIS_LSTICK_X, Index);	

	
				// If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
				if (h * rigidbody2D.velocity.x < maxSpeed)
		// ... add a force to the player.
						rigidbody2D.AddForce (Vector2.right * h * moveForce);
	
				// If the player's horizontal velocity is greater than the maxSpeed...
				if (Mathf.Abs (rigidbody2D.velocity.x) > maxSpeed)
		// ... set the player's velocity to the maxSpeed in the x axis.
						rigidbody2D.velocity = new Vector2 (Mathf.Sign (rigidbody2D.velocity.x) * maxSpeed, rigidbody2D.velocity.y);
	
				if (h > 0 && !facingRight)
						Flip ();
				else if (h < 0 && facingRight)
						Flip ();
	
				// If the player should jump...
				if (jump) {
						// Add a vertical force to the player.
						rigidbody2D.AddForce (new Vector2 (0f, jumpForce));
		
						// Make sure the player can't jump again until the jump conditions from Update are satisfied.
						jump = false;
				}
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