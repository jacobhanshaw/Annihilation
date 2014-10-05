using UnityEngine;
using System.Collections;

public class ColorChangeEvent : ItemEvent
{

		private ColorPlatform colorPlatfromScript;

		private int changeFrom;
		public  int changeTo;

		new void Start ()
		{
				base.Start ();

				colorPlatfromScript = item.GetComponent<ColorPlatform> ();

				if (HelperFunction.Instance.PlayersInLayer (item.layer, 1) != -1)
						changeFrom = 0;
				else
						changeFrom = HelperFunction.Instance.PlayersInLayer (item.layer, 2);
		}

		public override void Trigger (bool trigger)
		{
				if (trigger)
						colorPlatfromScript.ChangeColorToBlockPlayer (changeTo);
				else
						colorPlatfromScript.ChangeColorToBlockPlayer (changeFrom);
		}
}
