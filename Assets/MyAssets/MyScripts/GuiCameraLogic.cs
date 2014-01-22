using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuiCameraLogic : MonoBehaviour
{	
		private List<TextMesh> playerScores;
		
		private int minPlayerNumber;
		private int maxPlayerNumber;
	
		void Start ()
		{
				string layer = LayerMask.LayerToName (gameObject.layer);
				int initialPlayer = int.Parse (layer [layer.Length - 2].ToString ());
				int otherPlayer = int.Parse (layer [layer.Length - 1].ToString ());	
				minPlayerNumber = initialPlayer;
				maxPlayerNumber = otherPlayer;
				
				playerScores = new List<TextMesh> ();
				
				TextMesh player1Name = transform.FindChild ("Player1Name").GetComponent<TextMesh> ();
				TextMesh player1Points = transform.FindChild ("Player1Points").GetComponent<TextMesh> ();
				playerScores.Add (player1Points);
		
				int trueNumber = initialPlayer;
				if (initialPlayer == 3 && GameLogic.Instance.numPlayers == 2 && GameLogic.Instance.splitScreen)
						trueNumber = 2;
					
				player1Name.text = "Player" + trueNumber.ToString ();
				player1Name.color = GameLogic.Instance.colors [trueNumber - 1];
				player1Points.color = player1Name.color;

				if (!((otherPlayer > GameLogic.Instance.numPlayers) || (GameLogic.Instance.splitScreen && GameLogic.Instance.numPlayers == 2))) {
						TextMesh player2Name = transform.FindChild ("Player2Name").GetComponent<TextMesh> ();
						player2Name.text = "Player" + otherPlayer.ToString ();
						player2Name.color = GameLogic.Instance.colors [otherPlayer - 1];
						TextMesh player2Points = transform.FindChild ("Player2Points").GetComponent<TextMesh> ();
						player2Points.color = player2Name.color;
						playerScores.Add (player2Points);
				} else {
						transform.FindChild ("Player2Name").gameObject.SetActive (false);
						transform.FindChild ("Player2Points").gameObject.SetActive (false);
				}

		}
	
		public void UpdateScore (Achievement achievement, int playerIndex, int newScore)
		{
				int localPlayerIndex = playerIndex;
		
				if (localPlayerIndex >= minPlayerNumber && localPlayerIndex <= maxPlayerNumber)
						playerScores [(localPlayerIndex - 1) % 2].text = newScore.ToString ();
		}
}
