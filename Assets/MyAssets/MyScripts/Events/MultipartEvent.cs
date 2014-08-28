using UnityEngine;
using System.Collections;

public class MultipartEvent : GameEvent
{

		private MultipartSwitch parentSwitch;

		void Start ()
		{
				parentSwitch = gameObject.transform.parent.gameObject.GetComponent<MultipartSwitch> ();
		}

		override public void Trigger (bool trigger)
		{
				parentSwitch.Trigger (trigger);
		}
}
