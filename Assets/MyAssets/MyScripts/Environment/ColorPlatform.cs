using UnityEngine;
using System.Collections;

public class ColorPlatform : MonoBehaviour
{

		void Start ()
		{
				if (HelperFunction.Instance.PlayersInLayer (gameObject.layer, 1) != -1)
						ChangeColorToBlockPlayer (0);
				else
						ChangeColorToBlockPlayer (HelperFunction.Instance.PlayersInLayer (gameObject.layer, 2));
		}


		public void ChangeColorToBlockPlayer (int player)
		{
				Color playerColor;
				LayerMask playerLayer;
				if (player == 0) {
						playerLayer = LayerMask.NameToLayer ("Interact" + HelperFunction.Instance.GetPair (HelperFunction.Instance.PlayersInLayer (gameObject.layer, 2)).ToString ());
						playerColor = Color.white;
				} else {
						playerLayer = LayerMask.NameToLayer ("Interact" + player.ToString ());
						playerColor = GameLogic.Instance.colors [player - 1];
				}
				gameObject.layer = playerLayer;
				gameObject.renderer.material.color = playerColor;

		}
}
