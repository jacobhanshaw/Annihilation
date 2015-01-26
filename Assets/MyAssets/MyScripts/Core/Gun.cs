using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
		private float speed = 50.0f;
		public Rigidbody2D bulletPrefab;

		private PlayerController playerController;
		//	private Animator anim;					// Reference to the Animator component.

		// Use this for initialization
		void Start ()
		{
				playerController = transform.root.GetComponent<PlayerController> ();
				//		anim = transform.root.gameObject.GetComponent<Animator> ();
		}
	
		// Update is called once per frame
		void Update ()
		{
				/*
				if (Input.GetKeyUp (playerController.keycodes [PlayerController.T_SHOOT_INDEX])) {
						Rigidbody2D bulletInstance = Instantiate (bulletPrefab, transform.position, Quaternion.Euler (new Vector3 (0, 0, 0))) as Rigidbody2D;
						bulletInstance.velocity = new Vector2 (speed, 0);
						bulletInstance = Instantiate (bulletPrefab, transform.position, Quaternion.Euler (new Vector3 (0, 0, 180))) as Rigidbody2D;
						bulletInstance.velocity = new Vector2 (-speed, 0);
						bulletInstance = Instantiate (bulletPrefab, transform.position, Quaternion.Euler (new Vector3 (0, 0, 90))) as Rigidbody2D;
						bulletInstance.velocity = new Vector2 (0, speed);
						bulletInstance = Instantiate (bulletPrefab, transform.position, Quaternion.Euler (new Vector3 (0, 0, 270))) as Rigidbody2D;
						bulletInstance.velocity = new Vector2 (0, -speed);
				} else if (Input.GetKeyUp (playerController.keycodes [PlayerController.X_SHOOT_INDEX])) {
						Rigidbody2D bulletInstance = Instantiate (bulletPrefab, transform.position, Quaternion.Euler (new Vector3 (0, 0, 45))) as Rigidbody2D;
						bulletInstance.velocity = new Vector2 (speed, speed);
						bulletInstance = Instantiate (bulletPrefab, transform.position, Quaternion.Euler (new Vector3 (0, 0, 135))) as Rigidbody2D;
						bulletInstance.velocity = new Vector2 (-speed, speed);
						bulletInstance = Instantiate (bulletPrefab, transform.position, Quaternion.Euler (new Vector3 (0, 0, 225))) as Rigidbody2D;
						bulletInstance.velocity = new Vector2 (-speed, -speed);
						bulletInstance = Instantiate (bulletPrefab, transform.position, Quaternion.Euler (new Vector3 (0, 0, 315))) as Rigidbody2D;
						bulletInstance.velocity = new Vector2 (speed, -speed);
				}
				*/
		}
		
		public void playerFire (bool facingRight, bool[] moveDirection, bool[] attackMods)
		{
				int numMissles = 1;
				float rotation = 0.0f;
				float rotationStep = 90.0f;
				float speedX = speed;
				float speedY = speed;

				if (attackMods [(int)Controller.AttackModIndex.TMod] && attackMods [(int)Controller.AttackModIndex.XMod]) {
						numMissles = 8;
						rotation = 45.0f;
						rotationStep = 45.0f;
				} else if (attackMods [(int)Controller.AttackModIndex.TMod]) {
						numMissles = 4;
				} else if (attackMods [(int)Controller.AttackModIndex.XMod]) {
						numMissles = 4;
						rotation = 45.0f;
				} else if (moveDirection [(int)Controller.MotionIndex.Left] || !facingRight)
						rotation = 180.0f;

				for (int i = 0; i < numMissles; ++i) {
						Vector2 velocity = new Vector2 (speedX * Mathf.Cos (rotation * Mathf.Deg2Rad), speedY * Mathf.Sin (rotation * Mathf.Deg2Rad));
						fire (Quaternion.Euler (new Vector3 (0, 0, rotation)), velocity, attackMods);
						rotation += rotationStep;
				}
		}

		public void fire (Quaternion rotation, Vector2 velocity, bool[] attackMods)
		{
				Rigidbody2D bulletInstance = Instantiate (bulletPrefab, transform.position, rotation) as Rigidbody2D;
				bulletInstance.velocity = velocity;
		}
}
