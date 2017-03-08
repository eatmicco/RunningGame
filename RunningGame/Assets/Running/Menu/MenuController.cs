using UnityEngine;
using System.Collections;

namespace Running.Menu
{
	public class MenuController : MonoBehaviour
	{
		public GameObject[] MenuPanels;

		public void OpenPanel(int index)
		{
			HideAllMenuPanels();
			MenuPanels[index].SetActive(true);
		}

		private void Awake()
		{
			InitializeControllers();
		}

		private void Start()
		{
			OpenPanel(GlobalSettings.Instance.MenuIndex);
		}

		private void InitializeControllers()
		{
			foreach (var menuGameObject in MenuPanels)
			{
				var childMenuController = menuGameObject.GetComponent<ChildMenuController>();
				childMenuController.MenuController = this;
			}
		}

		private void HideAllMenuPanels()
		{
			foreach (var menuPanel in MenuPanels)
			{
				menuPanel.SetActive(false);
			}
		}
	}
}