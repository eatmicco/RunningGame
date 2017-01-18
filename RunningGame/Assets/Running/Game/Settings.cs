using UnityEngine;
using System.Collections;

namespace Running.Game
{
	public class Settings : MonoBehaviour
	{
		public const int LaneCount = 3;
		public int MaxPoolCount = 5;
		public float LaneWidth;
		public float Speed;
		
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
