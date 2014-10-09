using UnityEngine;
using System.Collections;

public class DestroyEvent : ItemEvent
{
	
		public override void Trigger (bool trigger)
		{
				if (trigger)
						Destroy (item);
		}
}
