using System.Collections;
using UnityEngine;

namespace Running.Game
{
	[RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
	public class Player : MonoBehaviour
	{
		private enum VerticalState
		{
			Jumping,
			Falling,
			Sliding,
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
		private float _currentSlidingTime;

		public event System.Action PlayerCollided;
		public Transform BottomRaySource;
		public Animator PlayerAnimator;

		private void Freeze()
		{
			_freezed = true;
			PlayerAnimator.Stop();
		}

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
			var element = other.gameObject.GetComponent<Element>();
			if (element != null && element.ElementType == ElementType.Obstacle)
			{
				if (PlayerCollided != null)
				{
					PlayerCollided.Invoke();
				}
				Freeze();
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
					_verticalState = VerticalState.Sliding;
					_currentSlidingTime = 0;
					StartCoroutine(SlidePlayer(true));
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
			if (_verticalState != VerticalState.Sliding)
			{
				var speed = _currentVelocity*Time.deltaTime;
				var threshold = Settings.Instance.JumpingVelocity*Time.deltaTime;
				if (transform.position.y >= _destination.y - threshold && transform.position.y <= _destination.y + threshold &&
				    _currentVelocity < 0)
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
					transform.Translate(Vector3.up*speed, Space.World);
				}
				_currentVelocity -= Settings.Instance.Gravity;
			}
			else
			{
				_currentSlidingTime += Time.deltaTime;
				if (_currentSlidingTime >= Settings.Instance.SlidingTime)
				{
					_verticalState = VerticalState.None;
					StartCoroutine(SlidePlayer(false));
				}
			}
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
					if (_verticalState == VerticalState.None && transform.position.y > 0)
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

		private IEnumerator SlidePlayer(bool slide)
		{
			PlayerAnimator.SetTrigger(slide ? "Slide" : "BackUp");

			yield return null;
		}
	}
}