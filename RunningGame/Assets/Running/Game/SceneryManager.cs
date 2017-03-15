using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Running.Game
{
	public class SceneryManager
	{
		private class SceneryPool
		{
			public int LatestIndex;
			public List<GameObject> Pool;
		}

		private readonly Dictionary<string, SceneryPool> _sceneryPools = new Dictionary<string, SceneryPool>();
		private readonly List<int> _latestPoolIndexes = new List<int>(); 
		private GameObject _poolGameObject;

		public void Initialize()
		{
			_poolGameObject = new GameObject(CommonName.SceneryPoolsGameObjectName);
		}

		public void Load(string xmlString)
		{
			XDocument xDoc = XDocument.Parse(xmlString);
			var root = xDoc.Root;

			var sceneryElements = root.DescendantsAndSelf(CommonName.SceneryXElementName);
			foreach (var sceneryElement in sceneryElements)
			{
				var name = sceneryElement.Attribute(CommonName.NameAttributeName).Value;
				var gameObject = new GameObject(name);
				gameObject.transform.parent = _poolGameObject.transform;

				if (sceneryElement.HasElements)
				{
					var childElements = sceneryElement.Elements(CommonName.ElementXElementName);
					if (childElements != null)
					{
						foreach (var childElement in childElements)
						{
							var childName = childElement.Attribute(CommonName.NameAttributeName).Value;
							var childPrefabName = childElement.Attribute(CommonName.PrefabAttributeName).Value;
							var childPosition = childElement.Attribute(CommonName.PositionAttributeName).Value.ToVector3();
							var childRotation = childElement.Attribute(CommonName.RotationAttributeName).Value.ToQuaternion();

							var childGameObject = GameObject.Instantiate(Asset.Instance.GetPrefabByName(childPrefabName));
							childGameObject.name = childName;
							childGameObject.transform.parent = gameObject.transform;
							childGameObject.transform.localPosition = childPosition;
							childGameObject.transform.localRotation = childRotation;
						}
					}
				}

				var rotation = sceneryElement.Attribute(CommonName.RotationAttributeName).Value.ToQuaternion();
				gameObject.transform.rotation = rotation;

				GeneratePool(gameObject);
			}
		}

		private void GeneratePool(GameObject gameObject)
		{
			var name = gameObject.name;
			if (!_sceneryPools.ContainsKey(name))
			{
				_sceneryPools.Add(name, new SceneryPool
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
				_sceneryPools[name].LatestIndex = -1;
				_sceneryPools[name].Pool.Clear();
				_sceneryPools[name].Pool.Add(gameObject);
			}

			// Instantiate another gameObjects
			var maxPoolCount = Settings.Instance.MaxPoolCount;
			for (var i = 0; i < maxPoolCount - 1; ++i)
			{
				var duplicate = Object.Instantiate(gameObject);
				duplicate.name = name;
				duplicate.transform.parent = _poolGameObject.transform;
				_sceneryPools[name].Pool.Add(duplicate);
				duplicate.SetActive(false);
			}

			gameObject.SetActive(false);
		}

		public GameObject GetScenery(string name)
		{
			if (_sceneryPools.ContainsKey(name))
			{
				if (_sceneryPools[name].LatestIndex < Settings.Instance.MaxPoolCount - 1)
				{
					_sceneryPools[name].LatestIndex += 1;
				}
				else
				{
					_sceneryPools[name].LatestIndex = 0;
				}
				return _sceneryPools[name].Pool[_sceneryPools[name].LatestIndex];
			}

			return null;
		}

		public GameObject GetRandomScenery()
		{
			var random = Random.Range(0, _sceneryPools.Keys.Count);
			var keyName = _sceneryPools.Keys.ElementAt(random);
			return GetScenery(keyName);
		}

		public void ReattachToPool(GameObject sceneryGameObject)
		{
			sceneryGameObject.transform.parent = _poolGameObject.transform;
			sceneryGameObject.transform.localPosition = Vector3.zero;
			sceneryGameObject.SetActive(false);
		}
	}
}