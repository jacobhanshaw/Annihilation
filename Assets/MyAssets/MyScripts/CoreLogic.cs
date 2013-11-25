using UnityEngine;
using System.Collections;

public class CoreLogic : MonoBehaviour
{

		bool player2Exists = false;
	
		public GameObject playerPrefab;
		public Material green;
		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		void Update ()
		{
				OuyaSDK.OuyaPlayer player2 = OuyaSDK.OuyaPlayer.player2;
				if (player2 != null && !player2Exists) {
						player2Exists = true;
						GameObject player2Obj = (GameObject)Instantiate (playerPrefab, new Vector3 (0, 10, 0), Quaternion.identity);
						player2Obj.renderer.material = green;
						PlayerController controller = player2Obj.GetComponent<PlayerController> ();
						controller.Index = player2;
				}

		}
}
