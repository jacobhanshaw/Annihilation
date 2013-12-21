using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
		private float xSmooth = 8f;
		private float ySmooth = 8f;
		private float standardZPos = -10.0f;
		private float minCameraSize = 9.0f;
		private float standardZoomAmount = 0.1f;
		private float frameEdgeGap = 0.1f;
		private float zoomEdgeGap = 0.25f;
		private GameObject[] players;

		void Start ()
		{
				players = GameObject.FindGameObjectsWithTag ("Player");
		}
		
		void FixedUpdate ()
		{
				TrackPlayers ();
		}
		
		void TrackPlayers ()
		{				
				Vector3 centroid = new Vector3 (0, 0, 0);
		
				bool zoomOutNecessary = false;
				bool zoomInNecessary = true;
				foreach (GameObject player in players) {
						centroid += player.transform.position;
						Vector2 viewportLocation = gameObject.camera.WorldToViewportPoint (player.transform.position); 
			
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
			
				centroid /= players.Length;
				
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
}
