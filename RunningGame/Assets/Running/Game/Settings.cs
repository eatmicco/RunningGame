using UnityEngine;
using System.Collections;

namespace Running.Game
{
	public class Settings : MonoBehaviour
	{
		public const int LaneCount = 3;
		public int MaxPoolCount = 5;

		private static Settings _instance;

		public float LaneWidth;

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
