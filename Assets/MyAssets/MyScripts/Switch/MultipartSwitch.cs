using UnityEngine;
using System.Collections;

public class MultipartSwitch : MonoBehaviour
{
		public bool oneTimeUse;
		public int numSwitchesToTrigger;
		private bool wasTriggered;
		
		private GameEvent[] gameEvents;
	
		void Start ()
		{
				gameEvents = transform.GetComponents<GameEvent> ();
		}
	
		public void Trigger (bool trigger)
		{
				if (trigger) {
						--numSwitchesToTrigger;
						if (numSwitchesToTrigger == 0 && !wasTriggered) {
								wasTriggered = true;
								foreach (GameEvent gameEvent in gameEvents)
										gameEvent.Trigger (wasTriggered);
						}	
				} else {
						++numSwitchesToTrigger;
						if (!oneTimeUse) {
								if (numSwitchesToTrigger == 1 && wasTriggered) {
										wasTriggered = false;
										foreach (GameEvent gameEvent in gameEvents)
												gameEvent.Trigger (wasTriggered);
					
								}
						}
				}
		}
}
