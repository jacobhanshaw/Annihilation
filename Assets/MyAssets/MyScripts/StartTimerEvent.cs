using UnityEngine;
using System.Collections;

public class StartTimerEvent : GameEvent
{
		public GameObject timedObject;
	
		private bool triggered;

		override public void Trigger (bool trigger)
		{
				if (!triggered && trigger) {
						triggered = true;
						if (timedObject != null)
								timedObject.GetComponent<CoinScript> ().startTimer ();
				}
		}

}
