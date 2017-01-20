using UnityEngine;
using System.Collections.Generic;
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
			_poolGameObject = new GameObject("Pools");
			_onPlatformHidden = onPlatformHidden;
		}

		public void Load(string xmlString)
		{
			XDocument xDoc = XDocument.Parse(xmlString);
			var root = xDoc.Root;

			var platformElements = root.DescendantsAndSelf("Platform");
			foreach (var platformElement in platformElements)
			{
				var name = platformElement.Attribute("Name").Value;
				var prefab = platformElement.Attribute("Prefab").Value;
				var gameObject = GameObject.Instantiate(Asset.Instance.GetPrefabByName(prefab));
				gameObject.name = name;
				gameObject.transform.parent = _poolGameObject.transform;
				var platform = gameObject.GetComponent<Platform>();
				platform.OnHidden = _onPlatformHidden;

				var hasStart = platformElement.Attribute("Start") != null;
				var hasEnd = platformElement.Attribute("End") != null;

				if (hasStart || hasEnd)
				{
					var pointsGameObject = new GameObject("Points");
					pointsGameObject.transform.parent = gameObject.transform;

					if (hasStart)
					{
						var start = platformElement.Attribute("Start").Value.ToVector3();
						var startGameObject = new GameObject("Start");
						startGameObject.transform.parent = pointsGameObject.transform;
						startGameObject.transform.localPosition = start;
						platform.Start = startGameObject.transform;
					}

					if (hasEnd)
					{
						var end = platformElement.Attribute("End").Value.ToVector3();
						var endGameObject = new GameObject("End");
						endGameObject.transform.parent = pointsGameObject.transform;
						endGameObject.transform.localPosition = end;
						platform.End = endGameObject.transform;
					}
				}
				
				if (platformElement.HasElements)
				{
					var childElements = platformElement.Elements("Element");
					if (childElements != null)
					{
						var elementsGameObject = new GameObject("Elements");
						elementsGameObject.transform.parent = gameObject.transform;

						foreach (var childElement in childElements)
						{
							var childName = childElement.Attribute("Name").Value;
							var childPosition = childElement.Attribute("Position").Value.ToVector3();
							var childRotation = childElement.Attribute("Rotation").Value.ToQuaternion();

							var childGameObject = GameObject.Instantiate(Asset.Instance.GetPrefabByName(childName));
							childGameObject.name = childName;
							childGameObject.transform.parent = elementsGameObject.transform;
							childGameObject.transform.localPosition = childPosition;
							childGameObject.transform.localRotation = childRotation;
						}
					}
				}

				var rotation = platformElement.Attribute("Rotation").Value.ToQuaternion();
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
