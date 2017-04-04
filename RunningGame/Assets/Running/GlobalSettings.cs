using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Running
{
	public class GlobalSettings : MonoBehaviour
	{
		public int MenuIndex;
		public int CurrentScore;
		public int CurrentCoin;

		private static GlobalSettings _instance;

		public static GlobalSettings Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = FindObjectOfType<GlobalSettings>();
				}
				return _instance;
			}
		}

		public void Awake()
		{
			DontDestroyOnLoad(this);
		}

		public void Start()
		{
			SceneManager.LoadScene("MenuScene");
		}
	}	
}