using UnityEngine;
using System.Collections;

public class RigidBody2DToggleEvent : GameEvent
{
		public GameObject item;

		public bool disable;
		private Rigidbody2D rigidBody2d;

		new void Start ()
		{
				rigidBody2d = item.GetComponent<Rigidbody2D> ();
		}

		public override void Trigger (bool trigger)
		{

				if (trigger)
						rigidBody2d.isKinematic = disable;
				else 
						rigidBody2d.isKinematic = disable;
		}
}
