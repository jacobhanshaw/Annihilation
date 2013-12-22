using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraFollow : MonoBehaviour
{
		private float xSmooth = 3.0f;
		private float ySmooth = 3.0f;
		private float standardZPos = -10.0f;
		private float minCameraSize = 9.0f;
		private float standardZoomAmount = 0.1f;
		private float frameEdgeGap = 0.1f;
		private float zoomEdgeGap = 0.25f;
		private List<GameObject> players;
		private List<POIScript> poiScripts;
		
		private bool zoomOutNecessary = false;
		private bool zoomInNecessary = true;

		void Start ()
		{
				GameObject[] playersArray = GameObject.FindGameObjectsWithTag ("Player");
				players = new List<GameObject> (playersArray);
				poiScripts = new List<POIScript> ();
				GameObject[] pois = GameObject.FindGameObjectsWithTag ("POI");
				foreach (GameObject poi in pois)
						poiScripts.Add (poi.GetComponent<POIScript> ());
		}
		
		void FixedUpdate ()
		{
				TrackPlayers ();
		}
		
		void TrackPlayers ()
		{				
		
				zoomOutNecessary = false;
				zoomInNecessary = true;

				Vector3 playerCentroid = CentroidFromList (players);
				
				List<GameObject> pois = new List<GameObject> ();
				foreach (POIScript poiScript in poiScripts) {
						if (Vector3.SqrMagnitude (playerCentroid - poiScript.gameObject.transform.position) <= poiScript.relevantRangeSquared) {
								pois.Add (poiScript.gameObject);
								poiScript.shown = true;
						}
				}
				
				Vector3 centroid;
				if (pois.Count > 0) {
						Vector3 poiCentroid = CentroidFromList (pois);
						centroid = (playerCentroid + poiCentroid) / 2.0f;
				} else
						centroid = playerCentroid;
				
				centroid.z = standardZPos;
				
				if (zoomOutNecessary)
						gameObject.camera.orthographicSize += standardZoomAmount;
				else if (zoomInNecessary) {
						gameObject.camera.orthographicSize -= standardZoomAmount;
						if (gameObject.camera.orthographicSize < minCameraSize)
								gameObject.camera.orthographicSize = minCameraSize;
				}
												
				transform.position = new Vector3 (Mathf.Lerp (transform.position.x, centroid.x, Time.deltaTime * xSmooth), 
										  Mathf.Lerp (transform.position.y, centroid.y, Time.deltaTime * ySmooth), 
										  transform.position.z);
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
