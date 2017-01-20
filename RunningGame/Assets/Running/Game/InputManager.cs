using UnityEngine;
using System.Collections;

namespace Running.Game
{
	public class InputManager
	{
		private enum TouchState
		{
			Start,
			End
		}

		public enum SwipeDirection
		{
			None,
			Right,
			Left,
			Top,
			Bottom
		}

		private const float SwipeDistance = 50.0f;
		private Vector3 _touchPosition;
		private TouchState _touchState;
		private SwipeDirection _swipeDirection;

		public SwipeDirection GetSwipeDirection()
		{
			var temp = _swipeDirection;
			_swipeDirection = SwipeDirection.None;
			return temp;
		}

		public void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				_touchPosition = Input.mousePosition;
				_touchState = TouchState.Start;
			}
			else if (Input.GetMouseButtonUp(0))
			{
				_touchPosition = Vector3.zero;
				_touchState = TouchState.End;
			}
			else if (_touchState == TouchState.Start)
			{
				if (Input.GetMouseButton(0))
				{
					var swipeHorizontal = Input.mousePosition.x - _touchPosition.x;
					var swipeVertical = Input.mousePosition.y - _touchPosition.y;
					if (swipeHorizontal > SwipeDistance)
					{
						_swipeDirection = SwipeDirection.Right;
					}
					else if (swipeHorizontal < -SwipeDistance)
					{
						_swipeDirection = SwipeDirection.Left;
					}
					else if (swipeVertical > SwipeDistance)
					{
						_swipeDirection = SwipeDirection.Top;
					}
					else if (swipeVertical < -SwipeDistance)
					{
						_swipeDirection = SwipeDirection.Bottom;
					}

					if (_swipeDirection != SwipeDirection.None)
					{
						_touchPosition = Input.mousePosition;
					}
				}
			}
		}
	}	
}