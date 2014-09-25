using UnityEngine;
using System.Collections;

public class MoveEvent : ItemsEvent
{	
		public float      speed = 3.0f;
		public bool       loop;

		public bool       shouldReverse;
		private bool[]      reversing;
	
		private Vector3[]   startPosition;
		public Vector3[]    movementVector;
		private Vector3[]   endPosition;
		public float[]      moveDelay;
	
		private float     lastTime;
		
		new void Start ()
		{				
				Debug.Log ("MoveEventCalled");
				
				base.Start ();

				startPosition = new Vector3[items.Length];
				reversing = new bool[items.Length];

				Vector3 tempVector;
				bool replaceMoveVector = false;
				if (movementVector.Length < items.Length) {
						tempVector = movementVector [0];
						movementVector = new Vector3[items.Length];
						movementVector [0] = tempVector;
						replaceMoveVector = true;
				}

				float temp;
				bool replaceMoveDelay = false;
				if (moveDelay.Length < items.Length) {
						if (moveDelay.Length > 0)
								temp = moveDelay [0];
						else
								temp = 0.0f;
						moveDelay = new float[items.Length];
						moveDelay [0] = temp;
						replaceMoveDelay = true;
				}
				
				endPosition = new Vector3[items.Length];
				for (int i = 0; i < items.Length; ++i) {
						startPosition [i] = items [i].transform.position;

						if (replaceMoveVector)
								movementVector [i] = movementVector [0];
			 
						endPosition [i] = startPosition [i] + movementVector [i];

						if (replaceMoveDelay)
								moveDelay [i] = moveDelay [0];
				}
		}
	
		void Update ()
		{
				float deltaTime;
				if (ignorePause)
						deltaTime = Time.realtimeSinceStartup - lastTime;
				else
						deltaTime = Time.deltaTime;

				if (loop) {
						for (int i = 0; i < items.Length; ++i) {
								if (items [i].transform.position == startPosition [i])
										reversing [i] = false;
								if (items [i].transform.position == endPosition [i])
										reversing [i] = true;
						}
				}

				for (int i = 0; i < items.Length; ++i) {
			
						if (triggered) {
								if (moveDelay [i] > 0.0f) 
										moveDelay [i] -= deltaTime;
								else {
										if (reversing [i])
												items [i].transform.position = Vector3.MoveTowards (items [i].transform.position, startPosition [i], deltaTime * speed);
										else
												items [i].transform.position = Vector3.MoveTowards (items [i].transform.position, endPosition [i], deltaTime * speed);
								}
						} else if (shouldReverse)
								items [i].transform.position = Vector3.MoveTowards (items [i].transform.position, startPosition [i], deltaTime * speed);

				}
				
				lastTime = Time.realtimeSinceStartup;
		}
		
}