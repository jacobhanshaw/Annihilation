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
				
		public int   priorityIndex;
	
		public bool  forcePan;
		public bool  panOnEnter;
		public float overridePanSpeed = -1.0f;
		public float overridePanZoomLevel = -1.0f;
		public bool  ignoreCollider;
		public float relevantDuration;
		
		private Vector2 bottomLeft;
		private Vector2 topRight;
		
		private int layerMask = 0;

		void Start ()
		{
				Vector2 position2d = gameObject.transform.position;
				if (!ignoreCollider) {
						bottomLeft = HelperFunction.Instance.BottomLeftOfBoxCollider2D (position2d, ((BoxCollider2D)gameObject.collider2D));
						topRight = HelperFunction.Instance.TopRightOfBoxCollider2D (position2d, ((BoxCollider2D)gameObject.collider2D));
				}
				
				string layer = LayerMask.LayerToName (gameObject.layer);
				int firstNum = HelperFunction.Instance.PlayersInLayer (gameObject.layer, 1);
				if (firstNum != -1)
						layerMask |= 1 << LayerMask.NameToLayer ("Player" + firstNum);
				int secondNum;
				secondNum = HelperFunction.Instance.PlayersInLayer (gameObject.layer, 2);
				if (secondNum != -1)
						layerMask |= 1 << LayerMask.NameToLayer ("Player" + secondNum);
		}

		void Update ()
		{
				//Naive polling approach, but OnTriggerExit wasn't getting called properly. Seems to be a known Unity bug
				
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
