using UnityEngine;
using System.Collections;

public class PanEvent : GameEvent
{
		public GameObject item;

		public float      overridePanSpeed = -1.0f;
		public float      overridePanZoomLevel = -1.0f;
		
		private POIScript poiScript;
		
		new void Start ()
		{
				poiScript = item.GetComponent<POIScript> ();
		}

		public override void Trigger (bool trigger)
		{
				poiScript.forcePan = true;
		}
}
