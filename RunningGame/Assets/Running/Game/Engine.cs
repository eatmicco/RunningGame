using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Running.Game
{
	// TODO : Add scoring system
	// TODO : Add basic gui (replacable)
	public class Engine : MonoBehaviour
	{
		private readonly PlatformManager _platformManager = new PlatformManager();
		private readonly List<GameObject> _platformGeneratedList = new List<GameObject>();
		private readonly List<GameObject> _sceneryGeneratedList = new List<GameObject>();
		private readonly SceneryManager _sceneryManager = new SceneryManager();

		public Action<Engine> EngineLateUpdateFinished;
		public Camera MainCamera;
		public GameObject PlayerPrefab;
		public TextAsset[] PlatformTextAssets;
		public TextAsset[] SceneryTextAssets;

		private Player _player;
		private int _score;
		private bool _freeze;

		public int Score
		{
			get { return _score; }
		}

		private void Start()
		{
			StartCoroutine(Initialize());
		}

		private void LateUpdate()
		{
			if (_freeze) return;

			_score += (int)(Settings.Instance.ScoreMultiplier * Time.deltaTime);
			
			if (EngineLateUpdateFinished != null)
			{
				EngineLateUpdateFinished.Invoke(this);
			}
		}

		private IEnumerator Initialize()
		{
			InitializePlayer();
			InitializeCamera();

			yield return InitializePlatforms();

			InitializeVariables();
		}

		private void InitializeCamera()
		{
			if (MainCamera != null)
			{
				MainCamera.transform.position = Settings.Instance.CameraPosition;
				MainCamera.transform.rotation = Quaternion.Euler(Settings.Instance.CameraRotation);
			}
			else
			{
				throw new Exception("No MainCamera detected.");
			}
		}

		private void InitializePlayer()
		{
			var playerGameObject = Instantiate(PlayerPrefab, Settings.Instance.PlayerPosition, Quaternion.identity) as GameObject;
			_player = playerGameObject.GetComponent<Player>();
			_player.PlayerCollided += HandleOnPlayerCollided;
		}

		private void InitializeVariables()
		{
			_score = 0;
		}

		private void HandleOnPlayerCollided()
		{
			MovePlatforms(false);

			_freeze = true;

			GlobalSettings.Instance.MenuIndex = 1;
			GlobalSettings.Instance.CurrentScore = _score;
			SceneManager.LoadScene("MenuScene");
		}

		private IEnumerator InitializePlatforms()
		{
			_platformManager.Initialize(OnPlatformHidden);
			_sceneryManager.Initialize();

			yield return LoadPlatformPools();
			yield return LoadSceneryPools();

			// Try to arrange 5 platforms
			_platformGeneratedList.Clear();
			ArrangePlatforms(5, Vector3.zero);
		}

		private IEnumerator LoadPlatformPools()
		{
			foreach (var textAsset in PlatformTextAssets)
			{
				_platformManager.Load(textAsset.text);

				yield return null;
			}
		}

		private IEnumerator LoadSceneryPools()
		{
			foreach (var textAsset in SceneryTextAssets)
			{
				_sceneryManager.Load(textAsset.text);

				yield return null;
			}
		}

		private void ArrangePlatforms(int count, Vector3 endPosition, Transform endTransform = null)
		{
			for (int i = 0; i < count; ++i)
			{
				var gameObject = _platformManager.GetRandomPlatform();
				gameObject.transform.parent = transform;

				var sceneryGameObject = _sceneryManager.GetRandomScenery();
				sceneryGameObject.transform.parent = gameObject.transform;
				sceneryGameObject.transform.localPosition = Vector3.zero;
				sceneryGameObject.SetActive(true);
				_sceneryGeneratedList.Add(sceneryGameObject);

				var platform = gameObject.GetComponent<Platform>();
				platform.EndConnectedPlatform = endTransform;
				_platformManager.ConnectPlatformToPoint(gameObject, endPosition);
				gameObject.SetActive(true);
				endTransform = _platformManager.GetEndTransform(gameObject);
				endPosition = endTransform.position;
				_platformGeneratedList.Add(gameObject);
			}
		}

		private void OnPlatformHidden()
		{
			var platform = _platformGeneratedList[1].GetComponent<Platform>();
			platform.EndConnectedPlatform = null;

			_sceneryManager.ReattachToPool(_sceneryGeneratedList[0]);
			_sceneryGeneratedList.RemoveAt(0);

			_platformManager.ReattachToPool(_platformGeneratedList[0]);
			_platformGeneratedList.RemoveAt(0);

			var endTransform = _platformManager.GetEndTransform(_platformGeneratedList[_platformGeneratedList.Count - 1]);
			ArrangePlatforms(1, endTransform.position, endTransform);
		}

		private void MovePlatforms(bool move)
		{
			foreach (var platformGameObject in _platformGeneratedList)
			{
				var platform = platformGameObject.GetComponent<Platform>();
				platform.Move(move);
			}
		}
	}	
}
