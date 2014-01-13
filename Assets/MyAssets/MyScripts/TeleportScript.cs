using UnityEngine;
using System.Collections;

public class TeleportScript : MonoBehaviour
{
		private int screen;
		
		public GameObject objectInTeleport;
		public GameObject receivedObject;

		void Start ()
		{
				if (gameObject.layer == LayerMask.NameToLayer ("Interact12") || gameObject.layer == LayerMask.NameToLayer ("Interact1") || gameObject.layer == LayerMask.NameToLayer ("Interact2") || gameObject.layer == LayerMask.NameToLayer ("Versus12"))
						screen = 0;
				else
						screen = 1;
		}
	
		void OnTriggerEnter2D (Collider2D other)
		{
				if (other.gameObject.CompareTag ("Player") || other.gameObject.CompareTag ("NPC")) {
				
						if (GameLogic.Instance.activeTeleports [screen] == null && other.gameObject != receivedObject) {
								GameLogic.Instance.activeTeleports [screen] = this;
								objectInTeleport = other.gameObject;	
						} else if (other.gameObject != receivedObject) {
								receivedObject = GameLogic.Instance.activeTeleports [screen].objectInTeleport;
								GameLogic.Instance.activeTeleports [screen].receivedObject = other.gameObject;
								
								Vector3 firstPlayerPosition = GameLogic.Instance.activeTeleports [screen].objectInTeleport.transform.position;
								GameLogic.Instance.activeTeleports [screen].objectInTeleport.transform.position = other.gameObject.transform.position;
								other.gameObject.transform.position = firstPlayerPosition;
						}
				}
		}
	
		void OnTriggerExit2D (Collider2D other)
		{			
				if (other.gameObject.CompareTag ("Player") || other.gameObject.CompareTag ("NPC")) {
				
						if (other.gameObject == receivedObject)
								receivedObject = null;
				
						if (GameLogic.Instance.activeTeleports [screen] == this)
								GameLogic.Instance.activeTeleports [screen] = null;
				}
						
		}	
}
