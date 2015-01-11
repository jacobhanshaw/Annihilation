using UnityEngine;
using System.Collections;

public class DestroyEvent : GameEvent
{

		public GameObject item;
	
		public override void Trigger (bool trigger)
		{
				if (trigger)
						Destroy (item);
		}
}
