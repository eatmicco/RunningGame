using UnityEngine;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Running.Game
{
	public class PlatformManager
	{
		private readonly Dictionary<string, List<GameObject>> _platformPool = new Dictionary<string, List<GameObject>>(); 

		public void Load(string xmlString)
		{
			XDocument xDoc = XDocument.Parse(xmlString);
			var root = xDoc.Root;

			var platformElements = root.DescendantsAndSelf("Platform");
			foreach (var platformElement in platformElements)
			{
				var name = platformElement.Attribute("Name").Value;

				var gameObject = GameObject.Instantiate(Asset.Instance.GetPrefabByName(name));
				gameObject.name = name;

				if (platformElement.HasElements)
				{
					var childElements = platformElement.Elements("Element");
					if (childElements != null)
					{
						foreach (var childElement in childElements)
						{
							var childName = childElement.Attribute("Name").Value;
							var position = childElement.Attribute("Position").Value.ToVector3();
							var rotation = childElement.Attribute("Rotation").Value.ToQuaternion();

							var childGameObject = GameObject.Instantiate(Asset.Instance.GetPrefabByName(childName));
							childGameObject.name = childName;
							childGameObject.transform.parent = gameObject.transform;
							childGameObject.transform.localPosition = position;
							childGameObject.transform.localRotation = rotation;
						}
					}
				}

				GeneratePool(gameObject);
			}
		}

		private void GeneratePool(GameObject gameObject)
		{
			var name = gameObject.name;
			if (!_platformPool.ContainsKey(name))
			{
				_platformPool.Add(name, new List<GameObject>()
					{
						gameObject
					});
			}
			else
			{
				_platformPool[name].Clear();
				_platformPool[name].Add(gameObject);
			}

			// Instantiate another gameObjects
			var maxPoolCount = Settings.Instance.MaxPoolCount;
			for (var i = 0; i < maxPoolCount - 1; ++i)
			{
				var duplicate = Object.Instantiate(gameObject);
				duplicate.name = name;
				_platformPool[name].Add(duplicate);
				duplicate.SetActive(false);
			}

			gameObject.SetActive(false);
		}
	}	
}
