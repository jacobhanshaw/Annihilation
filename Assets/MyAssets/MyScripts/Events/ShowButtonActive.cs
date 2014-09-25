using UnityEngine;
using System.Collections;

public class ShowButtonActive : ItemsEvent
{
		private SwitchScript[] switchScripts;

		new void Start ()
		{
				base.Start ();

				switchScripts = new SwitchScript[items.Length];
				for (int i = 0; i < items.Length; ++i)
						switchScripts [i] = items [i].GetComponent<SwitchScript> ();
		}
	
		override public void Trigger (bool trigger)
		{
				foreach (SwitchScript switchScript in switchScripts)
						switchScript.ShowButtonAsActivated (trigger);
		}
}
