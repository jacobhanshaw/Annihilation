using UnityEngine;
using System.Collections;

public class PanEvent : ItemsEvent
{

		public float[]      overridePanSpeed;
		public float[]      overridePanZoomLevel;
		
		private POIScript[] poiScripts;
		
		void Start ()
		{
				poiScripts = new POIScript[items.Length];
				for (int i = 0; i < items.Length; ++i)
						poiScripts [i] = items [i].GetComponent<POIScript> ();
		}

		override public void Trigger (bool trigger)
		{
				for (int i = 0; i < poiScripts.Length; ++i) {
						poiScripts [i].forcePan = true;
						if (overridePanSpeed.Length == 0)
								poiScripts [i].overridePanSpeed = -1;
						else if (overridePanSpeed.Length == 1)
								poiScripts [i].overridePanSpeed = overridePanSpeed [0];
						else
								poiScripts [i].overridePanSpeed = overridePanSpeed [i];

						if (overridePanZoomLevel.Length == 0)
								poiScripts [i].overridePanZoomLevel = -1;
						else if (overridePanZoomLevel.Length == 1)
								poiScripts [i].overridePanZoomLevel = overridePanZoomLevel [0];
						else
								poiScripts [i].overridePanZoomLevel = overridePanZoomLevel [i];
				}
		}
}
