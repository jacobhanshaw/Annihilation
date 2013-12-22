using UnityEngine;
using System.Collections;

public class SwitchScript : MonoBehaviour
{
		public bool shouldScale;
		public int minItemsInTrigger;
		public bool oneTimeUse;
		
		private int currentItemsCount = 0;
		private bool wasTriggered;

		private GameEvent[] gameEvents;

		void Start ()
		{
				if (shouldScale) {
						int yItems = (minItemsInTrigger / 4) + 1;
						int xItems = minItemsInTrigger % 4;
						gameObject.transform.localScale = new Vector3 (gameObject.transform.localScale.x * xItems, gameObject.transform.localScale.y * yItems, gameObject.transform.localScale.z);
						gameObject.transform.position = new Vector3 (gameObject.transform.position.x, gameObject.transform.position.y + (gameObject.transform.localScale.y - 1.0f) / 2.0f, gameObject.transform.position.z);
				}
				
				gameEvents = transform.GetComponents<GameEvent> ();
		}
		
	
		void OnTriggerEnter2D (Collider2D other)
		{
				++currentItemsCount;
				if (currentItemsCount >= minItemsInTrigger && !wasTriggered) {
						wasTriggered = true;
						foreach (GameEvent gameEvent in gameEvents)
								gameEvent.Trigger (wasTriggered);
				
						gameObject.renderer.material.color = new Color (0.0f, 1.0f, 0.0f);
				}
		}
	
		void OnTriggerExit2D (Collider2D other)
		{
				--currentItemsCount;
				if (!oneTimeUse) {
						if (currentItemsCount < minItemsInTrigger && wasTriggered) {
								wasTriggered = false;
								foreach (GameEvent gameEvent in gameEvents)
										gameEvent.Trigger (wasTriggered);
								
								gameObject.renderer.material.color = new Color (1.0f, 0.0f, 0.0f);
						}
				}
		}
}
