using UnityEngine;
using System.Collections;

public class PanEvent : GameEvent
{
		public bool       inverted;
		public string     poiScriptObjectName;
		public float      overridePanSpeed = -1.0f;
		public float      overridePanZoomLevel = -1.0f;
		
		private POIScript poiScript;
		
		void Start ()
		{
				poiScript = HelperFunction.Instance.FindBasedOnLayer (poiScriptObjectName, gameObject.layer, inverted).GetComponent<POIScript> ();
		}

		override public void Trigger (bool trigger)
		{
				poiScript.forcePan = true;
				poiScript.overridePanSpeed = overridePanSpeed;
				poiScript.overridePanZoomLevel = overridePanZoomLevel;
		}
}
