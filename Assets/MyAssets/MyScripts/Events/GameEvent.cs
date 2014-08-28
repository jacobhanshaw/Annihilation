using UnityEngine;
using System.Collections;

abstract public class GameEvent : MonoBehaviour
{
		abstract public void Trigger (bool trigger);
}