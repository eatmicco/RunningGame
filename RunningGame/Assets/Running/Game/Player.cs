using UnityEngine;

namespace Running.Game
{
	public class Player : MonoBehaviour
	{
		private readonly InputManager _inputManager = new InputManager();

		private void Update()
		{
			_inputManager.Update();

			switch (_inputManager.GetSwipeDirection())
			{
				case InputManager.SwipeDirection.Right:
					Debug.Log("Move Player to Right!");
					break;
				case InputManager.SwipeDirection.Left:
					Debug.Log("Move Player to Left!");
					break;
				case InputManager.SwipeDirection.Top:
					Debug.Log("Player Jumps!");
					break;
				case InputManager.SwipeDirection.Bottom:
					Debug.Log("Player Slides!");
					break;
			}			
		}
	}
}