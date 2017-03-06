using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Running.Game
{
	public class GuiManager : MonoBehaviour
	{
		public Engine Engine;
		public Text ScoreText;

		private void Awake()
		{
			InitializeCallback();
		}

		private void InitializeCallback()
		{
			if (Engine != null)
			{
				Engine.EngineLateUpdateFinished += HandleOnEngineLateUpdateFinished;
			}
		}

		private void HandleOnEngineLateUpdateFinished(Engine engine)
		{
			ScoreText.text = engine.Score.ToString();
		}
	}	
}
