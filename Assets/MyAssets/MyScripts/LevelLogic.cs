using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelLogic : MonoBehaviour
{

/*	public delegate void EnergyTypeClickEventHandler(EnergySelection energyType);
	public static EnergyTypeClickEventHandler EnergyChange; check if null
	EnergyChange(currentEnergyType);
	EnergyTypeClickHandler.EnergyChange += EnergyChange;
	EnergyTypeClickHandler.EnergyChange -= EnergyChange;  */
	
		public GameObject playerPrefab;
		
		private List<PlayerController> playerControllers;
		private List<TextMesh> scoreTexts;
		
		private Color[] colors = {
				new Color (0.0f, 181.0f / 255.0f, 1.0f),
				new Color (0.0f, 1.0f, 149.0f / 255.0f),
				new Color (246.0f / 255.0f, 64.0f / 255.0f, 195.0f / 255.0f), 
				new Color (46.0f / 255.0f, 141.0f / 255.0f, 91.0f / 255.0f),
				new Color (154.0f / 255.0f, 76.0f / 255.0f, 219.0f / 255.0f),
				new Color (136.0f / 255.0f, 206.0f / 255.0f, 198.0f / 255.0f), 
				new Color (160.0f / 255.0f, 204.0f / 255.0f, 186.0f / 255.0f),
				new Color (1.0f / 255.0f, 249.0f / 255.0f, 97.0f / 255.0f)
		};
		
		private Color[] spawnColors = {
		new Color (0.0f, 181.0f / 255.0f, 1.0f),
		new Color (0.0f, 1.0f, 0.0f),
		new Color (246.0f / 255.0f, 64.0f / 255.0f, 195.0f / 255.0f), 
		new Color (46.0f / 255.0f, 141.0f / 255.0f, 91.0f / 255.0f),
		new Color (154.0f / 255.0f, 76.0f / 255.0f, 219.0f / 255.0f),
		new Color (136.0f / 255.0f, 206.0f / 255.0f, 198.0f / 255.0f), 
		new Color (160.0f / 255.0f, 204.0f / 255.0f, 186.0f / 255.0f),
		new Color (1.0f / 255.0f, 249.0f / 255.0f, 97.0f / 255.0f)
	};
		
		public GameObject[] spawnLocations;
		
		void Start ()
		{
				playerControllers = new List<PlayerController> ();
				scoreTexts = new List<TextMesh> ();
				
				for (int i = 1; i <= GameLogic.Instance.numPlayers; ++i) {
						GameObject playerObj = (GameObject)Instantiate (playerPrefab, spawnLocations [i - 1].transform.position, Quaternion.identity);
						playerObj.renderer.material.color = colors [i - 1];
						PlayerController controller = playerObj.GetComponent<PlayerController> ();
						playerControllers.Add (controller);
						scoreTexts.Add (Camera.main.gameObject.transform.FindChild ("Player" + i + "Points").GetComponent<TextMesh> ());
						controller.spawnColor = spawnColors [i - 1];
						controller.spawnLocation = spawnLocations [i - 1];
						controller.index = GameLogic.Instance.splitControllers ? (OuyaSDK.OuyaPlayer)((i + 1) / 2) : (OuyaSDK.OuyaPlayer)i;
						controller.splitController = GameLogic.Instance.splitControllers;
						controller.leftSplit = ((i % 2) != 0);
				}
				
		}
	
		void Update ()
		{
				for (int i = 0; i < playerControllers.Count; ++i) {
						scoreTexts [i].text = playerControllers [i].score.ToString ();
				}
		}
}
