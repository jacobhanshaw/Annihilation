using UnityEngine;
using System.Collections;

public class ShowButtonActive : GameEvent
{
		public GameObject item;
		private SwitchScript switchScript;

		new void Start ()
		{
				switchScript = item.GetComponent<SwitchScript> ();
		}
	
		public override void Trigger (bool trigger)
		{
				switchScript.ShowButtonAsActivated (trigger);
		}
}
