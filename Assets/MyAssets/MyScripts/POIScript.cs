using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class POIScript : MonoBehaviour
{

		[HideInInspector]
		public bool
				shown;
		[HideInInspector]
		public bool
				activeScript;
	
		public bool  forcePan;
		public bool  panOnEnter;
		public float overridePanSpeed = -1.0f;
		public float overridePanZoomLevel = -1.0f;
		public bool  ignoreCollider;
		public float relevantDuration;
		
		private Vector2 bottomLeft;
		private Vector2 topRight;

		void Start ()
		{
				Vector2 position2d = gameObject.transform.position;
				if (!ignoreCollider) {
						bottomLeft = position2d + ((BoxCollider2D)gameObject.collider2D).center - ((BoxCollider2D)gameObject.collider2D).size / 2.0f;
						topRight = position2d + ((BoxCollider2D)gameObject.collider2D).center + ((BoxCollider2D)gameObject.collider2D).size / 2.0f;
				}
		}

		void Update ()
		{
				//Naive polling approach, but OnTriggerExit wasn't getting called properly. Seems to be a known Unity bug
				int layerMask = 1 << LayerMask.NameToLayer ("Player");
				
				if (!ignoreCollider) {
						Collider2D[] colliders = Physics2D.OverlapAreaAll (bottomLeft, 
		                                                   topRight, layerMask);

						activeScript = colliders.Length > 0;
				}
				
				if (shown && relevantDuration != -1.0f && relevantDuration != 0) {
						relevantDuration -= Time.deltaTime;
						if (relevantDuration < 0)
								Destroy (gameObject);
				}
		}
		
}
