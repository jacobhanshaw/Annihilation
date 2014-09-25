using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemsEvent : GameEvent
{
		public bool       inverted;
		public bool       ignorePause;
	
		public string      itemName;
		private List<GameObject> itemsList;
		protected GameObject[] items;
		protected bool triggered;

		public void Start ()
		{
				Debug.Log ("ItemEventCalled");

				itemsList = new List<GameObject> ();
				GameObject item = HelperFunction.Instance.FindBasedOnLayer (itemName, gameObject.layer, inverted);
				if (item != null)
						itemsList.Add (item);

				int index = 0;
				while (item = HelperFunction.Instance.FindBasedOnLayer (itemName + (index++), gameObject.layer, inverted))
						itemsList.Add (item);

				items = itemsList.ToArray ();
		} 

		override public void Trigger (bool trigger)
		{
				triggered = trigger;
		}
}
