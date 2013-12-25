using UnityEngine;
using System.Collections;

public class PanEvent : GameEvent
{
		public POIScript poiScript;
		public float     overridePanSpeed = -1.0f;
		public float     overridePanZoomLevel = -1.0f;

		override public void Trigger (bool trigger)
		{
				poiScript.forcePan = true;
				poiScript.overridePanSpeed = overridePanSpeed;
				poiScript.overridePanZoomLevel = overridePanZoomLevel;
		}
}
