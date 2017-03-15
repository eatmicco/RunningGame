using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Running.Game
{
	public class PlatformManager
	{
		private class PlatformPool
		{
			public int LatestIndex;
			public List<GameObject> Pool;
		}

		private readonly Dictionary<string, PlatformPool> _platformPools = new Dictionary<string, PlatformPool>();
		private readonly List<int> _latestPoolIndexes = new List<int>();
		private GameObject _poolGameObject;
		private System.Action _onPlatformHidden;

		public void Initialize(System.Action onPlatformHidden)
		{
			_poolGameObject = new GameObject(CommonName.PlatformPoolsGameObjectName);
			_onPlatformHidden = onPlatformHidden;
		}

		public void Load(string xmlString)
		{
			XDocument xDoc = XDocument.Parse(xmlString);
			var root = xDoc.Root;

			var platformElements = root.DescendantsAndSelf(CommonName.PlatformXElementName);
			foreach (var platformElement in platformElements)
			{
				var name = platformElement.Attribute(CommonName.NameAttributeName).Value;
				var prefab = platformElement.Attribute(CommonName.PrefabAttributeName).Value;
				var gameObject = GameObject.Instantiate(Asset.Instance.GetPrefabByName(prefab));
				gameObject.name = name;
				gameObject.transform.parent = _poolGameObject.transform;
				var platform = gameObject.GetComponent<Platform>();
				platform.OnHidden = _onPlatformHidden;

				var hasStart = platformElement.Attribute(CommonName.StartAttributeName) != null;
				var hasEnd = platformElement.Attribute(CommonName.EndAttributeName) != null;

				if (hasStart || hasEnd)
				{
					var pointsGameObject = new GameObject(CommonName.PointsGameObjectName);
					pointsGameObject.transform.parent = gameObject.transform;

					if (hasStart)
					{
						var start = platformElement.Attribute(CommonName.StartAttributeName).Value.ToVector3();
						var startGameObject = new GameObject(CommonName.StartAttributeName);
						startGameObject.transform.parent = pointsGameObject.transform;
						startGameObject.transform.localPosition = start;
						platform.Start = startGameObject.transform;
					}

					if (hasEnd)
					{
						var end = platformElement.Attribute(CommonName.EndAttributeName).Value.ToVector3();
						var endGameObject = new GameObject(CommonName.EndAttributeName);
						endGameObject.transform.parent = pointsGameObject.transform;
						endGameObject.transform.localPosition = end;
						platform.End = endGameObject.transform;
					}
				}
				
				if (platformElement.HasElements)
				{
					var childElements = platformElement.Elements(CommonName.ElementXElementName);
					if (childElements != null)
					{
						var elementsGameObject = new GameObject(CommonName.ElementsGameObjectName);
						elementsGameObject.transform.parent = gameObject.transform;

						foreach (var childElement in childElements)
						{
							var childName = childElement.Attribute(CommonName.NameAttributeName).Value;
							var childPrefabName = childElement.Attribute(CommonName.PrefabAttributeName).Value;
							var childPosition = childElement.Attribute(CommonName.PositionAttributeName).Value.ToVector3();
							var childRotation = childElement.Attribute(CommonName.RotationAttributeName).Value.ToQuaternion();

							var childGameObject = GameObject.Instantiate(Asset.Instance.GetPrefabByName(childPrefabName));
							childGameObject.name = childName;
							childGameObject.transform.parent = elementsGameObject.transform;
							childGameObject.transform.localPosition = childPosition;
							childGameObject.transform.localRotation = childRotation;
						}
					}
				}

				var rotation = platformElement.Attribute(CommonName.RotationAttributeName).Value.ToQuaternion();
				gameObject.transform.rotation = rotation;

				GeneratePool(gameObject);
			}
		}

		private void GeneratePool(GameObject gameObject)
		{
			var name = gameObject.name;
			if (!_platformPools.ContainsKey(name))
			{
				_platformPools.Add(name, new PlatformPool
				{
					LatestIndex = -1,
					Pool = new List<GameObject>()
					{
						gameObject
					}
				});
			}
			else
			{
				_platformPools[name].LatestIndex = -1;
				_platformPools[name].Pool.Clear();
				_platformPools[name].Pool.Add(gameObject);
			}

			// Instantiate another gameObjects
			var maxPoolCount = Settings.Instance.MaxPoolCount;
			for (var i = 0; i < maxPoolCount - 1; ++i)
			{
				var duplicate = Object.Instantiate(gameObject);
				duplicate.name = name;
				var platform = duplicate.GetComponent<Platform>();
				platform.OnHidden = _onPlatformHidden;
				duplicate.transform.parent = _poolGameObject.transform;
				_platformPools[name].Pool.Add(duplicate);
				duplicate.SetActive(false);
			}

			gameObject.SetActive(false);
		}

		public GameObject GetPlatform(string name)
		{
			if (_platformPools.ContainsKey(name))
			{
				if (_platformPools[name].LatestIndex < Settings.Instance.MaxPoolCount - 1)
				{
					_platformPools[name].LatestIndex += 1;
				}
				else
				{
					_platformPools[name].LatestIndex = 0;
				}
				return _platformPools[name].Pool[_platformPools[name].LatestIndex];
			}

			return null;
		}

		public GameObject GetRandomPlatform()
		{
			var random = Random.Range(0, _platformPools.Keys.Count);
			var keyName = _platformPools.Keys.ElementAt(random);
			return GetPlatform(keyName);
		}

		public void ReattachToPool(GameObject platformGameObject)
		{
			platformGameObject.transform.parent = _poolGameObject.transform;
			platformGameObject.transform.localPosition = Vector3.zero;
			platformGameObject.SetActive(false);
		}

		public void ConnectPlatformToPoint(GameObject platformGameObject, Vector3 point)
		{
			var startPosition = GetStartTransform(platformGameObject).position;
			var diff = point - startPosition;
			platformGameObject.transform.Translate(diff, Space.World);
		}

		public Transform GetStartTransform(GameObject platformGameObject)
		{
			var platform = platformGameObject.GetComponent<Platform>();
			return platform.Start.transform;
		}

		public Transform GetEndTransform(GameObject platformGameObject)
		{
			var platform = platformGameObject.GetComponent<Platform>();
			return platform.End.transform;
		}
	}	
}
