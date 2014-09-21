using UnityEngine;
using System.Collections;

public class HelperFunction : Singleton<HelperFunction>
{

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

}