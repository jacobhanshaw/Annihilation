using UnityEngine;
using System.Collections;

public class Controller : Singleton<Controller>
{
		//Move Delegates
		public delegate void Move (bool[] movesArray);
		public static Move MoveListeners; //check if null

		//Attack Delegates
		public delegate void Attack (bool[] movesArray,bool[] attackModsArray);
		public static Attack ThrowListeners; //check if null
		public static Attack BurstListeners; //check if null
		public static Attack ShootListeners; //check if null
		public static Attack SwordListeners; //check if null
		public static Attack PushListeners; //check if null
		public static Attack PullListeners; //check if null

		//Power Delegates
		public delegate void Power ();
		public static Power FlyListeners; //check if null
		public static Power SloMoListeners; //check if null
		public static Power DoubleSpeedListeners; //check if null
		public static Power PauseListeners; //check if null
		public static Power SizeUpListeners; //check if null
		public static Power SizeDownListeners; //check if null

		public enum ActionIndex
		{
				//Move/Direction - Hold
				Up,
				Left,
				Right,
				Down,
				Jump,
				//Attack - KeyDown
				Throw,
				Burst,
				Shoot,
				Sword,
				Push,
				Pull,
				//Attack Mod - Hold
				TMod,
				XMod,
				Zap,
				Minion,
				AutoAim,
				//Poweeeeeeeeeeeeeeeeeeeeer - KeyDown
				Fly,
				SlowMo,
				DoubleSpeed,
				Pause,
				Randomize,
				SizeUp,
				SizeDown,
				Length
		}

		public KeyCode[] keycodes = { KeyCode.Alpha0,KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, 
		KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, 
		KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K,KeyCode.L, KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, 
		KeyCode.Q, KeyCode.R, KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W,KeyCode.X, KeyCode.Y, KeyCode.Z };

		void Start ()
		{
				HelperFunction.Instance.Assert (keycodes.Length >= ActionIndex.Length);

				Input.ResetInputAxes ();
		}
	
		void Update ()
		{
				Bool[] directionArray = new bool[Input.GetKey (keycodes [ActionIndex.Up]), Input.GetKey (keycodes [ActionIndex.Left]),
		                            		 Input.GetKey (keycodes [ActionIndex.Right]), Input.GetKey (keycodes [ActionIndex.Down]),
		                                   	 Input.GetKey (keycodes [ActionIndex.Jump])];
				
				if (MoveListeners != null)
						MoveListeners (directionArray);
				
				bool[] attackModArray = new bool[Input.GetKey (ActionIndex.TMod), Input.GetKey (ActionIndex.XMod), Input.GetKey (ActionIndex.Zap),
		                                 Input.GetKey (ActionIndex.Minion), Input.GetKey (ActionIndex.AutoAim)];

				if (ThrowListeners != null && Input.GetKeyDown (keycodes [ActionIndex.Throw]))
						ThrowListeners (directionArray, attackModArray);

				if (ShootListeners != null && Input.GetKeyDown (keycodes [ActionIndex.Shoot]))
						ShootListeners (directionArray, attackModArray);

				if (SwordListeners != null && Input.GetKeyDown (keycodes [ActionIndex.Sword]))
						SwordListeners (directionArray, attackModArray);

				if (PushListeners != null && Input.GetKeyDown (keycodes [ActionIndex.Push]))
						PushListeners (directionArray, attackModArray);

				if (PullListeners != null && Input.GetKeyDown (keycodes [ActionIndex.Pull]))
						PullListeners (directionArray, attackModArray);

				if (FlyListeners != null && Input.GetKeyDown (keycodes [ActionIndex.Fly]))
						FlyListeners ();

				if (SloMoListeners != null && Input.GetKeyDown (keycodes [ActionIndex.SlowMo]))
						SloMoListeners ();

				if (DoubleSpeedListeners != null && Input.GetKeyDown (keycodes [ActionIndex.DoubleSpeed]))
						DoubleSpeedListeners ();

				if (PauseListeners != null && Input.GetKeyDown (keycodes [ActionIndex.Pause]))
						PauseListeners ();
	
				if (SizeUpListeners != null && Input.GetKeyDown (keycodes [ActionIndex.SizeUp]))
						SizeUpListeners ();

				if (SizeDownListeners != null && Input.GetKeyDown (keycodes [ActionIndex.SizeDown]))
						SizeDownListeners ();

				if (Input.GetKeyDown (keycodes [ActionIndex.Randomize]))
						HelperFunction.Instance.RandomizeArray (keycodes);

		}
}
