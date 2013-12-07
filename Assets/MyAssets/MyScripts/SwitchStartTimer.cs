using UnityEngine;
using System.Collections;

public class SwitchStartTimer : MonoBehaviour
{

		public GameObject timedObject;
	
		private bool triggered;
	
		void OnTriggerEnter2D (Collider2D other)
		{
				if (!triggered) {
						triggered = true;
						if (timedObject != null)
								timedObject.GetComponent<CoinScript> ().startTimer ();
				}

		}
}
