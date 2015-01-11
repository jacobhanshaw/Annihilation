using UnityEngine;
using System.Collections;

public class TeleportScript : MonoBehaviour
{
		public GameObject objectInTeleport;
		public GameObject receivedObject;

		void Start ()
		{
		}
	
		void OnTriggerEnter2D (Collider2D other)
		{
				if (other.gameObject.CompareTag ("Player") || other.gameObject.CompareTag ("NPC")) {
				
						if (GameLogic.Instance.activeTeleport == null && other.gameObject != receivedObject) {
								GameLogic.Instance.activeTeleport = this;
								objectInTeleport = other.gameObject;	
						} else if (GameLogic.Instance.activeTeleport != this && other.gameObject != receivedObject) {
								receivedObject = GameLogic.Instance.activeTeleport.objectInTeleport;
								GameLogic.Instance.activeTeleport.receivedObject = other.gameObject;
								
								Vector3 firstPlayerPosition = GameLogic.Instance.activeTeleport.objectInTeleport.transform.position;
								GameLogic.Instance.activeTeleport.objectInTeleport.transform.position = other.gameObject.transform.position;
								other.gameObject.transform.position = firstPlayerPosition;
						}
				}
		}
	
		void OnTriggerExit2D (Collider2D other)
		{			
				if (other.gameObject.CompareTag ("Player") || other.gameObject.CompareTag ("NPC")) {
				
						if (other.gameObject == objectInTeleport)
								objectInTeleport = null;

						if (other.gameObject == receivedObject)
								receivedObject = null;
				
						if (GameLogic.Instance.activeTeleport == this)
								GameLogic.Instance.activeTeleport = null;
				}
						
		}	
}
