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
				GameObject potentialItem = GameObject.Find (poiScriptObjectName);
				if (inverted && gameObject.layer == potentialItem.layer)
						poiScript = GameObject.Find (poiScriptObjectName.Replace ("(Clone)", "")).GetComponent<POIScript> ();
				else if (gameObject.layer == potentialItem.layer || potentialItem.layer == LayerMask.NameToLayer ("Default"))
						poiScript = potentialItem.GetComponent<POIScript> ();
				else
						poiScript = GameObject.Find (poiScriptObjectName + "(Clone)").GetComponent<POIScript> ();
		}

		override public void Trigger (bool trigger)
		{
				poiScript.forcePan = true;
				poiScript.overridePanSpeed = overridePanSpeed;
				poiScript.overridePanZoomLevel = overridePanZoomLevel;
		}
}
