using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnScript : MonoBehaviour
{
		private ParticleSystem system;
		private List<Color> colors;
		private int currentIndex;
	
		private float lastChangeTime;
		private float timeBetweenChanges = 1.5f;
	
		// Use this for initialization
		void Start ()
		{
				system = gameObject.GetComponentInChildren<ParticleSystem> ();
				colors = new List<Color> ();
				colors.Add (new Color (1.0f, 1.0f, 1.0f));
		}
	
		// Update is called once per frame
		void Update ()
		{
				if (Time.time - lastChangeTime > timeBetweenChanges) {
						lastChangeTime = Time.time;
						changeColor ();
				}
		
		}
	
		private void changeColor ()
		{
				system.startColor = colors [currentIndex];
				++currentIndex;
				if (currentIndex >= colors.Count)
						currentIndex = 0;
		}
	
		public void addColor (Color color)
		{
				colors.Add (color);
				currentIndex = colors.Count - 1;
				changeColor ();
		}
		
		public void removeColor (Color color)
		{
				colors.Remove (color);
				if (currentIndex >= colors.Count)
						currentIndex = 0;
				changeColor ();
		}
	
}
