using UnityEngine;
using System.Collections;

public class StartTimerEvent : GameEvent
{
		public string timedObjectName;
		private GameObject timedObject;
	
		private bool triggered;
		
		void Start ()
		{
				GameObject potentialItem = GameObject.Find (timedObjectName);
				if (gameObject.layer == potentialItem.layer)
						timedObject = potentialItem;
				else
						timedObject = GameObject.Find (timedObjectName + "(Clone)");
		}

		override public void Trigger (bool trigger)
		{
				if (!triggered && trigger) {
						triggered = true;
						if (timedObject != null)
								timedObject.GetComponent<CoinScript> ().startTimer ();
				}
		}

}
