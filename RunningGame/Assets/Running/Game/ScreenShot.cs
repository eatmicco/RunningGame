using UnityEngine;
using System.Collections;

namespace Running.Game
{
	[RequireComponent(typeof(Camera))]
	public class ScreenShot : MonoBehaviour
	{
		public bool Grab;

		private void OnPostRender()
		{
			if (Grab)
			{
				if (GlobalSettings.Instance.ScreenshotTexture == null)
				{
					GlobalSettings.Instance.ScreenshotTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
				}

				GlobalSettings.Instance.ScreenshotTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
				GlobalSettings.Instance.ScreenshotTexture.Apply();
			}
		}
	}	
}