using UnityEngine;
using System.Collections;

public class MoveEvent : GameEvent
{

		public GameObject movedItem;
		private Vector3   startPosition;
		public Vector3    endPosition;
		public float      speed = 3.0f;
		
		private bool      triggered = false;
		
		void Start ()
		{
				startPosition = movedItem.transform.position;
		}
	
		void Update ()
		{
				if (triggered)
						movedItem.transform.position = Vector3.MoveTowards (movedItem.transform.position, endPosition, Time.deltaTime * speed);
				else
						movedItem.transform.position = Vector3.MoveTowards (movedItem.transform.position, startPosition, Time.deltaTime * speed);
		}
		
		override public void Trigger (bool trigger)
		{
				triggered = trigger;
		}
		
}