using UnityEngine;
using System.Collections;

public class SwitchScript : MonoBehaviour
{

		public GameObject movedItem;
		private Vector3   startPosition;
		public Vector3    endPosition;
		public float      speed = 3.0f;
		
		private bool      itemInTrigger = false;
		
		// Use this for initialization
		void Start ()
		{
				startPosition = movedItem.transform.position;
		}
	
		// Update is called once per frame
		void Update ()
		{
				if (!itemInTrigger)
						movedItem.transform.position = Vector3.MoveTowards (movedItem.transform.position, startPosition, Time.deltaTime * speed);
		}
		
		void OnTriggerEnter2D (Collider2D other)
		{
				itemInTrigger = true;
		}
		
		void OnTriggerStay2D (Collider2D other)
		{
				movedItem.transform.position = Vector3.MoveTowards (movedItem.transform.position, endPosition, Time.deltaTime * speed);
		}
		
		void OnTriggerExit2D (Collider2D other)
		{
				itemInTrigger = false;
		}
}
