using UnityEngine;
using System.Collections;

public class ShowButtonActive : GameEvent
{

		public bool      	  inverted;
		public string     	  shownSwitchName;
		private SwitchScript  shownSwitchScript;
	
		void Start ()
		{
				GameObject potentialItem = GameObject.Find (shownSwitchName);
				if (inverted && gameObject.layer == potentialItem.layer)
						shownSwitchScript = GameObject.Find (shownSwitchName.Replace ("(Clone)", "")).GetComponent<SwitchScript> ();
				if (gameObject.layer == potentialItem.layer || potentialItem.layer == LayerMask.NameToLayer ("Default") || inverted)
						shownSwitchScript = potentialItem.GetComponent<SwitchScript> ();
				else
						shownSwitchScript = GameObject.Find (shownSwitchName + "(Clone)").GetComponent<SwitchScript> ();
		}
	
		override public void Trigger (bool trigger)
		{
				shownSwitchScript.ShowButtonAsActivated (trigger);
		}
}
