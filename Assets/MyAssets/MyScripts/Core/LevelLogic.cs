using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelLogic : MonoBehaviour
{

/*	Demo C# Delegation
    public delegate void EnergyTypeClickEventHandler(EnergySelection energyType);
	public static EnergyTypeClickEventHandler EnergyChange; check if null
	EnergyChange(currentEnergyType);
	EnergyTypeClickHandler.EnergyChange += EnergyChange;
	EnergyTypeClickHandler.EnergyChange -= EnergyChange;  */
	
		//Player variables
		public GameObject   playerPrefab;
		public bool[]       playersGrabHeld;
		public bool[]       playersPositionLocked;
		public bool[]       playersJumpDisabled;
		public bool[]       playersJointDisabled;
		public bool[]       playersPickUpDisabled;
		public bool[]       playersSlingShotDisabled;
		public bool[]       playersJointLengthChangeDisabled;
		
		//List of all player controllers ERROR CURRENTLY UNUSED
		private List<PlayerController> playerControllers;
		
		//Spawn variables
		public GameObject[] spawnLocations;
	
		//GUI
		GuiCameraLogic guiCameraLogic;
		
		//Parent Object of All Objects Needed for 1-on-1 or 2-on-1 play (2-on-1 may need work)
		private GameObject versus;
		
		void Awake ()
		{
				versus = GameObject.Find ("Versus");
		}
		
		void Start ()
		{
				GameLogic.Instance.resetForNewRace ();
		
				playerControllers = new List<PlayerController> ();
				
				Debug.Log ("Num players: " + GameLogic.Instance.numPlayers);

				for (int i = 1; i <= GameLogic.Instance.numPlayers; ++i) {
						GameObject playerObj = (GameObject)Instantiate (playerPrefab, spawnLocations [(i - 1) % 2].transform.position, Quaternion.identity);
			
						PlayerController controller = playerObj.GetComponent<PlayerController> ();
						playerControllers.Add (controller);
						controller.spawnLocation = spawnLocations [(i - 1) % 2];
						controller.controllerIndex = GameLogic.Instance.splitControllers ? (OuyaSDK.OuyaPlayer)((i + 1) / 2) : (OuyaSDK.OuyaPlayer)i;
#if OUYA						
						controller.splitController = GameLogic.Instance.splitControllers;
						controller.leftSplit = ((i % 2) != 0);
#endif						
						if (GameLogic.Instance.numPlayers == 2 && GameLogic.Instance.splitScreen) {
								controller.grabHeld = playersGrabHeld [0] || playersGrabHeld [1];
								controller.positionLocked = playersPositionLocked [0] || playersPositionLocked [1];
								controller.jumpDisabled = playersJumpDisabled [0] || playersJumpDisabled [1];
								controller.jointDisabled = playersJointDisabled [0] || playersJointDisabled [1];
								controller.pickUpDisabled = playersPickUpDisabled [0] || playersPickUpDisabled [1];
								controller.slingShotDisabled = playersPickUpDisabled [0] || playersPickUpDisabled [1];
								controller.jointLengthChangeDisabled = playersJointLengthChangeDisabled [0] || playersJointLengthChangeDisabled [1];
						
								if (i == 2) {
										SetLayerRecursively (playerObj, LayerMask.NameToLayer ("Player3"));	
										playerObj.renderer.material.color = GameLogic.Instance.colors [1];
										controller.playerIndex = 3;
										controller.spawnColor = GameLogic.Instance.spawnColors [1];
								}
						} else {						
								SetLayerRecursively (playerObj, LayerMask.NameToLayer ("Player" + i.ToString ()));				
								playerObj.renderer.material.color = GameLogic.Instance.colors [i - 1];
								controller.playerIndex = i;
								controller.spawnColor = GameLogic.Instance.spawnColors [i - 1];
								
								controller.grabHeld = playersGrabHeld [(i - 1) % 2];
								controller.positionLocked = playersPositionLocked [(i - 1) % 2];
								controller.jumpDisabled = playersJumpDisabled [(i - 1) % 2];
								controller.jointDisabled = playersJointDisabled [(i - 1) % 2];
								controller.pickUpDisabled = playersPickUpDisabled [(i - 1) % 2];
								controller.slingShotDisabled = playersSlingShotDisabled [(i - 1) % 2];
								controller.jointLengthChangeDisabled = playersJointLengthChangeDisabled [(i - 1) % 2];
						}
				}
				
				versus.SetActive (GameLogic.Instance.splitScreen && GameLogic.Instance.numPlayers < 4);
				
				if (GameLogic.Instance.splitScreen) {
						GameObject[] allObjectsArray = (GameObject[])FindObjectsOfType (typeof(GameObject));
						List<GameObject> allObjects = new List<GameObject> (allObjectsArray);
						DuplicateGameObjectsInLayerToLayer (allObjects, LayerMask.NameToLayer ("Interact12"), LayerMask.NameToLayer ("Interact34"));
						DuplicateGameObjectsInLayerToLayer (allObjects, LayerMask.NameToLayer ("GUI12"), LayerMask.NameToLayer ("GUI34"));
						DuplicateGameObjectsInLayerToLayer (allObjects, LayerMask.NameToLayer ("Interact1"), LayerMask.NameToLayer ("Interact3"));
						DuplicateGameObjectsInLayerToLayer (allObjects, LayerMask.NameToLayer ("Interact2"), LayerMask.NameToLayer ("Interact4"));
						if (GameLogic.Instance.numPlayers == 2)
								DuplicateGameObjectsInLayerToLayer (allObjects, LayerMask.NameToLayer ("Versus34"), LayerMask.NameToLayer ("Versus12"));
				} else
						PlayerController.AchievementReceivedListeners += GameObject.Find ("GuiCamera").GetComponent<GuiCameraLogic> ().UpdateScore;
	
		}
		
		void DuplicateGameObjectsInLayerToLayer (List<GameObject> gameObjects, int fromLayer, int toLayer)
		{
				for (int i = gameObjects.Count - 1; i >= 0; --i) {
						if (gameObjects [i].name == "ColorWall0")
								Debug.Log ("THINGS: " + LayerMask.LayerToName (fromLayer));

						if (gameObjects [i].transform.parent != null && gameObjects [i].transform.parent.parent == null && !gameObjects [i].transform.parent.gameObject.CompareTag ("Player")) {
								if (gameObjects [i].layer == fromLayer) {
										GameObject newObject = (GameObject)Instantiate (gameObjects [i]);
										SetLayerRecursively (newObject, toLayer);
										Camera cameraComponent = gameObjects [i].GetComponent<Camera> ();
										if (cameraComponent != null) {
												cameraComponent.rect = new Rect (0.0f, 0.5f, 1.0f, 0.5f);
												bool guiCamera = LayerMask.LayerToName (gameObjects [i].layer).Substring (0, 3) == "GUI";
												FixDuplicateCamera (newObject.GetComponent<Camera> (), guiCamera);
												if (guiCamera) {
														PlayerController.AchievementReceivedListeners += newObject.GetComponent<GuiCameraLogic> ().UpdateScore;
														PlayerController.AchievementReceivedListeners += gameObjects [i].GetComponent<GuiCameraLogic> ().UpdateScore;
												}
										}
										gameObjects.RemoveAt (i);
								}
						} else
								gameObjects.RemoveAt (i);
				}
		}
		
		void FixDuplicateCamera (Camera cameraComponent, bool guiCamera)
		{
				cameraComponent.rect = new Rect (0.0f, 0.0f, 1.0f, 0.5f);
				if (guiCamera) {
						int mask = cameraComponent.cullingMask;
						mask &= ~(1 << LayerMask.NameToLayer ("GUI12"));
						mask |= 1 << LayerMask.NameToLayer ("GUI34");
						cameraComponent.cullingMask = mask;
				} else {
						int mask = cameraComponent.cullingMask;
						mask &= ~(1 << LayerMask.NameToLayer ("Player1"));
						mask &= ~(1 << LayerMask.NameToLayer ("Player2"));
						mask &= ~(1 << LayerMask.NameToLayer ("Interact1"));
						mask &= ~(1 << LayerMask.NameToLayer ("Interact2"));
						mask &= ~(1 << LayerMask.NameToLayer ("Interact12"));
						mask &= ~(1 << LayerMask.NameToLayer ("Versus12"));
						mask |= 1 << LayerMask.NameToLayer ("Player3");
						mask |= 1 << LayerMask.NameToLayer ("Player4");
						mask |= 1 << LayerMask.NameToLayer ("Interact3");
						mask |= 1 << LayerMask.NameToLayer ("Interact4");
						mask |= 1 << LayerMask.NameToLayer ("Interact34");
						mask |= 1 << LayerMask.NameToLayer ("Versus34");
						cameraComponent.cullingMask = mask;
				}
		}
		
		void SetLayerRecursively (GameObject aObject, int layer)
		{
				if (aObject == null)
						return;
		
				aObject.layer = layer;
			
				foreach (Transform child in aObject.transform) {
						if (child == null)
								continue;
					
						SetLayerRecursively (child.gameObject, layer);
				}
		}
		
		void OnDestroy ()
		{
				//Potentially remove listeners here
		}
}
