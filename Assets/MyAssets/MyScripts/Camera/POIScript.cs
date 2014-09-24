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
		
		private int playersInTrigger;

		void Update ()
		{
				activeScript = playersInTrigger > 0;
				
				if (shown && relevantDuration != -1.0f && relevantDuration != 0) {
						relevantDuration -= Time.deltaTime;
						if (relevantDuration < 0)
								Destroy (gameObject);
				}
		}

		void OnTriggerEnter2D (Collider2D other)
		{
				if (other.gameObject.CompareTag ("Player")) 
						++playersInTrigger;
		}
	
		void OnTriggerExit2D (Collider2D other)
		{
				if (other.gameObject.CompareTag ("Player")) 
						--playersInTrigger;
		}
		
}
