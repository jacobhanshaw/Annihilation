using UnityEngine;
using System.Collections;

public class LevelLogic : MonoBehaviour
{
		public string levelType;
		public int    levelNumber;
	
		public GameObject playerPrefab;
		public Material green;
		
		private int playerCount;
		private ArrayList spawnLocation;
		
		// Use this for initialization
		void Start ()
		{

				for (int i = 1; i <= playerCount; ++i) {
						GameObject player2Obj = (GameObject)Instantiate (playerPrefab, new Vector3 (0, 10, 0), Quaternion.identity);
						player2Obj.renderer.material = green;
						PlayerController controller = player2Obj.GetComponent<PlayerController> ();
						//		controller.Index = player2;
				}
		}
	
		// Update is called once per frame
		void Update ()
		{

		}
}
