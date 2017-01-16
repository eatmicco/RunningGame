using UnityEngine;
using System.Collections;

namespace Running.Game
{
	public class Engine : MonoBehaviour
	{
		private readonly PlatformManager _platformManager = new PlatformManager();

		public TextAsset[] PlatformTextAssets;

		private void Start()
		{
			StartCoroutine(Initialize());
		}

		private IEnumerator Initialize()
		{
			foreach (var textAsset in PlatformTextAssets)
			{
				_platformManager.Load(textAsset.text);

				yield return null;
			}
		}
	}	
}
