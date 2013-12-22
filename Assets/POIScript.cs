using UnityEngine;
using System.Collections;

public class POIScript : MonoBehaviour
{
		public bool  shown;
		public float relevantRange;
		public float relevantDuration;
		public float relevantRangeSquared;

		void Start ()
		{
				if (relevantRange != -1.0f)
						relevantRangeSquared = relevantRange * relevantRange;
				else 
						relevantRangeSquared = -1.0f;
		}

		void Update ()
		{
				if (shown && relevantDuration != -1.0f) {
						relevantDuration -= Time.deltaTime;
						if (relevantDuration <= 0)
								Destroy (gameObject);
				}
		}
}
