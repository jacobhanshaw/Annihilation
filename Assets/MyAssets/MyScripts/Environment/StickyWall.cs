﻿using UnityEngine;
using System.Collections;

public class StickyWall : MonoBehaviour
{
		void Update ()
		{
				foreach (Transform child in transform) {
						if (child.gameObject.CompareTag ("Player") || child.gameObject.CompareTag ("NPC")) {
								PlayerController playerController = child.gameObject.GetComponent<PlayerController> ();
								if (playerController.Connected ())
										child.transform.parent = null;
								else
										child.transform.parent = gameObject.transform;
						}
				}  
		}

		void OnTriggerEnter2D (Collider2D other)
		{
				other.transform.parent = gameObject.transform;
		}
		
		void OnTriggerExit2D (Collider2D other)
		{
				other.transform.parent = null;
		}
	
}