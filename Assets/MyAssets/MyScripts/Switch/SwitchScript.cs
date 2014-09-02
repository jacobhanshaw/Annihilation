using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SwitchScript : MonoBehaviour
{
		public bool invisible;
		public bool ignoreAutoColor;
		public int minItemsInTrigger;
		public bool oneTimeUse;
		
		// 		Can't do this, because parenting switching causes a re-count
		//		private int playersInTrigger = 0;
		private List<int> itemsInTrigger;
		private bool wasTriggered;

		private GameEvent[] gameEvents;

		void Start ()
		{
		
				if (GameLogic.Instance.splitScreen) {
						bool topScreen = false;
				
						string layer = LayerMask.LayerToName (gameObject.layer);
						string numOne = layer [layer.Length - 2].ToString ();
						if (numOne.Equals ("1"))
								topScreen = true;
						string numTwo = layer [layer.Length - 1].ToString ();
						if (numTwo.Equals ("1") || numTwo.Equals ("2"))
								topScreen = true;
						
						int playersInScreen = int.MaxValue;
						if (!topScreen && GameLogic.Instance.numPlayers <= 3)
								playersInScreen = 1;
						else if (topScreen && GameLogic.Instance.numPlayers <= 2)
								playersInScreen = 1;
		
						minItemsInTrigger = Mathf.Min (minItemsInTrigger, playersInScreen);
				}
		
				if (!invisible) {
						int yItems = (minItemsInTrigger / 4) + 1;
						int xItems = minItemsInTrigger % 4;
						gameObject.transform.localScale = new Vector3 (gameObject.transform.localScale.x * xItems, gameObject.transform.localScale.y * yItems, gameObject.transform.localScale.z);
						gameObject.transform.position = new Vector3 (gameObject.transform.position.x, gameObject.transform.position.y + (gameObject.transform.localScale.y - 1.0f) / 2.0f, gameObject.transform.position.z);
				}
				
				gameEvents = transform.GetComponents<GameEvent> ();
				itemsInTrigger = new List<int> ();
		}
		
	
		void OnTriggerEnter2D (Collider2D other)
		{

				if (other.CompareTag ("Player") || other.CompareTag ("NPC")) {
						if (!itemsInTrigger.Contains (other.GetInstanceID ()))
								itemsInTrigger.Add (other.GetInstanceID ());
						if (itemsInTrigger.Count >= minItemsInTrigger && !wasTriggered) {
								wasTriggered = true;
								foreach (GameEvent gameEvent in gameEvents)
										gameEvent.Trigger (wasTriggered);
								if (!invisible && !ignoreAutoColor)
										gameObject.renderer.material.color = new Color (0.0f, 1.0f, 0.0f);
						}
				}

		}
	
		void OnTriggerExit2D (Collider2D other)
		{
				Debug.Log ("FAIL");
				if (other.CompareTag ("Player") || other.CompareTag ("NPC")) {
						itemsInTrigger.Remove (other.GetInstanceID ());
						if (!oneTimeUse) {
								if (itemsInTrigger.Count < minItemsInTrigger && wasTriggered) {
										wasTriggered = false;
										foreach (GameEvent gameEvent in gameEvents)
												gameEvent.Trigger (wasTriggered);
										if (!invisible && !ignoreAutoColor)
												gameObject.renderer.material.color = new Color (1.0f, 0.0f, 0.0f);
								}
						}
				}
				
		}
		
		public void ShowButtonAsActivated (bool activated)
		{
				if (activated && !invisible && !ignoreAutoColor)
						gameObject.renderer.material.color = new Color (0.0f, 1.0f, 0.0f);
				else
						gameObject.renderer.material.color = new Color (1.0f, 0.0f, 0.0f);
		}
}
