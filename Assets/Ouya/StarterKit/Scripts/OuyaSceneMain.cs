/*
 * Copyright (C) 2012, 2013 OUYA, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using UnityEngine;

public class OuyaSceneMain : MonoBehaviour
{		
		private int splitControllers = -1;

		private int selectedNumPlayers = 0;
		private int   availablePlayers = 2;
		private float maxNumPlayers = 4.0f;
		private float maxItemsPerRow = 5.0f;
		
		private string storyMiniMode = "";
	
		private int levelNumber = -1;
		
		public void OnGUI ()
		{
				if (splitControllers == -1)
						SetUpSplitControllerButtons ();
				else if (selectedNumPlayers == 0)
						SetUpPlayerButtons ();
				else if (storyMiniMode.Equals (""))
						SetUpStoryMiniButtons ();
				else if (levelNumber == -1)
						SetUpSceneSelectButtons ();
				else {
						GameLogic.Instance.numPlayers = selectedNumPlayers;
						GameLogic.Instance.splitControllers = splitControllers != 0;
						
						GameLogic.Instance.currentLevelType = storyMiniMode;
						GameLogic.Instance.currentLevelNumber = levelNumber;
						
						Application.LoadLevel (storyMiniMode + levelNumber.ToString ());
				}
		}
	
		void Update ()
		{
				CheckNumAvailablePlayers ();
		}
		
		
		void SetUpSplitControllerButtons ()
		{
				float numButtons = 2.0f;
				GUIStyle myButtonStyle = new GUIStyle (GUI.skin.button);
				myButtonStyle.fontSize = 50;
			
				// Load and set Font
				//	Font myFont = (Font)Resources.Load("Fonts/comic", typeof(Font));
				//	myButtonStyle.font = myFont;
			
				float buttonWidth = Screen.width / 2.0f;
				float buttonHeight = Screen.height / 4.0f;
				float verticalSpace = (Screen.height - buttonHeight * numButtons) / (numButtons + 1.0f);
			
				float startHeight = 0.0f;
				string[] buttonNameArray = { "Share Controllers", "Don't Share Controllers" };
				for (int i = 0; i < numButtons; ++i) {
						string buttonName = buttonNameArray [i];
				
						if (GUI.Button (new Rect (Screen.width / 2.0f - buttonWidth / 2.0f, startHeight + verticalSpace, buttonWidth, buttonHeight), buttonName, myButtonStyle)) {		
								splitControllers = 1 - i;
								maxNumPlayers *= (splitControllers + 1);
						}
						startHeight += (verticalSpace + buttonHeight);
				}
			
		}
	
		void SetUpPlayerButtons ()
		{
				CheckNumAvailablePlayers ();
		
				float numPlayerButtons = maxNumPlayers - 1.0f;
				GUIStyle myButtonStyle = new GUIStyle (GUI.skin.button);
				myButtonStyle.fontSize = 50;
		
				// Load and set Font
				//	Font myFont = (Font)Resources.Load("Fonts/comic", typeof(Font));
				//	myButtonStyle.font = myFont;
		
				float buttonWidth = Screen.width / 2.0f;
				float buttonHeight = Screen.height / (numPlayerButtons + 1.0f);
				float verticalSpace = (Screen.height - buttonHeight * numPlayerButtons) / (numPlayerButtons + 1.0f);
		
				float startHeight = 0.0f;
				for (int i = 0; i < numPlayerButtons; ++i) {
						
						if (availablePlayers >= (i + 2)) {
								myButtonStyle.normal.textColor = Color.white;
								myButtonStyle.hover.textColor = Color.white;
						} else {
								myButtonStyle.normal.textColor = Color.red;
								myButtonStyle.hover.textColor = Color.red;
						}
			
						if (GUI.Button (new Rect (Screen.width / 2.0f - buttonWidth / 2.0f, startHeight + verticalSpace, buttonWidth, buttonHeight), (i + 2) + " Player", myButtonStyle)) {
								if (availablePlayers >= (i + 2))
										selectedNumPlayers = i + 2;
						}
						startHeight += (verticalSpace + buttonHeight);
				}
		}
		
		void SetUpStoryMiniButtons ()
		{
				float numButtons = 2.0f;
				GUIStyle myButtonStyle = new GUIStyle (GUI.skin.button);
				myButtonStyle.fontSize = 50;
		
				// Load and set Font
				//	Font myFont = (Font)Resources.Load("Fonts/comic", typeof(Font));
				//	myButtonStyle.font = myFont;
		
				float buttonWidth = Screen.width / 2.0f;
				float buttonHeight = Screen.height / 4.0f;
				float verticalSpace = (Screen.height - buttonHeight * numButtons) / (numButtons + 1.0f);
		
				float startHeight = 0.0f;
				string[] storyMini = { "StoryMode", "Minigames" };
				for (int i = 0; i < numButtons; ++i) {
						string buttonName = storyMini [i];
				
						if (GUI.Button (new Rect (Screen.width / 2.0f - buttonWidth / 2.0f, startHeight + verticalSpace, buttonWidth, buttonHeight), buttonName, myButtonStyle))
								storyMiniMode = storyMini [i];
						startHeight += (verticalSpace + buttonHeight);
				}
				
		}
	
		void SetUpSceneSelectButtons ()
		{
				float numButtons = storyMiniMode.Equals ("StoryMode") ? 20 : 12;
				GUIStyle myButtonStyle = new GUIStyle (GUI.skin.button);
				myButtonStyle.fontSize = 50;
		
				// Load and set Font
				//	Font myFont = (Font)Resources.Load("Fonts/comic", typeof(Font));
				//	myButtonStyle.font = myFont;
		
				int rows = Mathf.CeilToInt (numButtons / maxItemsPerRow);
				float buttonWidth = Screen.width / (maxItemsPerRow + 2.0f);
				float buttonHeight = Screen.height / (rows + 1.0f);
				float horizontalSpace = (Screen.width - buttonWidth * maxItemsPerRow) / (maxItemsPerRow + 1.0f);
				float verticalSpace = (Screen.height - buttonHeight * rows) / (rows + 1.0f);
				
				bool done = false;
				float startHeight = 0.0f;
				for (int i = 0; i < rows && !done; ++i) {
						float startWidth = 0.0f;
						for (int j = 0; j < maxItemsPerRow; ++j) {
								int level = (i * (int)maxItemsPerRow + j + 1);
								if (level > numButtons) {
										done = true;
										break;
								}
						
								if (GUI.Button (new Rect (startWidth + horizontalSpace, startHeight + verticalSpace, buttonWidth, buttonHeight), level.ToString (), myButtonStyle))
										levelNumber = level;
										
								startWidth += (horizontalSpace + buttonWidth);
						}
						startHeight += (verticalSpace + buttonHeight);
				}
		}
	
	
		void CheckNumAvailablePlayers ()
		{
				availablePlayers = 2;
		
				if (OuyaSDK.GetSupportedController (OuyaSDK.OuyaPlayer.player1) != null)
						availablePlayers = 1;
				else if (OuyaSDK.GetSupportedController (OuyaSDK.OuyaPlayer.player2) != null)
						availablePlayers = 2;
				else if (OuyaSDK.GetSupportedController (OuyaSDK.OuyaPlayer.player3) != null)
						availablePlayers = 3;
				else if (OuyaSDK.GetSupportedController (OuyaSDK.OuyaPlayer.player4) != null)
						availablePlayers = 4;
				else if (OuyaSDK.GetSupportedController (OuyaSDK.OuyaPlayer.player5) != null)
						availablePlayers = 5;
						
				availablePlayers *= (splitControllers + 1);
		}
}