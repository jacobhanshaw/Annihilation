using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{

		// Use this for initialization
		void Start ()
		{
				gameObject.GetComponentInChildren<EnemyBounce> ().killDelegate = Kill;
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}

		void Kill ()
		{
				Destroy (gameObject);
		}
}
