using UnityEngine;
using System.Collections;

public class DuplicateEvents : MonoBehaviour
{


		void Start ()
		{

				foreach (ItemEvent itemEvent in gameObject.GetComponents<GameEvent>()) {
						if (itemEvent.itemName.EndsWith ("0")) {
								string baseName = itemEvent.itemName.Substring (0, itemEvent.itemName.LastIndexOf ("0"));
								bool notNull;
								ItemEvent newItemEvent;
								int index = 0;
								do {
										++index;
										newItemEvent = HelperFunction.Instance.CopyComponent (itemEvent, gameObject);
										notNull = newItemEvent.setNewItemName (baseName + index.ToString ());
								} while (notNull);

								if (newItemEvent)
										DestroyImmediate (newItemEvent);
						}

				}
		}

}
