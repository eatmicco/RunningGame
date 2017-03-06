using UnityEngine;

namespace Running.Game
{
	public class Settings : MonoBehaviour
	{
		public const int LaneCount = 3;
		public int MaxPoolCount = 10;
		public float LaneWidth;
		public float PlatformSpeed;
		public float HideDepth = -5f;
		public float SwipeDistance = 50f;
		public float PlayerSpeed = 0.1f;
		public float JumpingVelocity = 10f;
		public float Gravity = 1f;
		public float SlidingTime = 1f;
		public float ScoreMultiplier = 10f;
		public Vector3 CameraPosition;
		public Vector3 CameraRotation;
		public Vector3 PlayerPosition;
		
		private static Settings _instance;

		public static Settings Instance
		{
			get 
			{
				if (_instance == null)
				{
					_instance = FindObjectOfType<Settings>();
				}
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
