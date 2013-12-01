using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
		public float xMargin = 1f;		// Distance in the x axis the player can move before the camera follows.
		public float yMargin = 1f;		// Distance in the y axis the player can move before the camera follows.
		public float xSmooth = 8f;		// How smoothly the camera catches up with it's target movement in the x axis.
		public float ySmooth = 8f;		// How smoothly the camera catches up with it's target movement in the y axis.
		public Vector2 maxXAndY;		// The maximum x and y coordinates the camera can have.
		public Vector2 minXAndY;		// The minimum x and y coordinates the camera can have.

		private float standardZPos = -10.0f;
		private float minCameraSize = 9.0f;
		private float standardZoomAmount = 0.1f;
		private float frameEdgeGap = 0.1f;
		private float zoomEdgeGap = 0.25f;
		private GameObject[] players;		// Reference to the player's transform.
		//private Transform player;

		void Start () //Awake
		{
				// Setting up the reference.
				players = GameObject.FindGameObjectsWithTag ("Player");
				//player = players [0].transform;
		}
		
		void FixedUpdate ()
		{
				TrackPlayers ();
		}

/*
		bool CheckXMargin ()
		{
				// Returns true if the distance between the camera and the player in the x axis is greater than the x margin.
				return Mathf.Abs (transform.position.x - player.position.x) > xMargin;
		}


		bool CheckYMargin ()
		{
				// Returns true if the distance between the camera and the player in the y axis is greater than the y margin.
				return Mathf.Abs (transform.position.y - player.position.y) > yMargin;
		}

		void TrackPlayer ()
		{
				// By default the target x and y coordinates of the camera are it's current x and y coordinates.
				float targetX = transform.position.x;
				float targetY = transform.position.y;

				// If the player has moved beyond the x margin...
				if (CheckXMargin ())
			// ... the target x coordinate should be a Lerp between the camera's current x position and the player's current x position.
						targetX = Mathf.Lerp (transform.position.x, player.position.x, xSmooth * Time.deltaTime);

				// If the player has moved beyond the y margin...
				if (CheckYMargin ())
			// ... the target y coordinate should be a Lerp between the camera's current y position and the player's current y position.
						targetY = Mathf.Lerp (transform.position.y, player.position.y, ySmooth * Time.deltaTime);

				Debug.Log ("XMargin: " + CheckXMargin () + " YMargin: " + CheckYMargin ());
				
				// The target x and y coordinates should not be larger than the maximum or smaller than the minimum.
				targetX = Mathf.Clamp (targetX, minXAndY.x, maxXAndY.x);
				targetY = Mathf.Clamp (targetY, minXAndY.y, maxXAndY.y);

				// Set the camera's position to the target position with the same z component.
				transform.position = new Vector3 (targetX, targetY, transform.position.z);
		}
		*/
		
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
