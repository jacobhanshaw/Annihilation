using UnityEngine;
using System.Collections;

public class GameEvent : MonoBehaviour
{
		protected bool triggered;
		protected bool triggeredChanged;

		public virtual void Trigger (bool trigger)
		{
				if (triggered != trigger)
						triggeredChanged = true;
		
				triggered = trigger;
		}
}