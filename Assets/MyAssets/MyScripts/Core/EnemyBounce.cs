using UnityEngine;
using System.Collections;

public class EnemyBounce : MonoBehaviour
{
		public const float BOUNCE_VELOCITY = 16.0f;

		public delegate void KillDelegate ();
		public KillDelegate killDelegate;

		void OnTriggerEnter2D (Collider2D other)
		{
				if (gameObjectShouldBounce (other.gameObject))
						other.gameObject.transform.rigidbody2D.velocity = new Vector2 (other.gameObject.transform.rigidbody2D.velocity.x, BOUNCE_VELOCITY);
				if (gameObjectShouldDie (other.gameObject))
						killDelegate ();
		}

		bool gameObjectShouldBounce (GameObject otherObject)
		{
				return otherObject.tag == "Player";
		}

		bool gameObjectShouldDie (GameObject otherObject)
		{
				return otherObject.tag == "Player";
		}
		
}
