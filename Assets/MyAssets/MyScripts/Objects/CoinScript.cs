using UnityEngine;
using System.Collections;

public class CoinScript : MonoBehaviour
{
		//Achievement info
		public string[] nameCoin;
		public int[]    pointsCoin;		

		//Movement Info
		private float    rotationSpeed = 100.0f;

		//Timer info
		public bool    timerUsed;
		public bool    coinExpires;
		public bool    startTimerOnStart;
		
		public float[]  seconds;
		private float   startTime = -1.0f;
		private float   maxTime = 0.0f;
		
		//Display Info
		private GameObject textObject;
		private TextMesh timeText;
	
		void Awake ()
		{
				textObject = transform.Find ("TimeLeft").gameObject;
		}
		
		void Start ()
		{
				
				timeText = textObject.GetComponent<TextMesh> ();
		
				if (seconds.Length > 0)
						maxTime = seconds [seconds.Length - 1];
						
				if (startTimerOnStart)
						startTimer ();
				else
						textObject.SetActive (false);
		}

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
						
						if (timeLeft <= 0) {
								if (coinExpires)
										Destroy (gameObject);
								else {
										timerUsed = false;
										timeText.text = "00:00:00";
								}	
						}
								
				}
		}
	
		public Achievement getAchievement ()
		{
				int index = getTimedIndex ();
				Debug.Log ("Index: " + index + " Versus: " + nameCoin.Length);
				Achievement achievement = new Achievement ();
				achievement.name = nameCoin [index];
				achievement.points = pointsCoin [index];
				
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
			
				return seconds.Length;
		}
		
		public void startTimer ()
		{
				startTime = Time.time;
				timerUsed = true;
				textObject.SetActive (timerUsed);
		}
		
}
