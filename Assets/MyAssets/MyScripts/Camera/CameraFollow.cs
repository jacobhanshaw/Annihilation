using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraFollow : MonoBehaviour
{
		private float xSmooth = 3.0f;
		private float ySmooth = 3.0f;
		private float minCameraSize = 9.0f;
		private float standardZoomAmount = 0.1f;
		private float frameEdgeGap = 0.1f;
		private float zoomEdgeGap = 0.25f;
		private List<GameObject> players;
		private List<POIScript> poiScripts;
		
		private List<GameObject> pois;
		
		private List<POIScript> panToPois;
		
		private float lastPanTime;
		private float panSpeed = 15.0f;
		private float panZoomLevel = 15.0f;
		
		private bool zoomOutNecessary = false;
		private bool zoomInNecessary = true;

		void Start ()
		{
				GameObject[] playersArray = GameObject.FindGameObjectsWithTag ("Player");
				
				Debug.Log ("Players: " + playersArray.Length.ToString ());
				players = new List<GameObject> (playersArray);
				
				LayerMask initialPlayerLayer = 0; 
				LayerMask otherPlayerLayer = 0;
				LayerMask initialInteractLayer = 0;
				LayerMask otherInteractLayer = 0;
				LayerMask combinedInteractLayer = 0;
				LayerMask combinedVersusLayer = 0;
				
				int testNum;
				string layer = LayerMask.LayerToName (gameObject.layer);
				string numOne = layer [layer.Length - 2].ToString ();
				bool validNumOne = int.TryParse (numOne, out testNum);
				if (validNumOne) {
						initialPlayerLayer = LayerMask.NameToLayer ("Player" + numOne);
						initialInteractLayer = LayerMask.NameToLayer ("Interact" + numOne);
				}
				string numTwo = layer [layer.Length - 1].ToString ();
				bool validNumTwo = int.TryParse (numTwo, out testNum);
				if (validNumTwo) {
						otherPlayerLayer = LayerMask.NameToLayer ("Player" + numTwo);
						otherInteractLayer = LayerMask.NameToLayer ("Interact" + numTwo);
				}
				
				if (validNumOne && validNumTwo) {
						combinedInteractLayer = LayerMask.NameToLayer ("Interact" + numOne + numTwo);
						combinedVersusLayer = LayerMask.NameToLayer ("Versus" + numOne + numTwo);
				}
					
				for (int i = players.Count-1; i >= 0; --i) {
						if (players [i].layer != initialPlayerLayer && players [i].layer != otherPlayerLayer)
								players.RemoveAt (i);
				}

				poiScripts = new List<POIScript> ();
				panToPois = new List<POIScript> ();
				
				GameObject[] pois = GameObject.FindGameObjectsWithTag ("POI");
				foreach (GameObject poi in pois) {
						if (poi.layer == initialInteractLayer || poi.layer == otherInteractLayer || poi.layer == combinedInteractLayer || poi.layer == combinedVersusLayer) 
								poiScripts.Add (poi.GetComponent<POIScript> ());
				}
		}
		
		void Update ()
		{
				if (panToPois.Count > 0) {
						if (!GameLogic.Instance.paused)
								GameLogic.Instance.PausePressed (0);
						
						float deltaTime = Time.realtimeSinceStartup - lastPanTime;
				
						float usedPanSpeed = panToPois [0].overridePanSpeed == -1.0f ? panSpeed : panToPois [0].overridePanSpeed;
						float usedPanZoomLevel = panToPois [0].overridePanZoomLevel == -1.0f ? panZoomLevel : panToPois [0].overridePanZoomLevel;
						gameObject.camera.orthographicSize = usedPanZoomLevel;
				
						Vector2 newPositionVec2 = Vector2.MoveTowards (gameObject.transform.position, panToPois [0].gameObject.transform.position, deltaTime * usedPanSpeed);
						Vector3 newPosition = new Vector3 (newPositionVec2.x, newPositionVec2.y, panToPois [0].transform.position.z);
																		
						if (newPosition == panToPois [0].gameObject.transform.position) {
								panToPois [0].forcePan = false;
								panToPois [0].panOnEnter = false;
								panToPois.RemoveAt (0);
						}
						newPosition.z = gameObject.transform.position.z;
						gameObject.transform.position = newPosition;
				} else {
						if (GameLogic.Instance.paused)
								GameLogic.Instance.PausePressed (0);
						
						TrackPlayers ();
				}
				
				lastPanTime = Time.realtimeSinceStartup;
		}
		
		void TrackPlayers ()
		{	
				zoomOutNecessary = false;
				zoomInNecessary = true;

				Vector3 playerCentroid = CentroidFromList (players);
				
				pois = new List<GameObject> ();
				for (int i = poiScripts.Count-1; i >= 0; --i) {
						POIScript poiScript = poiScripts [i];
						if (!poiScript)
								poiScripts.Remove (poiScript);
						else {
								if (poiScript.forcePan)
										panToPois.Add (poiScript);	
								if (poiScript.activeScript) {
										if (poiScript.panOnEnter)
												panToPois.Add (poiScript);
										pois.Add (poiScript.gameObject);
										poiScript.shown = true;
								}
						}
				}
				
				Vector3 centroid;
				if (pois.Count > 0) {
						Vector3 poiCentroid = CentroidFromList (pois);
						centroid = (playerCentroid + poiCentroid) / 2.0f;
				} else
						centroid = playerCentroid;
				
				if (zoomOutNecessary)
						gameObject.camera.orthographicSize += standardZoomAmount * 2.0f;
				else if (zoomInNecessary) {
						gameObject.camera.orthographicSize -= standardZoomAmount;
						if (gameObject.camera.orthographicSize < minCameraSize)
								gameObject.camera.orthographicSize = minCameraSize;
				}
												
				transform.position = new Vector3 (Mathf.Lerp (transform.position.x, centroid.x, Time.deltaTime * xSmooth), 
										  Mathf.Lerp (transform.position.y, centroid.y, Time.deltaTime * ySmooth), 
										  transform.position.z);
										  
				if (panToPois.Count > 0)
						panToPois.Sort (ByPriorityIndex);
		}
		
		int ByPriorityIndex (POIScript leftPOI, POIScript rightPOI)
		{
				return leftPOI.priorityIndex - rightPOI.priorityIndex;
		}
		
		Vector3 CentroidFromList (List<GameObject> trackedItems)
		{
				Vector3 centroid = new Vector3 (0, 0, 0);
		
				foreach (GameObject item in trackedItems) {
						centroid += item.transform.position;
						Vector2 viewportLocation = gameObject.camera.WorldToViewportPoint (item.transform.position); 
			
						if (viewportLocation.x < frameEdgeGap ||
								viewportLocation.y < frameEdgeGap ||
								viewportLocation.x > 1 - frameEdgeGap ||
								viewportLocation.y > 1 - frameEdgeGap)
								zoomOutNecessary = true;
						else if (!(viewportLocation.x > zoomEdgeGap &&
								viewportLocation.y > zoomEdgeGap &&
								viewportLocation.x < 1 - zoomEdgeGap &&
								viewportLocation.y < 1 - zoomEdgeGap))
								zoomInNecessary = false;
				}
		
				centroid /= trackedItems.Count;
		
				return centroid;
		}
		
}