using UnityEngine;
using System.Collections;

public class StartTimerEvent : GameEvent
{
		public bool   inverted;
		public string timedObjectName;
		private GameObject timedObject;
	
		private bool triggered;
		
		void Start ()
		{
				timedObject = HelperFunction.Instance.FindBasedOnLayer (timedObjectName, gameObject.layer, inverted);
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
