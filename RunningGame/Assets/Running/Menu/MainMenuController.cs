using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Running.Menu
{
	public class MainMenuController : ChildMenuController
	{
		public Button StartButton;

		private void Awake()
		{
			if (StartButton != null)
			{
				StartButton.onClick.AddListener(HandleStartButtonClicked);
			}
		}

		private void HandleStartButtonClicked()
		{
			SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
		}
	}	
}
