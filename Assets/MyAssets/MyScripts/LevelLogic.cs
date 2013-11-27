using UnityEngine;
using System.Collections;

public class LevelLogic : MonoBehaviour
{
		public GameObject playerPrefab;
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
		
		public Transform[] spawnLocations;
		
		// Use this for initialization
		void Start ()
		{
				for (int i = 1; i <= GameLogic.Instance.numPlayers; ++i) {
						GameObject playerObj = (GameObject)Instantiate (playerPrefab, spawnLocations [i - 1].position, Quaternion.identity);
						playerObj.renderer.material.color = colors [i - 1];
						PlayerController controller = playerObj.GetComponent<PlayerController> ();
						controller.index = (OuyaSDK.OuyaPlayer)i;
						controller.splitController = GameLogic.Instance.splitControllers;
						controller.leftSplit = (i % 2) != 0;
				}
				
		}
	
		// Update is called once per frame
		//	void Update ()
		//	{

		//	}
}
