using UnityEngine;
using System.Collections;

namespace Running.Game
{
	public class Settings : MonoBehaviour
	{
		public const int LaneCount = 3;
		public int MaxPoolCount = 10;
		public float LaneWidth;
		public float Speed;
		public float HideDepth = -5;
		public Vector3 CameraPosition;
		public Vector3 CameraRotation;
		public Vector3 PlayerPosition;
		
		private static Settings _instance;

		public static Settings Instance
		{
			get 
			{
				return _instance;
			}
		}

		private void Reset()
		{
			_instance = this;
		}

		private void Awake()
		{
			_instance = this;
		}
	}	
}
