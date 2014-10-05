using UnityEngine;
using System.Collections;

public class GameLogic : Singleton<GameLogic>
{
		//Players and controls
		public int  numPlayers = 4;  //debug number
		public bool splitScreen = true;
		public bool splitControllers = false;

		//Scoring
		private int place = 1; 			  //lowest unclaimed spot on the podium
		
		public Color[] colors = {
		new Color (0.0f, 181.0f / 255.0f, 1.0f),
		new Color (0.0f, 1.0f, 149.0f / 255.0f),
		new Color (246.0f / 255.0f, 64.0f / 255.0f, 195.0f / 255.0f), 
		new Color (46.0f / 255.0f, 141.0f / 255.0f, 91.0f / 255.0f),
		new Color (154.0f / 255.0f, 76.0f / 255.0f, 219.0f / 255.0f),
		new Color (136.0f / 255.0f, 206.0f / 255.0f, 198.0f / 255.0f), 
		new Color (160.0f / 255.0f, 204.0f / 255.0f, 186.0f / 255.0f),
		new Color (1.0f / 255.0f, 249.0f / 255.0f, 97.0f / 255.0f)
	};
		
		public Color[] spawnColors = {
		new Color (0.0f, 181.0f / 255.0f, 1.0f),
		new Color (0.0f, 1.0f, 0.0f),
		new Color (255.0f / 255.0f, 0.0f / 255.0f, 0.0f / 255.0f), 
		new Color (0.0f / 255.0f, 255.0f / 255.0f, 0.0f / 255.0f),
		new Color (154.0f / 255.0f, 76.0f / 255.0f, 219.0f / 255.0f),
		new Color (136.0f / 255.0f, 206.0f / 255.0f, 198.0f / 255.0f), 
		new Color (160.0f / 255.0f, 204.0f / 255.0f, 186.0f / 255.0f),
		new Color (1.0f / 255.0f, 249.0f / 255.0f, 97.0f / 255.0f)
	};
		
		//Level info
		public string currentLevelType;
		public int    currentLevelNumber;
		
		//Teleport variables
		public TeleportScript[] activeTeleports = { null, null };
		
		//Pause Logic
		public bool paused = false;
		private int  pausingPlayer = -1;
		
		protected GameLogic ()
		{
		} // guarantee this will be always a singleton only - can't use the constructor!

		//void Start () { }
		//void Update () { }
		
		public bool PausePressed (int playerIndex)
		{
				//if(playerIndex != 0)
				//ShowMenu()

				if (!paused || playerIndex == pausingPlayer) {
						paused = !paused;
						pausingPlayer = playerIndex;
	
						Time.timeScale = paused ? 0 : 1;
						return true;
				}

				return false;
		}
		
		//Methods to track podium placement for races
		public int placePlayerFinished ()
		{
				return place++;
		}
		
		public void resetForNewRace ()
		{
				place = 1;
		}
		
}
