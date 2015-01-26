using UnityEngine;
using System.Collections;

public class Controller : Singleton<Controller>
{
		private float lastTimeScale = 1.0f;

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


		public enum MotionIndex
		{
				//Move/Direction - Hold
				Up,
				Left,
				Right,
				Down,
				Jump,
				Length
		}

		public enum AttackIndex
		{
				//Attack - KeyDown
				Throw,
				Burst,
				Shoot,
				Sword,
				Push,
				Pull,
				Length
		}
		public enum AttackModIndex
		{
				//Attack Mod - Hold
				TMod,
				XMod,
				Zap,
				Minion,
				AutoAim,
				Length
		}
		public enum PowerIndex
		{
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
				HelperFunction.Instance.Assert (keycodes.Length >= ((int)MotionIndex.Length + (int)AttackIndex.Length + (int)AttackModIndex.Length + (int)PowerIndex.Length));

				Input.ResetInputAxes ();
		}
	
		void Update ()
		{
				int offset = 0;
				bool[] directionArray = new bool[] {
						Input.GetKey (keycodes [(int)MotionIndex.Up]),
						Input.GetKey (keycodes [(int)MotionIndex.Left]), 
						Input.GetKey (keycodes [(int)MotionIndex.Right]),
						Input.GetKey (keycodes [(int)MotionIndex.Down]),
						Input.GetKey (keycodes [(int)MotionIndex.Jump])
				};
				
				if (MoveListeners != null)
						MoveListeners (directionArray);
				
				offset += (int)MotionIndex.Length;

				bool[] attackModArray = new bool[] {
			Input.GetKey (keycodes [offset + (int)AttackModIndex.TMod]), 
			Input.GetKey (keycodes [offset + (int)AttackModIndex.XMod]), 
			                        Input.GetKey (keycodes [offset + (int)AttackModIndex.Zap]),
			                        Input.GetKey (keycodes [offset + (int)AttackModIndex.Minion]), 
			                        Input.GetKey (keycodes [offset + (int)AttackModIndex.AutoAim])
		};

				offset += (int)AttackModIndex.Length;

				if (ThrowListeners != null && Input.GetKeyDown (keycodes [offset + (int)AttackIndex.Throw]))
						ThrowListeners (directionArray, attackModArray);

				if (ShootListeners != null && Input.GetKeyDown (keycodes [offset + (int)AttackIndex.Shoot]))
						ShootListeners (directionArray, attackModArray);

				if (SwordListeners != null && Input.GetKeyDown (keycodes [offset + (int)AttackIndex.Sword]))
						SwordListeners (directionArray, attackModArray);

				if (PushListeners != null && Input.GetKeyDown (keycodes [offset + (int)AttackIndex.Push]))
						PushListeners (directionArray, attackModArray);

				if (PullListeners != null && Input.GetKeyDown (keycodes [offset + (int)AttackIndex.Pull]))
						PullListeners (directionArray, attackModArray);

				offset += (int)AttackIndex.Length;

				if (FlyListeners != null && Input.GetKeyDown (keycodes [offset + (int)PowerIndex.Fly]))
						FlyListeners ();

				if (Input.GetKeyDown (keycodes [offset + (int)PowerIndex.SlowMo])) {
						Debug.Log ("Slo Mo");
						setTimeScale (0.5f);
						if (SloMoListeners != null)
								SloMoListeners ();
				}

				if (Input.GetKeyDown (keycodes [offset + (int)PowerIndex.DoubleSpeed])) {
						Debug.Log ("Fast");
						setTimeScale (2.0f);	
						if (DoubleSpeedListeners != null)
								DoubleSpeedListeners ();
				}

				if (Input.GetKeyDown (keycodes [offset + (int)PowerIndex.Pause])) {
						Debug.Log ("Pause");
						setTimeScale (0.0f);
						if (PauseListeners != null)
								PauseListeners ();
				}
	
				if (SizeUpListeners != null && Input.GetKeyDown (keycodes [offset + (int)PowerIndex.SizeUp]))
						SizeUpListeners ();

				if (SizeDownListeners != null && Input.GetKeyDown (keycodes [offset + (int)PowerIndex.SizeDown]))
						SizeDownListeners ();

				//	if (Input.GetKeyDown (keycodes [offset + (int)PowerIndex.Randomize]))
				//			HelperFunction.Instance.RandomizeArray (keycodes);

		}

		private void setTimeScale (float newTimeScale)
		{
				if (Time.timeScale != newTimeScale)
						Time.timeScale = newTimeScale;
				else
						Time.timeScale = 1.0f;
		}

}
