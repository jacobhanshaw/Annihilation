using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StickyWall : MonoBehaviour
{
		Vector3 prevLocation;
		List<GameObject> players;
		
		void Start ()
		{
				prevLocation = gameObject.transform.position;
				players = new List<GameObject> ();
		}

		void Update ()
		{
				if (prevLocation != gameObject.transform.position) {
						foreach (GameObject player in players)
								player.transform.position += (gameObject.transform.position - prevLocation);

						prevLocation = gameObject.transform.position;
				}
		}

		void OnTriggerEnter2D (Collider2D other)
		{
				if (other.gameObject.CompareTag ("Player"))
						players.Add (other.gameObject);
		}
		
		void OnTriggerExit2D (Collider2D other)
		{
				if (other.gameObject.CompareTag ("Player")) 
						players.Remove (other.gameObject);
		}
	
}