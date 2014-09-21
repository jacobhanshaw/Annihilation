using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuiCameraLogic : MonoBehaviour
{	
		private List<TextMesh> playerScores;
		
		private int minPlayerNumber;
		private int maxPlayerNumber;

		private const string LEFT_SCORE_NAME = "Player1Name";
		private const string LEFT_SCORE_POINTS = "Player1Points";
		private const string RIGHT_SCORE_NAME = "Player2Name";
		private const string RIGHT_SCORE_POINTS = "Player2Points";
	
		void Start ()
		{
				minPlayerNumber = HelperFunction.Instance.PlayersInLayer (gameObject.layer, 1);
				maxPlayerNumber = HelperFunction.Instance.PlayersInLayer (gameObject.layer, 2);
				
				playerScores = new List<TextMesh> ();
				
				TextMesh player1Name = transform.FindChild (LEFT_SCORE_NAME).GetComponent<TextMesh> ();
				TextMesh player1Points = transform.FindChild (LEFT_SCORE_POINTS).GetComponent<TextMesh> ();
				playerScores.Add (player1Points);
		
				int trueNumber = minPlayerNumber;
				if (minPlayerNumber == 3 && GameLogic.Instance.numPlayers == 2 && GameLogic.Instance.splitScreen)
						trueNumber = 2;
					
				player1Name.text = "Player" + trueNumber.ToString ();
				player1Name.color = GameLogic.Instance.colors [trueNumber - 1];
				player1Points.color = player1Name.color;

				if (!((maxPlayerNumber > GameLogic.Instance.numPlayers) || (GameLogic.Instance.splitScreen && GameLogic.Instance.numPlayers == 2))) {
						TextMesh player2Name = transform.FindChild (RIGHT_SCORE_NAME).GetComponent<TextMesh> ();
						player2Name.text = "Player" + maxPlayerNumber.ToString ();
						player2Name.color = GameLogic.Instance.colors [maxPlayerNumber - 1];
						TextMesh player2Points = transform.FindChild (RIGHT_SCORE_POINTS).GetComponent<TextMesh> ();
						player2Points.color = player2Name.color;
						playerScores.Add (player2Points);
				} else {
						transform.FindChild ("Player2Name").gameObject.SetActive (false);
						transform.FindChild ("Player2Points").gameObject.SetActive (false);
				}

		}
	
		public void UpdateScore (Achievement achievement, int playerIndex, int newScore)
		{
				if (playerIndex >= minPlayerNumber && playerIndex <= maxPlayerNumber)
						playerScores [(playerIndex - 1) % 2].text = newScore.ToString ();
		}
}
