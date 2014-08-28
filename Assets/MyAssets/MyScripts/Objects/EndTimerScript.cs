using UnityEngine;
using System.Collections;

public class EndTimerScript : MonoBehaviour
{
		int minItemsInTrigger = 2;
		int currentItemsCount = 0;
		float coinLifetime = 2.0f;
		float timeBetweenCoins = 0.5f;
		float startTime;
		bool goodEnding;
		float goodEndingTime = 960.0f;
		bool gameOver;
		
		TextMesh timeText;
		
		public GameObject     regularCoin;
		public Vector3[]      bummerEyesLocations;
		private GameObject[]   bummerEyes;
		public GameObject     rigidCoin;

		// Use this for initialization
		void Start ()
		{
				startTime = Time.time;
				timeText = gameObject.GetComponent<TextMesh> ();
				bummerEyes = new GameObject[bummerEyesLocations.Length];
				for (int i = 0; i < bummerEyesLocations.Length; ++i) {
						GameObject coin = (GameObject)Instantiate (regularCoin, bummerEyesLocations [i], Quaternion.identity);
						coin.layer = gameObject.layer;
						coin.SetActive (false);
						bummerEyes [i] = coin;
				}
						
				if (GameLogic.Instance.splitScreen) {
						bool topScreen = false;
			
						string layer = LayerMask.LayerToName (gameObject.layer);
						string numOne = layer [layer.Length - 2].ToString ();
						if (numOne.Equals ("1"))
								topScreen = true;
						string numTwo = layer [layer.Length - 1].ToString ();
						if (numTwo.Equals ("1") || numTwo.Equals ("2"))
								topScreen = true;
			
						int playersInScreen = int.MaxValue;
						if (!topScreen && GameLogic.Instance.numPlayers <= 3)
								playersInScreen = 1;
						else if (topScreen && GameLogic.Instance.numPlayers <= 2)
								playersInScreen = 1;
			
						minItemsInTrigger = Mathf.Min (minItemsInTrigger, playersInScreen);
				}
		}
	
		// Update is called once per frame
		void Update ()
		{
				if (!gameOver) {
						float timePassed = Time.time - startTime;
		
						int minutes = (int)(timePassed / 60.0f);
						int seconds = (int)(timePassed % 60.0f);
						int rest = (int)((timePassed % 60.0f - seconds) * 100.0f);
						timeText.text = minutes + ":" + seconds.ToString ("00") + ":" + rest.ToString ("00");
				} else {
						if (goodEnding) {
								if (Time.time - startTime > timeBetweenCoins) {
										Vector3 location = new Vector3 (73.0f + Random.Range (-5.0f, 5.0f), -19.5f - Random.Range (0.0f, 1.5f), 0.0f);
										GameObject coin = (GameObject)Instantiate (rigidCoin, location, Quaternion.identity);
										coin.layer = gameObject.layer;
										Destroy (coin, coinLifetime);
								}
						}
				}
		}
		
		void OnTriggerEnter2D (Collider2D other)
		{
				++currentItemsCount;
				if (currentItemsCount == minItemsInTrigger && !gameOver) {
						gameOver = true;
						goodEnding = GameLogic.Instance.splitScreen ? GameLogic.Instance.placePlayerFinished() == 1 : Time.time - startTime < goodEndingTime;
						if (!goodEnding) {
								foreach (GameObject eye in bummerEyes)
										eye.SetActive (true);
						}
				}
		}
	
		void OnTriggerExit2D (Collider2D other)
		{
				--currentItemsCount;
		}
}
