using UnityEngine;
using System.Collections;

public class GameLogic : Singleton<GameLogic>
{
		//Players and controls
		public int numPlayers = 2;  //debug number
		public bool splitControllers;
		
		//Level info
		public string currentLevelType;
		public int    currentLevelNumber;
		
		//Pause Logic
		private bool paused = false;
		private int  pausingPlayer = -1;
	
		protected GameLogic ()
		{
		} // guarantee this will be always a singleton only - can't use the constructor!

		//void Start () { }
		//	void Update () { }
		
		public void PausePressed (int playerIndex)
		{
				if (!paused || playerIndex == pausingPlayer) {
						paused = !paused;
						pausingPlayer = playerIndex;
	
						if (paused)
								Time.timeScale = 0;
						else
								Time.timeScale = 1;
				}
		}
}
