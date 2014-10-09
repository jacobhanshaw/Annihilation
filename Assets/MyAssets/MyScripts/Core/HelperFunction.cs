using UnityEngine;
using System.Collections;

public class HelperFunction : Singleton<HelperFunction>
{

		public int GetPartner (int player)
		{
				if (player == 1)
						return 2;
				else if (player == 3)
						return 3;
				else if (player == 2)
						return 1;

				return 4;
		}

		public int GetPair (int player)
		{
				if (player == 1 || player == 2)
						return 12;

				return 34;
		}

		public int PlayersInLayer (LayerMask layer, int index)
		{
				int result;
				bool validNum;
				string layerString = LayerMask.LayerToName (layer);
				if (index == 1)
						validNum = int.TryParse (layerString [layerString.Length - 2].ToString (), out result);
				else
						validNum = int.TryParse (layerString [layerString.Length - 1].ToString (), out result);	

				if (!validNum)
						return -1;
		
				return result;
		}

		public Vector2 BottomLeftOfBoxCollider2D (Vector2 position, BoxCollider2D collider)
		{
				return position + collider.center - collider.size / 2.0f;
		}

		public Vector2 TopRightOfBoxCollider2D (Vector2 position, BoxCollider2D collider)
		{
				return position + collider.center + collider.size / 2.0f;
		}

		public GameObject FindBasedOnLayer (string objectName, int layer, bool inverted)
		{
				GameObject potentialItem = GameObject.Find (objectName);
				bool exists = potentialItem != null;
				bool inSameLayer = exists && (layer & (1 << potentialItem.layer)) != 0;

				if (inSameLayer && inverted)
						return GameObject.Find (objectName.Replace ("(Clone)", ""));
				else if (inSameLayer || (exists && potentialItem.layer == LayerMask.NameToLayer ("Default")) || inverted)
						return potentialItem;
			
				return GameObject.Find (objectName + "(Clone)");
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