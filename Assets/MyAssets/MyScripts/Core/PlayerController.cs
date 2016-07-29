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

		[HideInInspector]
		public float
				health = 100.0f;
		public float personalSpeedModifier = 1.0f;
	
		//Debounce variables
		private bool flying;
		private const float FLYING_SPEED_MULT = 2.5f;
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

		//Buttons pressed
		private bool[] movesArray;
		private Gun gunComponent;
	
		void Awake ()
		{

				leftWallCheck = transform.FindChild ("leftWallCheck");
				rightWallCheck = transform.FindChild ("rightWallCheck");
				groundChecks [0] = transform.FindChild ("groundChecka");
				groundChecks [1] = transform.FindChild ("groundCheckb");
				groundChecks [2] = transform.FindChild ("groundCheckc");
				
				achievements = new List<Achievement> ();

				movesArray = new bool[]{false,false,false,false,false};
		}
	
		void Start ()
		{
				PlayerDiedListeners += PlayerDiedEvent;
				Controller.MoveListeners += UpdateMovesArray;
				gunComponent = gameObject.GetComponent<Gun> ();
				Controller.ShootListeners += shootGun;
				Controller.SizeUpListeners += changeScale;
				Controller.SizeDownListeners += changeScale;
				Controller.FlyListeners += flyChanged;
		}
	
		void OnDestroy ()
		{
				PlayerDiedListeners += PlayerDiedEvent;
				Controller.MoveListeners -= UpdateMovesArray;
				Controller.SizeUpListeners -= changeScale;
				Controller.SizeDownListeners -= changeScale;
				Controller.FlyListeners -= flyChanged;
		}
		
		void PlayerDiedEvent ()
		{
		}

		void changeScale (float newScale)
		{
				gameObject.transform.localScale = new Vector3 (newScale, newScale, 1.0f);
		}

		void flyChanged ()
		{
				flying = !flying;
				gameObject.rigidbody2D.gravityScale = flying ? 0.0f : 1.0f;
		}

		void shootGun (bool[] moveDirection, bool[] attackMods)
		{
				gunComponent.playerFire (facingRight, moveDirection, attackMods, gameObject.transform.localScale.x);
		}
		
		void Update ()
		{		
				grounded = PlayerIsGrounded ();
				onWall = PlayerIsOnWall ();

				if (!GameLogic.Instance.paused) {
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
				float h = GetHorizontalMovement ();
				
				if (h != 0.0f)
						facingRight = h > 0.0f;
				if (flying) {
						StopMovement (gameObject);
						Vector3 movement = new Vector3 (h * FLYING_SPEED_MULT, GetVerticalMovement () * FLYING_SPEED_MULT, 0.0f);
						//	while (CollisionIfGameObjectMovedAlongVector (gameObject, movement) && (movement.x > 0.01f || movement.y > 0.01f))
						//			movement *= 0.5f;
						//s	if (!CollisionIfGameObjectMovedAlongVector (gameObject, movement))
						gameObject.transform.position += movement * Time.fixedDeltaTime;
				} else {
						Vector2 velocity = rigidbody2D.velocity;

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
				} else if (other.gameObject.CompareTag ("Deadly") || other.gameObject.CompareTag ("Enemy")) {
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
		
		bool JumpPressed ()
		{
				return movesArray [(int)Controller.MotionIndex.Jump];
		}

		float GetVerticalMovement ()
		{
				float direction = 0.0f;
				if (movesArray [(int)Controller.MotionIndex.Jump])
						direction += 1.0f;
				if (movesArray [(int)Controller.MotionIndex.Down])
						direction += -1.0f;
		
				return direction;
		}	

		float GetHorizontalMovement ()
		{
				float direction = 0.0f;
				if (movesArray [(int)Controller.MotionIndex.Left])
						direction += -1.0f;
				if (movesArray [(int)Controller.MotionIndex.Right])
						direction += 1.0f;

				return direction;
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

		//Box corners
		Vector3 TopRight (GameObject box)
		{
				return box.transform.position + new Vector3 (box.transform.localScale.x / 2.0f, box.transform.localScale.y / 2.0f, 0);
		}
	
		Vector3 BottomLeft (GameObject box)
		{
				return box.transform.position - new Vector3 (box.transform.localScale.x / 2.0f, box.transform.localScale.y / 2.05f, 0);
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

		public void UpdateMovesArray (bool[] moves)
		{
				movesArray = moves;
		}
}