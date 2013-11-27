using UnityEngine;
using System.Collections;

public class GameLogic : Singleton<GameLogic>
{
		public int numPlayers = 2;  //debug number
		public bool splitControllers;
		
		public string currentLevelType;
		public int    currentLevelNumber;
	
		protected GameLogic ()
		{
		} // guarantee this will be always a singleton only - can't use the constructor!

		// Use this for initialization
		//void Start ()
		//{
		//}
	
		// Update is called once per frame
		//	void Update ()
		//	{
	
		//	}
}
