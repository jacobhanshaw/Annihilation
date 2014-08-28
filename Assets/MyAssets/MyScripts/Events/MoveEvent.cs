using UnityEngine;
using System.Collections;

public class MoveEvent : GameEvent
{

		public bool       inverted;
		public bool       ignorePause;
		public string      movedItemName;
		private GameObject movedItem;
		private Vector3   startPosition;
		public Vector3    endPosition;
		public float      speed = 3.0f;
		
		private bool      triggered = false;
		private float     lastTime;
		
		void Start ()
		{
				GameObject potentialItem = GameObject.Find (movedItemName);
				if (gameObject.layer == potentialItem.layer || potentialItem.layer == LayerMask.NameToLayer ("Default") || inverted)
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
		
				if (triggered)
						movedItem.transform.position = Vector3.MoveTowards (movedItem.transform.position, endPosition, deltaTime * speed);
				else
						movedItem.transform.position = Vector3.MoveTowards (movedItem.transform.position, startPosition, deltaTime * speed);
				
				lastTime = Time.realtimeSinceStartup;
		}
		
		override public void Trigger (bool trigger)
		{
				triggered = trigger;
		}
		
}