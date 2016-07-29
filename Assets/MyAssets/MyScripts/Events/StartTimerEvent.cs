using UnityEngine;
using System.Collections;

public class StartTimerEvent : GameEvent
{	
		public GameObject item;

		public override void Trigger (bool trigger)
		{
				if (!triggered && trigger) {
						triggered = true;
						//		if (item != null)
						//		item.GetComponent<CoinScript> ().startTimer ();
				}
		}

}
