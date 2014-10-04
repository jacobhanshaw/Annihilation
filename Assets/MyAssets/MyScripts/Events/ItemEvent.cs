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
				item = HelperFunction.Instance.FindBasedOnLayer (itemName, gameObject.layer, inverted);
		} 

}
