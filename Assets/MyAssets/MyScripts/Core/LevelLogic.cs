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
		[HideInInspector]
		public float
				playerScale = 1.0f;
		
		//List of all player controllers ERROR CURRENTLY UNUSED
		private PlayerController playerController;
		
		//Spawn variables
		public GameObject spawnLocation;
	
		//GUI
		GuiCameraLogic guiCameraLogic;

		void Awake ()
		{
		}
		
		void Start ()
		{

				GameObject playerObj = (GameObject)Instantiate (playerPrefab, spawnLocation.transform.position, Quaternion.identity);
				playerController = playerObj.GetComponent<PlayerController> ();
				playerController.spawnLocation = spawnLocation;
				PlayerController.AchievementReceivedListeners += GameObject.Find ("GuiCamera").GetComponent<GuiCameraLogic> ().UpdateScore;
				PlayerController.HealthChangedListeners += GameObject.Find ("GuiCamera").GetComponent<GuiCameraLogic> ().UpdateHealth;
		}
		
		
		void OnDestroy ()
		{
				//Potentially remove listeners here
		}
}
