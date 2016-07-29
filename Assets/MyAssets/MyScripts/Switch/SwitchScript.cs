using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SwitchScript : MonoBehaviour
{
		public bool invisible;
		public bool ignoreAutoColor;
		public int minItemsInTrigger = 1;
		public bool oneTimeUse;
		
		// 		Can't do this, because parenting switching causes a re-count
		//		private int playersInTrigger = 0;
		private List<int> itemsInTrigger;
		private bool wasTriggered;

		//private Vector2 bottomLeft;
		//private Vector2 topRight;
	
		private int layerMask = 0;

		private GameEvent[] gameEvents;

		void Start ()
		{
				gameEvents = transform.GetComponents<GameEvent> ();
				itemsInTrigger = new List<int> ();

				Vector2 position2d = gameObject.transform.position;

				//bottomLeft = HelperFunction.Instance.BottomLeftOfBoxCollider2D (position2d, ((BoxCollider2D)gameObject.collider2D));
				//topRight = HelperFunction.Instance.TopRightOfBoxCollider2D (position2d, ((BoxCollider2D)gameObject.collider2D));

				if (minItemsInTrigger <= 0)
						Trigger (true);
		}

		void Update ()
		{
				/*		Collider2D[] colliders = Physics2D.OverlapAreaAll (bottomLeft, 
		                                                   topRight, layerMask);

				if (colliders.Length >= minItemsInTrigger && !wasTriggered)
						Trigger (true);
				else if (!oneTimeUse && colliders.Length < minItemsInTrigger && wasTriggered)
						Trigger (false); */
		}
		
		void Trigger (bool trigger)
		{
				wasTriggered = trigger;
				foreach (GameEvent gameEvent in transform.GetComponents<GameEvent> ())
						gameEvent.Trigger (wasTriggered);
				if (!invisible && !ignoreAutoColor) {
						if (wasTriggered)
								gameObject.renderer.material.color = new Color (0.0f, 1.0f, 0.0f);
						else
								gameObject.renderer.material.color = new Color (1.0f, 0.0f, 0.0f);
				}
		}

		void OnTriggerEnter2D (Collider2D other)
		{
				if (other.CompareTag ("Player") || other.CompareTag ("NPC")) {
						if (!itemsInTrigger.Contains (other.GetInstanceID ()))
								itemsInTrigger.Add (other.GetInstanceID ());

						if (itemsInTrigger.Count >= minItemsInTrigger && !wasTriggered) 
								Trigger (true);
				}

		}

		void OnTriggerExit2D (Collider2D other)
		{
				if (other.CompareTag ("Player") || other.CompareTag ("NPC")) {
						itemsInTrigger.Remove (other.GetInstanceID ());
						if (!oneTimeUse) {
								if (itemsInTrigger.Count < minItemsInTrigger && wasTriggered)
										Trigger (false);
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
