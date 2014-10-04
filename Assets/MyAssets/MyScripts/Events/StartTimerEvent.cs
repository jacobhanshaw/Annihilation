using UnityEngine;
using System.Collections;

public class StartTimerEvent : ItemEvent
{	

		public override void Trigger (bool trigger)
		{
				if (!triggered && trigger) {
						triggered = true;
						if (item != null)
								item.GetComponent<CoinScript> ().startTimer ();
				}
		}

}
