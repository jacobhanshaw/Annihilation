using UnityEngine;
using System.Collections;

public class GameLogic : Singleton<GameLogic>
{
		//Players and controls
		public int  numPlayers = 2;  //debug number
		private bool first = true;
		public bool splitScreen = true;
		public bool splitControllers = true;
		
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
		
		//Pause Logic
		public bool paused = false;
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
		
		public bool firstToFinish ()
		{
				if (first) {
						first = false;
						return true;
				}
			
				return false;
		}
		
		public void resetFirstToFinish ()
		{
				first = false;
		}
		
}
