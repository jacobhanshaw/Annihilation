using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemEvent : GameEvent
{
		public bool       inverted;
		public bool       ignorePause;
	
		public string      itemName;
		protected GameObject item;

		public void Start ()
		{
				findItem ();
		} 

		public bool setNewItemName (string name)
		{
				itemName = name;
				return findItem ();
		}

		private bool findItem ()
		{
				int layer = 1 << gameObject.layer;
				layer |= 1 << LayerMask.NameToLayer ("Interact" + HelperFunction.Instance.PlayersInLayer (gameObject.layer, 1));
				layer |= 1 << LayerMask.NameToLayer ("Interact" + HelperFunction.Instance.PlayersInLayer (gameObject.layer, 2));
				item = HelperFunction.Instance.FindBasedOnLayer (itemName, layer, inverted);

				return item != null;
		}

}
