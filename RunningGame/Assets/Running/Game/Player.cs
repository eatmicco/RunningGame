using UnityEngine;

namespace Running.Game
{
	public class Player : MonoBehaviour
	{
		private readonly InputManager _inputManager = new InputManager();
		private Vector3 _destination;
		private float _currentVelocity;
		private int _currentLane = 1;
		private bool _onMoving;
		private bool _onJumping;

		private void Update()
		{
			_inputManager.Update();

			var swipeDirection = _inputManager.GetSwipeDirection();
			if (!_onMoving)
			{
				SwipeHorizontal(swipeDirection);
			}
			else
			{
				Moving();
			}

			if (!_onJumping)
			{
				SwipeVertical(swipeDirection);
			}
			else
			{
				Jumping();
			}
		}


		private void SwipeHorizontal(InputManager.SwipeDirection direction)
		{
			switch (direction)
			{
				case InputManager.SwipeDirection.Right:
					if (_currentLane < Settings.LaneCount - 1)
					{
						_onMoving = true;
						_destination = transform.position;
						_destination.x += Settings.Instance.LaneWidth;
					}
					break;
				case InputManager.SwipeDirection.Left:
					if (_currentLane > 0)
					{
						_onMoving = true;
						_destination = transform.position;
						_destination.x -= Settings.Instance.LaneWidth;
					}
					break;
			}
		}

		private void SwipeVertical(InputManager.SwipeDirection swipeDirection)
		{
			switch (swipeDirection)
			{
				case InputManager.SwipeDirection.Top:
					Debug.Log("Jumping");
					_onJumping = true;
					_currentVelocity = Settings.Instance.JumpingVelocity;
					break;
				case InputManager.SwipeDirection.Bottom:
					Debug.Log("Player Slides!");
					break;
			}
		}

		private void Moving()
		{
			var direction = _destination.x - transform.position.x;
			var speed = Settings.Instance.PlayerSpeed * Time.deltaTime;
			if (Mathf.Abs(direction) <= speed)
			{
				var pos = transform.position;
				pos.x = _destination.x;
				transform.position = pos;
				_onMoving = false;
				_currentLane = direction < 0 ? _currentLane - 1 : _currentLane + 1;
			}
			else
			{
				transform.Translate(Vector3.right * direction * speed);
			}
		}

		private void Jumping()
		{
			var speed = _currentVelocity * Time.deltaTime;
			var threshold = Settings.Instance.JumpingVelocity * Time.deltaTime;
			if (transform.position.y >= -threshold && transform.position.y <= threshold && _currentVelocity < 0)
			{
				var pos = transform.position;
				pos.y = 0;
				transform.position = pos;
				_onJumping = false;
			}
			else
			{
				transform.Translate(Vector3.up * speed, Space.World);
			}
			_currentVelocity -= Settings.Instance.Gravity;
		}
	}
}