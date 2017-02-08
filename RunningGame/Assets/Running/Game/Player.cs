using UnityEngine;

namespace Running.Game
{
	// TODO : Add player sliding
	// TODO : Add scoring system
	// TODO : Add basic gui (replacable)
	[RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
	public class Player : MonoBehaviour
	{
		private enum VerticalState
		{
			Jumping,
			Falling,
			None
		}

		private enum OnTopOf
		{
			Platform,
			Obstacle
		}

		private readonly InputManager _inputManager = new InputManager();
		private Vector3 _destination;
		private float _currentVelocity;
		private int _currentLane = 1;
		private bool _onMoving;
		private VerticalState _verticalState;
		private bool _freezed;
		private OnTopOf _onTopOf;

		public event System.Action<Collider> PlayerCollided;
		public Transform BottomRaySource;

		private void Update()
		{
			if (_freezed)
			{
				return;
			}

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

			if (_verticalState == VerticalState.None)
			{
				SwipeVertical(swipeDirection);
			}
			else
			{
				VerticalMovement();
			}

			RayBottom();
		}

		private void OnTriggerEnter(Collider other)
		{
			if (PlayerCollided != null)
			{
				PlayerCollided.Invoke(other);
			}

			_freezed = true;
		}

		private void SwipeHorizontal(InputManager.SwipeDirection direction)
		{
			switch (direction)
			{
				case InputManager.SwipeDirection.Right:
					if (_currentLane < Settings.LaneCount - 1)
					{
						_onMoving = true;
						_destination.x = transform.position.x + Settings.Instance.LaneWidth;
						
					}
					break;
				case InputManager.SwipeDirection.Left:
					if (_currentLane > 0)
					{
						_onMoving = true;
						_destination.x = transform.position.x - Settings.Instance.LaneWidth;
					}
					break;
			}
		}

		private void SwipeVertical(InputManager.SwipeDirection swipeDirection)
		{
			switch (swipeDirection)
			{
				case InputManager.SwipeDirection.Top:
					_verticalState = VerticalState.Jumping;
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

		private void VerticalMovement()
		{
			var speed = _currentVelocity * Time.deltaTime;
			var threshold = Settings.Instance.JumpingVelocity * Time.deltaTime;
			if (transform.position.y >= _destination.y - threshold && transform.position.y <= _destination.y + threshold && _currentVelocity < 0)
			{
				var pos = transform.position;
				pos.y = _destination.y;
				transform.position = pos;
				_verticalState = VerticalState.None;
			}
			else
			{
				if (speed < 0)
				{
					_verticalState = VerticalState.Falling;
				}
				transform.Translate(Vector3.up * speed, Space.World);
			}
			_currentVelocity -= Settings.Instance.Gravity;
		}

		private void RayBottom()
		{
			var ray = new Ray(BottomRaySource.position, Vector3.down);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				var platform = hit.transform.gameObject.GetComponentInChildren<Platform>();
				if (platform != null)
				{
					if (_onTopOf == OnTopOf.Obstacle && _verticalState == VerticalState.None)
					{
						_verticalState = VerticalState.Falling;
						_currentVelocity = 0;
						_destination.y = 0;
					}
					_onTopOf = OnTopOf.Platform;
					return;
				}

				var element = hit.transform.gameObject.GetComponentInChildren<Element>();
				if (element != null && element.ElementType == ElementType.Obstacle)
				{
					_onTopOf = OnTopOf.Obstacle;
					if (_verticalState == VerticalState.Jumping | _verticalState == VerticalState.Falling)
					{
						_destination.y = hit.point.y;
					}
					return;
				}
			}
		}
	}
}