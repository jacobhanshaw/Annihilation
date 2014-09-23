using UnityEngine;
using System.Collections;

public class ShowButtonActive : GameEvent
{
		public bool       inverted;
		public string     	  shownSwitchName;
		private SwitchScript  shownSwitchScript;
	
		void Start ()
		{
				shownSwitchScript = HelperFunction.Instance.FindBasedOnLayer (shownSwitchName, gameObject.layer, inverted).GetComponent<SwitchScript> ();
		}
	
		override public void Trigger (bool trigger)
		{
				shownSwitchScript.ShowButtonAsActivated (trigger);
		}
}
