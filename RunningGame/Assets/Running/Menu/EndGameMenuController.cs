using UnityEngine;
using System.Collections;
using Running;
using Running.Menu;
using UnityEngine.UI;

namespace Runnning.Menu
{
	public class EndGameMenuController : ChildMenuController
	{
		public Button HomeButton;
		public Text ScoreText;

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
		}

		private void HandleHomeButtonClicked()
		{
			MenuController.OpenPanel(0);
		}
	}	
}