using UnityEngine;
using System.Collections;

public class MoveEvent : GameEvent
{
		public bool       inverted;
		public float      moveDelay;
		public bool       shouldReverse;
		public bool       ignorePause;
		public string      movedItemName;
		private GameObject movedItem;
		private Vector3   startPosition;
		public bool       relativeEndPosition;
		public Vector3    endPosition;
		public float      speed = 3.0f;
		
		private bool      triggered = false;
		private float     lastTime;
		
		void Start ()
		{
				GameObject potentialItem = GameObject.Find (movedItemName);
				if (inverted && gameObject.layer == potentialItem.layer)
						movedItem = GameObject.Find (movedItemName.Replace ("(Clone)", ""));
				else if (gameObject.layer == potentialItem.layer || potentialItem.layer == LayerMask.NameToLayer ("Default") || inverted)
						movedItem = potentialItem;
				else
						movedItem = GameObject.Find (movedItemName + "(Clone)");
						
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

				if (triggered) {
						if (moveDelay > 0.0f) 
								moveDelay -= deltaTime;
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