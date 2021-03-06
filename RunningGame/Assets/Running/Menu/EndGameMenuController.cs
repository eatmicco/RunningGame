﻿using UnityEngine;
using Running;
using Running.Menu;
using UnityEngine.UI;

namespace Runnning.Menu
{
	public class EndGameMenuController : ChildMenuController
	{
		public Image BackgroundImage;
		public Button HomeButton;
		public Text ScoreText;
		public Text CoinText;

		private void Awake()
		{
			if (HomeButton != null)
			{
				HomeButton.onClick.AddListener(HandleHomeButtonClicked);
			}

			if (ScoreText != null)
			{
				ScoreText.text = GlobalSettings.Instance.CurrentScore.ToString();
			}

			if (CoinText != null)
			{
				CoinText.text = GlobalSettings.Instance.CurrentCoin.ToString();
			}
		}

		private void Start()
		{
			BackgroundImage.material = new Material(Shader.Find("Unlit/Texture"));
			BackgroundImage.material.mainTexture = GlobalSettings.Instance.ScreenshotTexture;
		}

		private void HandleHomeButtonClicked()
		{
			MenuController.OpenPanel(0);
		}
	}	
}