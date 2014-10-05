using UnityEngine;
using System.Collections;

public class MoveEvent : ItemEvent
{	
		private const float DEFAULT_MOVEMENT_SPEED = 3.0f;
		private const float DEFAULT_ROTATION_SPEED = 10.0f;

		public bool       		loop;
		public bool      		shouldReverse;

		public float 			startDelay;
		public float[]      	moveDelay;
		public float[]      	movementSpeed;
		public float[]      	rotationSpeed;

		public Vector3[]    	movementVector;
		public Vector3[]     	randomizeMovementMax;
		public Vector3[]    	rotationVector;
		public Vector3[]     	randomizeRotationMax;

		private bool      	 	reversing;
		private int	        	currentIndex; 

		private float[]      	originalMoveDelay;
		private Vector3[]   	nextPosition;
		private Quaternion[]    nextRotation;

		private float     	  	lastTime;
		
		new void Start ()
		{			

				base.Start ();

				if (!loop)
						HelperFunction.Instance.Assert (shouldReverse);

				if (moveDelay.Length == 0)
						moveDelay = new float[]{ 0.0f };
				if (movementSpeed.Length == 0)
						movementSpeed = new float[]{  DEFAULT_MOVEMENT_SPEED };
				if (rotationSpeed.Length == 0)
						rotationSpeed = new float[]{  DEFAULT_ROTATION_SPEED };

				if (rotationVector.Length == 0)
						rotationVector = new Vector3[] { Vector3.zero };
				if (randomizeMovementMax.Length == 0)
						randomizeMovementMax = new Vector3[] { Vector3.zero };
				if (randomizeRotationMax.Length == 0)
						randomizeRotationMax = new Vector3[]  { Vector3.zero };

				moveDelay = SpreadArrayToLength<float> (moveDelay, movementVector.Length);
				movementSpeed = SpreadArrayToLength<float> (movementSpeed, movementVector.Length);
				rotationSpeed = SpreadArrayToLength<float> (rotationSpeed, movementVector.Length);

				rotationVector = SpreadArrayToLength<Vector3> (rotationVector, movementVector.Length);
				randomizeMovementMax = SpreadArrayToLength<Vector3> (randomizeMovementMax, movementVector.Length);
				randomizeRotationMax = SpreadArrayToLength<Vector3> (randomizeRotationMax, movementVector.Length);

				originalMoveDelay = (float[])moveDelay.Clone ();

				nextPosition = new Vector3[(movementVector.Length + 1)];
				nextRotation = new Quaternion[(rotationVector.Length + 1)];

				nextPosition [0] = item.transform.position;
				nextRotation [0] = item.transform.rotation;

				for (int i = 0; i < nextPosition.Length-1; ++i) {
						nextPosition [i + 1] = nextPosition [i] + (movementVector [i] + RandomVector (randomizeMovementMax [i]));
						Quaternion newRotation = Quaternion.identity;
						newRotation.eulerAngles = (rotationVector [i] + RandomVector (randomizeRotationMax [i]));
						nextRotation [i + 1] = nextRotation [i] * newRotation;
				}

				//COULD NOT DO THIS OR DUPLICATE WON'T WORK \/
				//movementVector = null;
				//rotationVector = null; 
		}
		
		private Vector3 RandomVector (Vector3 maxes)
		{
				return new Vector3 (Random.Range (-maxes.x, maxes.x), Random.Range (-maxes.y, maxes.y), Random.Range (-maxes.z, maxes.z));
		}
	
		/*
		private T ItemAtIndex<T> (T[] array, int index)
		{
				if (array.Length == 1)
						return array [0];

				return array [index];
		}
	*/	

		private T[] SpreadArrayToLength<T> (T[] array, int newLength)
		{
				T temp;
				if (array.Length == 1) {
						temp = array [0];
						array = new T[newLength];
						for (int i = 0; i < newLength; ++i)
								array [i] = temp;
						return array;
				}

				HelperFunction.Instance.Assert (newLength == array.Length);
				return null;
		}
	
		void Update ()
		{
				float deltaTime;
				if (ignorePause)
						deltaTime = Time.realtimeSinceStartup - lastTime;
				else
						deltaTime = Time.deltaTime;
	
				if (triggeredChanged) {
						UpdateOnTriggerChanged ();
						triggeredChanged = false;
				} else
						UpdateIfNextPositionReached ();

				int transitionIndex = currentIndex;
				if (!reversing)
						transitionIndex -= 1;

				if (triggered && startDelay > 0.0f) 
						startDelay -= deltaTime;
				else if (triggered && moveDelay [transitionIndex] > 0.0f) 
						moveDelay [transitionIndex] -= deltaTime;
				else if (triggered || (reversing && shouldReverse)) {
						item.transform.position = Vector3.MoveTowards (item.transform.position, nextPosition [currentIndex], deltaTime * movementSpeed [transitionIndex]);
						item.transform.rotation = Quaternion.RotateTowards (item.transform.rotation, nextRotation [currentIndex], deltaTime * rotationSpeed [transitionIndex]);
				}

				lastTime = Time.realtimeSinceStartup;
		}

		private void UpdateIfNextPositionReached ()
		{
				if (item.transform.position == nextPosition [currentIndex]) {
						if (currentIndex != (nextPosition.Length - 1) || loop) {
								if (reversing)
										currentIndex -= 1;
								else
										currentIndex += 1;
				
								CheckIndexOverflow ();
						}
				}
		}

		private void UpdateOnTriggerChanged ()
		{
				if (triggered && reversing) {
						moveDelay = (float[])originalMoveDelay.Clone ();
						reversing = false;
						currentIndex += 1;
				} else if (!triggered && shouldReverse) {
						reversing = true;
						currentIndex -= 1;
				}

				CheckIndexOverflow ();
		}

		private void CheckIndexOverflow ()
		{
				if (currentIndex < 0) {
						reversing = false;
						currentIndex = 1;
				} else if (currentIndex >= nextPosition.Length) {
						reversing = loop;
						currentIndex = nextPosition.Length - 2;
				}
		}
		
}