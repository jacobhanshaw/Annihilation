using UnityEngine;
using System.Collections;

public class CoinScript : MonoBehaviour
{
		private float    rotationSpeed = 100.0f;

		public string[] achievementName;
		public int[]    points;
		public float[]  seconds;
		public bool     coinExpires;
		public bool     startTimerOnStart;
		
		private bool    timerUsed;
		private float   startTime = -1.0f;
		private float   maxTime = 0.0f;
		
		private GameObject textObject;
		private TextMesh timeText;
	
		// Use this for initialization
		void Start ()
		{
				textObject = transform.Find ("TimeLeft").gameObject;
				textObject.SetActive (timerUsed);
				timeText = textObject.GetComponent<TextMesh> ();
		
				if (seconds.Length > 0)
						maxTime = seconds [seconds.Length - 1];
						
				if (startTimerOnStart)
						startTimer ();
		}
	
		// Update is called once per frame
		void Update ()
		{
				gameObject.transform.Rotate (Vector3.up * Time.deltaTime * rotationSpeed);
				textObject.transform.eulerAngles = new Vector3 (0, 0, 0);
				
				if (timerUsed) {
						float timePassed = Time.time - startTime;
						float timeLeft = maxTime - timePassed;
						
						int minutes = (int)(timeLeft / 60.0f);
						int seconds = (int)(timeLeft % 60.0f);
						int rest = (int)((timeLeft % 60.0f - seconds) * 100.0f);
						timeText.text = minutes + ":" + seconds.ToString ("00") + ":" + rest.ToString ("00");
						
						if (coinExpires && timeLeft <= 0)
								Destroy (gameObject);
				}
		}
	
		public Achievement getAchievement ()
		{
				int index = getTimedIndex ();
		
				Achievement achievement = new Achievement ();
				achievement.achievementName = achievementName [index];
				achievement.points = points [index];
				
				return achievement;
		}
		
		int getTimedIndex ()
		{
				if (startTime == -1.0f)
						return 0;
				
				float timePassed = Time.time - startTime;
			
				for (int i = 0; i < seconds.Length; ++i) {
						if (timePassed < seconds [i])
								return i;
				}
			
				return achievementName.Length;
		}
		
		public void startTimer ()
		{
				startTime = Time.time;
				timerUsed = true;
				textObject.SetActive (timerUsed);
		}
		
}
