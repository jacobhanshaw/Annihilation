using UnityEngine;
using System.Collections;

public class MultipartEvent : GameEvent
{
		private MultipartSwitch parentSwitch;

		void Start ()
		{
				parentSwitch = gameObject.transform.parent.gameObject.GetComponent<MultipartSwitch> ();
		}

		public override void Trigger (bool trigger)
		{
				parentSwitch.Trigger (trigger);
		}
}
