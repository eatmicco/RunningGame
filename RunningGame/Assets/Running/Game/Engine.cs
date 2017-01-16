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
			_platformManager.Initialize();
			foreach (var textAsset in PlatformTextAssets)
			{
				_platformManager.Load(textAsset.text);

				yield return null;
			}

			// Try to arrange 3 platforms
			Vector3 endPosition = Vector3.zero;
			for (int i = 0; i < 3; ++i)
			{
				var gameObject = _platformManager.GetPlatform("PlatformDesign01");
				gameObject.transform.parent = transform;
				_platformManager.ConnectPlatformToPoint(gameObject, endPosition);
				gameObject.SetActive(true);
				endPosition = _platformManager.GetEndPosition(gameObject);
			}
		}
	}	
}
