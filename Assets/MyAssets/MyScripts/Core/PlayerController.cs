using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{		
		//Delegates
		public delegate void AchievementReceived (Achievement achievement,int newScore);
		public static AchievementReceived AchievementReceivedListeners; //check if null
	
		public delegate void HealthChanged (int newHealth);
		public static HealthChanged HealthChangedListeners; //check if null
	
		public delegate void PlayerDied ();
		public static PlayerDied PlayerDiedListeners; //check if null
	
		//INDEX in KeyCodes for Action - arbitrary
		public const int THROW_INDEX = 0;
		public const int JUMP_INDEX = 1;
		public const int LEFT_INDEX = 2;
		public const int RIGHT_INDEX = 3;
		public const int UP_BURST_INDEX = 4;
		public const int DOWN_BURST_INDEX = 5;
		public const int LEFT_BURST_INDEX = 6;
		public const int RIGHT_BURST_INDEX = 7;
		public const int DUL_BURST_INDEX = 8;
		public const int DUR_BURST_INDEX = 9;
		public const int DDL_BURST_INDEX = 10;
		public const int DDR_BURST_INDEX = 11;
		public const int ZAP_INDEX = 12;
		public const int RANDOMIZE_INDEX = 13;
		public const int X_SHOOT_INDEX = 14;
		public const int T_SHOOT_INDEX = 15;
		public const int DOUBLE_SPEED_INDEX = 16;
		public const int SLOW_MO_INDEX = 17;
		public const int SWORD_INDEX = 18;
		public const int FLY_INDEX = 19;
		public const int MINION_INDEX = 20;
		public const int AUTO_AIM_INDEX = 21;
		public const int PAUSE_INDEX = 22;
		public const int PUSH_INDEX = 23;
		public const int PULL_INDEX = 24;
		public const int SD_INDEX = 25;

		public KeyCode[] keycodes = { KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K,
	                              KeyCode.L, KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R, KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W,
		KeyCode.X, KeyCode.Y, KeyCode.Z };

		public float health = 100.0f;
		public float personalSpeedModifier = 1.0f;

		public bool enemy;
		public bool lockedPosition;
		
		//Debounce variables
		private bool jumpReleased;
		private float debounceY = 0.5f;

		//Spawn parameters
		public  Color spawnColor;
		public  GameObject spawnLocation;

		//Wall Jump
		private bool onWall;
		private bool wallJump;
		private bool wallJumpLeft;
		private Transform leftWallCheck;
		private Transform rightWallCheck;

		private float controlledAirSpeedForce = 5.0f;
		private float maxControlledAirSpeed = 2.0f;

		//Jump status variables
		private bool jumping = false;  
		private float jumpVelocity = 16.0f;
	
		//Movement variables
		private float maxSpeed = 6.0f;

		//Throw variables
		private float throwForceX = 100.0f;
		private float throwForceY = 50.0f;

		//Grounding variables
		private Transform[] groundChecks = new Transform[3];			// A position marking where to check if the player is grounded.
		private  bool grounded = false;			// Whether or not the player is grounded.
		
		//Achievement variables
		public int score;
		public  List<Achievement> achievements;
		
		[HideInInspector]
		public bool
				facingRight = true;			// For determining which way the player is currently facing.
	
		void Awake ()
		{

				leftWallCheck = transform.FindChild ("leftWallCheck");
				rightWallCheck = transform.FindChild ("rightWallCheck");
				groundChecks [0] = transform.FindChild ("groundChecka");
				groundChecks [1] = transform.FindChild ("groundCheckb");
				groundChecks [2] = transform.FindChild ("groundCheckc");
				
				achievements = new List<Achievement> ();
		}
	
		void Start ()
		{
				Input.ResetInputAxes ();
				gameObject.rigidbody2D.isKinematic = lockedPosition;
				PlayerDiedListeners += PlayerDiedEvent;

				if (enemy)
						health = 20.0f;
		}
	
		void OnDestroy ()
		{

		}
		
		void PlayerDiedEvent ()
		{
		}
		
		void Update ()
		{		
				grounded = PlayerIsGrounded ();
				onWall = PlayerIsOnWall ();

				if (!GameLogic.Instance.paused) {
						CheckForKeyboardPause ();				
						bool jumpPressed = JumpPressed ();
						jumpReleased |= !jumpPressed;
						jumping &= !jumpReleased;
		
						if (jumpPressed && (grounded || onWall) && jumpReleased && !jumping) {
								jumping = true;
								jumpReleased = false;
								wallJump = onWall;
						}
			 
				}
		}
		
		void FixedUpdate ()
		{
				Vector2 velocity = rigidbody2D.velocity;
				float h = GetHorizontalMovement ();
				
				if (h != 0.0f)
						facingRight = h > 0.0f;

				if (grounded)
						velocity.x = maxSpeed * h;
				else if (h * velocity.x < 0.0f || Mathf.Abs (velocity.x) < maxControlledAirSpeed)
						gameObject.rigidbody2D.AddForce (Vector2.right * (controlledAirSpeedForce * h));
		
				if (jumping) {
						if (wallJump) {
								float direction = wallJumpLeft ? 1.0f : -1.0f;
								velocity.x = 1.2f * maxSpeed * direction;
						} 
						velocity.y = jumpVelocity;
						jumping = false;
			
				}		
					
				rigidbody2D.velocity = velocity;
		}
		
		void RandomizeKeycodes ()
		{
				for (int i = 0; i < keycodes.Length; ++i) {
						int newIndex = Random.Range (0, keycodes.Length);
						KeyCode tmp = keycodes [i];
						keycodes [i] = keycodes [newIndex];
						keycodes [newIndex] = tmp;
				}
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
		
		void StopMovement (GameObject stopObject)
		{
				stopObject.rigidbody2D.velocity = Vector2.zero;
				stopObject.rigidbody2D.angularVelocity = 0.0f;
		}

		void ThrowPlayer ()
		{
				float fractionX = Mathf.Abs (gameObject.rigidbody2D.velocity.x / maxSpeed);
				float direction = gameObject.rigidbody2D.velocity.x < 0 ? -1 : 1;
				if (fractionX == 0)
						direction = 0;
				Vector2 forceVector = new Vector2 ((1.0f + fractionX) * throwForceX * direction, (2.0f - fractionX) * throwForceY);
				//	otherPlayer.rigidbody2D.AddForce (forceVector);
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
						PlayerDiedListeners ();
						KillPlayer ();
				} else if (other.gameObject.CompareTag ("Coin")) {
						Achievement newAchievement = other.gameObject.GetComponent<CoinScript> ().getAchievement ();
						achievements.Add (newAchievement);
						score += newAchievement.points;
						if (AchievementReceivedListeners != null)
								AchievementReceivedListeners (newAchievement, score);
						Destroy (other.gameObject);
				}
		}
		
		void KillPlayer ()
		{
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
		
		void CheckForKeyboardPause ()
		{
				if (Input.GetKeyUp (keycodes [PAUSE_INDEX])) {
						if (Time.timeScale != 0.0f)
								Time.timeScale = 0.0f;
						else
								Time.timeScale = 1.0f;
				} 
		}
		bool JumpPressed ()
		{
				return Input.GetKey (keycodes [JUMP_INDEX]);
		}
		
		float GetHorizontalMovement ()
		{

				if (Input.GetKey (keycodes [LEFT_INDEX]))
						return -1.0f;
				else if (Input.GetKey (keycodes [RIGHT_INDEX]))
						return 1.0f;

				return 0.0f;
		}
		
		bool PlayerIsGrounded ()
		{
				bool playerGrounded = false;
				Vector3 startRaycast = transform.position;
				startRaycast.y -= transform.localScale.y / 1.99f;

				int defaultLayer = 1 << LayerMask.NameToLayer ("Default");
		
				foreach (Transform groundCheck in groundChecks) {
						RaycastHit2D[] hits = Physics2D.LinecastAll (startRaycast, groundCheck.position, defaultLayer);
						foreach (RaycastHit2D raycastInfo in hits) {
								if (!(raycastInfo.collider.isTrigger || raycastInfo.collider.gameObject == gameObject)) 
										return true;		
								
						}
				}
				
				return false;
		}

		bool PlayerIsOnWall ()
		{
				int defaultLayer = 1 << LayerMask.NameToLayer ("Default");

				Vector3 startRaycastLeft = transform.position;
				startRaycastLeft.x -= transform.localScale.x / 1.99f;

				RaycastHit2D[] hits = Physics2D.LinecastAll (startRaycastLeft, leftWallCheck.position, defaultLayer);
				foreach (RaycastHit2D raycastInfo in hits) {
						if (!(raycastInfo.collider.isTrigger || raycastInfo.collider.gameObject == gameObject)) {
								wallJumpLeft = true;
								return true;		
						}
				}

				Vector3 startRaycastRight = transform.position;
				startRaycastRight.x += transform.localScale.x / 1.99f;
	
				hits = Physics2D.LinecastAll (startRaycastRight, rightWallCheck.position, defaultLayer);
				foreach (RaycastHit2D raycastInfo in hits) {
						if (!(raycastInfo.collider.isTrigger || raycastInfo.collider.gameObject == gameObject)) {
								wallJumpLeft = false;
								return true;		
						}

				}
		
				return false;
		}

		//Collision check
		bool CollisionIfGameObjectMovedAlongVector (GameObject movingObject, Vector3 vector)
		{
				int defaultLayer = 1 << LayerMask.NameToLayer ("Default");

				Collider2D[] colliders = Physics2D.OverlapAreaAll (BottomLeft (movingObject) + vector, 
		                                                   TopRight (movingObject) + vector, defaultLayer);
		
				foreach (Collider2D aCollider in colliders) {
						if (!aCollider.isTrigger && aCollider.gameObject != movingObject)
								return true;
				}
		
				return false;
		}
}