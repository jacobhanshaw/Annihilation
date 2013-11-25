using UnityEngine;
using System.Collections;

public class StickyWall : MonoBehaviour
{

		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}
	
		void OnTriggerEnter2D (Collider2D other)
		{
				other.transform.parent = gameObject.transform;
		}
	
		void OnTriggerExit2D (Collider2D other)
		{
				other.transform.parent = null;
		}
	
}
