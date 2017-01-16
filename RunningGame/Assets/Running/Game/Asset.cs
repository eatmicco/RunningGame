using UnityEngine;
using System.Linq;

namespace Running.Game
{
	public class Asset : MonoBehaviour
	{
		private static Asset _instance;

		public GameObject[] Prefabs;

		public static Asset Instance
		{
			get { return _instance; }
		}

		private void Awake()
		{
			_instance = this;
		}

		public GameObject GetPrefabByName(string name)
		{
			return Prefabs.FirstOrDefault(p => p.name == name);
		}
	}	
}
