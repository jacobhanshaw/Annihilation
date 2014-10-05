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

		//private Vector2 bottomLeft;
		//private Vector2 topRight;
	
		private int layerMask = 0;

		private GameEvent[] gameEvents;

		void Start ()
		{

				int firstNum = HelperFunction.Instance.PlayersInLayer (gameObject.layer, 1);
				if (firstNum != -1)
						layerMask |= 1 << LayerMask.NameToLayer ("Player" + firstNum);
				int secondNum = HelperFunction.Instance.PlayersInLayer (gameObject.layer, 2);
				if (secondNum != -1)
						layerMask |= 1 << LayerMask.NameToLayer ("Player" + secondNum);

				bool topScreen = false;
				if (GameLogic.Instance.splitScreen) {
						

						if (firstNum == 1 || secondNum == 1 || secondNum == 2)
								topScreen = true;
						
						int playersInScreen = int.MaxValue;
						if (!topScreen && GameLogic.Instance.numPlayers <= 3)
								playersInScreen = 1;
						else if (topScreen && GameLogic.Instance.numPlayers <= 2)
								playersInScreen = 1;
		
						minItemsInTrigger = Mathf.Min (minItemsInTrigger, playersInScreen);
				}
		
				if (!invisible && topScreen) {
						int yItems = (minItemsInTrigger / 4) + 1;
						int xItems = minItemsInTrigger % 4;
						gameObject.transform.localScale = new Vector3 (gameObject.transform.localScale.x * xItems, gameObject.transform.localScale.y * yItems, gameObject.transform.localScale.z);
						gameObject.transform.position = new Vector3 (gameObject.transform.position.x, gameObject.transform.position.y + (gameObject.transform.localScale.y - 1.0f) / 2.0f, gameObject.transform.position.z);
				}
				
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
				foreach (GameEvent gameEvent in gameEvents)
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
