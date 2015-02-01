using UnityEngine;
using System.Collections;

public class Controller : Singleton<Controller>
{
		//Time Consts
		private const float PAUSE_TIMESCALE = 0.01f;
		private const float SLOW_MO_TIMESCALE = 0.25f;
		private const float DOUBLE_SPEED_TIMESCALE = 2.5f;

		//Size Vars
		private float currentSize = 1.0f;
		private const float SIZE_UP_SIZE = 2.0f;
		private const float SIZE_DOWN_SIZE = 0.5f;
/*
 * 1 - Left
 * 2 - Right
 * 3 - Down
 * 4 - Jump
 * 5 - T-Mod
 * 6 - X-Mod
 * I - Fast
 * H - Slow
 * J - Pause
 * C - Shoot
 * L - Size Up
 * M - Size Down
 */

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
		public delegate void SizePower (float size);
		public static Power FlyListeners; //check if null
		public static Power SloMoListeners; //check if null
		public static Power DoubleSpeedListeners; //check if null
		public static Power PauseListeners; //check if null
		public static SizePower SizeUpListeners; //check if null
		public static SizePower SizeDownListeners; //check if null


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

				if (BurstListeners != null && Input.GetKeyDown (keycodes [offset + (int)AttackIndex.Burst]))
						BurstListeners (directionArray, attackModArray);

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
						setTimeScale (SLOW_MO_TIMESCALE);
						if (SloMoListeners != null)
								SloMoListeners ();
				}

				if (Input.GetKeyDown (keycodes [offset + (int)PowerIndex.DoubleSpeed])) {
						setTimeScale (DOUBLE_SPEED_TIMESCALE);	
						if (DoubleSpeedListeners != null)
								DoubleSpeedListeners ();
				}

				if (Input.GetKeyDown (keycodes [offset + (int)PowerIndex.Pause])) {
						setTimeScale (PAUSE_TIMESCALE);
						if (PauseListeners != null)
								PauseListeners ();
				}

				if (SizeUpListeners != null && Input.GetKeyDown (keycodes [offset + (int)PowerIndex.SizeUp])) {
						currentSize = newSize (SIZE_UP_SIZE);
						SizeUpListeners (currentSize);
				}
				if (SizeDownListeners != null && Input.GetKeyDown (keycodes [offset + (int)PowerIndex.SizeDown])) {
						currentSize = newSize (SIZE_DOWN_SIZE);
						SizeDownListeners (currentSize);
				}

				//	if (Input.GetKeyDown (keycodes [offset + (int)PowerIndex.Randomize]))
				//			HelperFunction.Instance.RandomizeArray (keycodes);

		}

		private float newSize (float goalSize)
		{
				if (currentSize != goalSize)
						return goalSize;

				return 1.0f;
		}

		private void setTimeScale (float newTimeScale)
		{
				if (Time.timeScale != newTimeScale)
						Time.timeScale = newTimeScale;
				else
						Time.timeScale = 1.0f;
		}

}
