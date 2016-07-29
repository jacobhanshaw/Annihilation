using UnityEngine;
using System.Collections;

public class GameLogic : Singleton<GameLogic>
{
	
		//Delegates
		public delegate void Paused (bool paused);
		public static Paused PausedListeners; //check if null

		//Players and controls
		public int  numPlayers = 1; 
	
		
		public Color[] colors = {
		new Color (0.0f, 181.0f / 255.0f, 1.0f),
	};
		
		public Color[] spawnColors = {
		new Color (0.0f, 181.0f / 255.0f, 1.0f),
	};
		
		//Level info
		public int    currentLevelNumber;
		
		//Teleport variables
		public TeleportScript activeTeleport;
		
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

						if (PausedListeners != null)
								PausedListeners (paused);
	
						Time.timeScale = paused ? 0 : 1;
						return true;
				}

				return false;
		}
		
}
