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

public class Menu : Singleton<Menu>
{		
		private float maxItemsPerRow = 5.0f;
	
		private int levelNumber = -1;

		private bool inLevel = false;

		private TextMesh textMesh;
		private string prevString;

		void Start ()
		{
				DontDestroyOnLoad (gameObject);

				textMesh = gameObject.GetComponent<TextMesh> ();

				//	GameLogic.PausedListeners += Paused;
		}
		
		public void OnGUI ()
		{
				if (levelNumber == -1)
						SetUpSceneSelectButtons ();
				else if (!inLevel) {
						GameLogic.Instance.currentLevelNumber = levelNumber;
						
						inLevel = true;
						Application.LoadLevel ("Story" + levelNumber.ToString ());
				}
		}
	
		void Update ()
		{

		}
		
		void Paused (bool paused)
		{
				if (paused) {
						prevString = textMesh.text;
						textMesh.text = "Paused";
				} else
						textMesh.text = prevString;
		}

		public void SetText (string text)
		{
				textMesh.text = text;
		}
	
		void SetUpSceneSelectButtons ()
		{
				float numButtons = 20;
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

}