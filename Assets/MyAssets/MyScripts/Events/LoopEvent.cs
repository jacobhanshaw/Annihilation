using UnityEngine;
using System.Collections;

public class LoopEvent : GameEvent
{

		public bool       inverted;
		public bool       reverses;
		public bool       ignorePause;
		public string      movedItemName;
		private GameObject movedItem;
		private Vector3   startPosition;
		public Vector3    endPosition;
		public float      speed = 3.0f;
	
		private bool      triggered = false;
		private bool      reversing = false;
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
				} else if (reverses)
						movedItem.transform.position = Vector3.MoveTowards (movedItem.transform.position, startPosition, deltaTime * speed);
		
				lastTime = Time.realtimeSinceStartup;
		}

		override public void Trigger (bool trigger)
		{
				triggered = trigger;
		}
}
