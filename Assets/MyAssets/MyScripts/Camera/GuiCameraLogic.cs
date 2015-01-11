using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuiCameraLogic : MonoBehaviour
{	
		private TextMesh playerPoints;
		private TextMesh playerHealth;
	
		private const string SCORE_POINTS = "ScorePoints";
		private const string HEALTH_POINTS = "HealthPoints";
	
		void Start ()
		{
				playerPoints = transform.FindChild (SCORE_POINTS).GetComponent<TextMesh> ();
				playerHealth = transform.FindChild (HEALTH_POINTS).GetComponent<TextMesh> ();
		}
	
		public void UpdateScore (Achievement achievement, int newScore)
		{
				playerPoints.text = newScore.ToString ();
		}

		public void UpdateHealth (int newHealth)
		{
				playerHealth.text = newHealth.ToString ();
		}
}
