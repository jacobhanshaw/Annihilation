using UnityEngine;
using System.Collections;

public class StartTimerEvent : ItemsEvent
{	
		override public void Trigger (bool trigger)
		{
				if (!triggered && trigger) {
						triggered = true;
						for (int i = 0; i < items.Length; ++i) {
								if (items [i] != null)
										items [i].GetComponent<CoinScript> ().startTimer ();
						}
				}
		}

}
