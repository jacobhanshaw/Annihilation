using UnityEngine;
using System.Collections;

public class ShowButtonActive : ItemEvent
{
		private SwitchScript switchScript;

		new void Start ()
		{
				base.Start ();

				switchScript = item.GetComponent<SwitchScript> ();
		}
	
		public override void Trigger (bool trigger)
		{
				switchScript.ShowButtonAsActivated (trigger);
		}
}
