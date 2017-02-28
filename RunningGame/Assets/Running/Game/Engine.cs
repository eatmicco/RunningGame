using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Running.Game
{
	public class Engine : MonoBehaviour
	{
		private readonly PlatformManager _platformManager = new PlatformManager();
		private readonly List<GameObject> _platformGeneratedList = new List<GameObject>();

		public Camera MainCamera;
		public GameObject PlayerPrefab;
		public TextAsset[] PlatformTextAssets;

		private Player _player;

		private void Start()
		{
			StartCoroutine(Initialize());
		}

		private IEnumerator Initialize()
		{
			InitializePlayer();
			InitializeCamera();

			yield return InitializePlatforms();
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
			_player.PlayerCollided += PlayerOnPlayerCollided;
		}

		private void PlayerOnPlayerCollided(Collider collider)
		{
			var element = collider.gameObject.GetComponent<Element>();
			if (element != null && element.ElementType == ElementType.Obstacle)
			{
				MovePlatforms(false);
				_player.Freeze();
			}
		}

		private IEnumerator InitializePlatforms()
		{
			_platformManager.Initialize(OnPlatformHidden);

			yield return LoadPlatformPools();

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

		private void ArrangePlatforms(int count, Vector3 endPosition, Transform endTransform = null)
		{
			for (int i = 0; i < count; ++i)
			{
				var gameObject = _platformManager.GetRandomPlatform();
				gameObject.transform.parent = transform;
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
			_platformGeneratedList[0].SetActive(false);
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
