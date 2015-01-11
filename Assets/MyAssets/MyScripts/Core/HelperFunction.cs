using UnityEngine;
using System.Collections;

public class HelperFunction : Singleton<HelperFunction>
{

		public Vector2 BottomLeftOfBoxCollider2D (Vector2 position, BoxCollider2D collider)
		{
				return position + collider.center - collider.size / 2.0f;
		}

		public Vector2 TopRightOfBoxCollider2D (Vector2 position, BoxCollider2D collider)
		{
				return position + collider.center + collider.size / 2.0f;
		}
	
		public T CopyComponent<T> (T original, GameObject destination) where T : Component
		{
				System.Type type = original.GetType ();
				Component copy = destination.AddComponent (type);
				System.Reflection.FieldInfo[] fields = type.GetFields ();
				foreach (System.Reflection.FieldInfo field in fields) {
						field.SetValue (copy, field.GetValue (original));
				}
				return copy as T;
		}

		public void Assert (bool condition)
		{
				if (!condition) 
						throw new System.Exception ();
		}
}