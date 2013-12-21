using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuiCameraLogic : MonoBehaviour
{

		//TextMeshes
		private TextMesh infoTextA;
		private TextMesh infoTextB;
		private List<TextMesh> playerScores;
	
		void Start ()
		{
				infoTextA = transform.FindChild ("InfoTextA").gameObject.GetComponent<TextMesh> ();
				infoTextB = transform.FindChild ("InfoTextA").gameObject.GetComponent<TextMesh> ();
				
				playerScores = new List<TextMesh> ();
		
				for (int i = 1; i <= GameLogic.Instance.numPlayers; ++i) {
						playerScores.Add (transform.FindChild ("Player" + i + "Points").GetComponent<TextMesh> ());
				}
		}
	
		//void Update () { }
		
		public void UpdateScore (Achievement achievement, int playerIndex, int newScore)
		{
				playerScores [playerIndex - 1].text = newScore.ToString ();
		}
}
