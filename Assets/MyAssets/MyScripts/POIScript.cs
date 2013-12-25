using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class POIScript : MonoBehaviour
{
		public bool panOnStart;
		public bool panToItem;

		public bool  shown;
		public bool  activeScript;
		public float relevantDuration;
		
		private Vector2 bottomLeft;
		private Vector2 topRight;
		
//		private float moveAmount = -0.01f;
// 		Can't do this, because parenting switching causes a re-count
//		private int playersInTrigger = 0;
//		private List<int> playersInTrigger;

		void Start ()
		{
				//playersInTrigger = new List<int> ();
				Vector2 position2d = gameObject.transform.position;
				bottomLeft = position2d + ((BoxCollider2D)gameObject.collider2D).center - ((BoxCollider2D)gameObject.collider2D).size / 2.0f;
				topRight = position2d + ((BoxCollider2D)gameObject.collider2D).center + ((BoxCollider2D)gameObject.collider2D).size / 2.0f;
		}

		void Update ()
		{
				int layerMask = 1 << LayerMask.NameToLayer ("Player");
				
				Collider2D[] colliders = Physics2D.OverlapAreaAll (bottomLeft, 
		                                                   topRight, layerMask);

				activeScript = colliders.Length > 0;
		
				if (shown && relevantDuration != -1.0f && relevantDuration != 0) {
						relevantDuration -= Time.deltaTime;
						if (relevantDuration < 0)
								Destroy (gameObject);
				}
		}
		
		/*
		void OnTriggerEnter2D (Collider2D other)
		{
				if (other.CompareTag ("Player")) {
						if (!playersInTrigger.Contains (other.gameObject.GetInstanceID ()))
								playersInTrigger.Add (other.gameObject.GetInstanceID ());
						Debug.Log ("Total: " + playersInTrigger + " Increment by: " + other.gameObject.ToString () + " id: " + other.gameObject.GetInstanceID ());
						activeScript = true;
				}
		}
		
		void OnTriggerStay2D (Collider2D other)
		{
				gameObject.transform.position = new Vector3 (gameObject.transform.position.x, gameObject.transform.position.y + moveAmount, gameObject.transform.position.z);
				moveAmount *= -1.0f;
				Debug.Log ("Total: " + playersInTrigger + " Stay by: " + other.gameObject.ToString () + " id: " + other.gameObject.GetInstanceID () + " with parent: " + other.gameObject.transform.parent);
		}
		
		void OnTriggerExit2D (Collider2D other)
		{
				if (other.CompareTag ("Player")) {
						playersInTrigger.Remove (other.gameObject.GetInstanceID ());
						Debug.Log ("Total: " + playersInTrigger + " Decrement by: " + other.gameObject.ToString () + " id: " + other.gameObject.GetInstanceID ());
						if (playersInTrigger.Count == 0) 
								activeScript = false;
				}
		}
	*/
}
