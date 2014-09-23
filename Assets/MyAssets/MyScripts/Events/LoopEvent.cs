using UnityEngine;
using System.Collections;

public class LoopEvent : GameEvent
{
		public bool       inverted;
		public bool       ignorePause;
	
		public string      movedItemName;
		protected GameObject movedItem;
	
		private Vector3   startPosition;
		public bool       relativeEndPosition;
		public Vector3    endPosition;

		public float      speed = 3.0f;
		public bool       shouldReverse;
		private bool      triggered = false;
		private bool      reversing = false;
		private float     lastTime;
	
		void Start ()
		{
				movedItem = HelperFunction.Instance.FindBasedOnLayer (movedItemName, gameObject.layer, inverted);

				startPosition = movedItem.transform.position;
				if (relativeEndPosition) 
						endPosition += startPosition;
		}

		void Update ()
		{
				float deltaTime;
				if (ignorePause)
						deltaTime = Time.realtimeSinceStartup - lastTime;
				else
						deltaTime = Time.deltaTime;

				if (movedItem.transform.position == startPosition)
						reversing = false;
				if (movedItem.transform.position == endPosition)
						reversing = true;

				if (triggered) {
						if (reversing)
								movedItem.transform.position = Vector3.MoveTowards (movedItem.transform.position, startPosition, deltaTime * speed);
						else
								movedItem.transform.position = Vector3.MoveTowards (movedItem.transform.position, endPosition, deltaTime * speed);
				} else if (shouldReverse)
						movedItem.transform.position = Vector3.MoveTowards (movedItem.transform.position, startPosition, deltaTime * speed);
		
				lastTime = Time.realtimeSinceStartup;
		}

		override public void Trigger (bool trigger)
		{
				triggered = trigger;
		}
}
