using UnityEngine;
using System.Collections;

public class CoinScript : MonoBehaviour
{
		//Achievement info
		public string   nameCoin;
		public int      pointsCoin;		

		//Movement Info
		private float    rotationSpeed = 100.0f;

		//Timer info
		public bool    timerUsed;
		public bool    coinExpires;
		public bool    startTimerOnStart;
		
		public float[]  seconds;
		private float   startTime = -1.0f;
		private float   maxTime = 0.0f;
		
	
		void Awake ()
		{
		}
		
		void Start ()
		{
						
		}

		void Update ()
		{
				gameObject.transform.Rotate (Vector3.up * Time.deltaTime * rotationSpeed);
				
		}
	
		public Achievement getAchievement ()
		{
				Achievement achievement = new Achievement ();
				achievement.name = nameCoin;
				achievement.points = pointsCoin;
				
				return achievement;
		}
		
		
}
