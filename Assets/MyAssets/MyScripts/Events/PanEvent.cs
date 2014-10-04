using UnityEngine;
using System.Collections;

public class PanEvent : ItemEvent
{
		public float      overridePanSpeed = -1.0f;
		public float      overridePanZoomLevel = -1.0f;
		
		private POIScript poiScript;
		
		new void Start ()
		{
				base.Start ();

				poiScript = item.GetComponent<POIScript> ();
		}

		public override void Trigger (bool trigger)
		{
				poiScript.forcePan = true;
		}
}
