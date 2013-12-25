using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SwitchScript : MonoBehaviour
{
		public bool skipScale;
		public bool skipRender;
		public int minItemsInTrigger;
		public bool oneTimeUse;
		
		// 		Can't do this, because parenting switching causes a re-count
		//		private int playersInTrigger = 0;
		private List<int> itemsInTrigger;
		private bool wasTriggered;

		private GameEvent[] gameEvents;

		void Start ()
		{
				if (!skipScale && !skipRender) {
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
				if (!itemsInTrigger.Contains (other.gameObject.GetInstanceID ()))
						itemsInTrigger.Add (other.gameObject.GetInstanceID ());
				if (itemsInTrigger.Count >= minItemsInTrigger && !wasTriggered) {
						wasTriggered = true;
						foreach (GameEvent gameEvent in gameEvents)
								gameEvent.Trigger (wasTriggered);
						if (!skipRender)
								gameObject.renderer.material.color = new Color (0.0f, 1.0f, 0.0f);
				}
		}
	
		void OnTriggerExit2D (Collider2D other)
		{
				itemsInTrigger.Remove (other.gameObject.GetInstanceID ());
				if (!oneTimeUse) {
						if (itemsInTrigger.Count < minItemsInTrigger && wasTriggered) {
								wasTriggered = false;
								foreach (GameEvent gameEvent in gameEvents)
										gameEvent.Trigger (wasTriggered);
								if (!skipRender)
										gameObject.renderer.material.color = new Color (1.0f, 0.0f, 0.0f);
						}
				}
		}
}
