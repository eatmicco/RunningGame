using System.Xml.Linq;
using Running.Game;
using UnityEditor;
using UnityEngine;

namespace Running.Editor
{
	public class PlatformEditor
	{
		[MenuItem("Running/Save Platform")]
		public static void SavePlatform()
		{
			XDocument xDoc = new XDocument(new XElement(CommonName.PlatformsXElementName));
			var root = xDoc.Root;
			var platforms = GameObject.FindObjectsOfType<Platform>();
			if (platforms != null)
			{
				foreach (var platform in platforms)
				{
					var prefabParent = PrefabUtility.GetPrefabParent(platform.gameObject);
					var transform = platform.transform;
					var xElement = new XElement(CommonName.PlatformXElementName);
					xElement.SetAttributeValue(CommonName.NameAttributeName, transform.name);
					xElement.SetAttributeValue(CommonName.PrefabAttributeName, prefabParent.name);
					xElement.SetAttributeValue(CommonName.RotationAttributeName, transform.localRotation.ToFormattedString());
					xElement.SetAttributeValue(CommonName.StartAttributeName, platform.Start.transform.localPosition.ToFormattedString());
					xElement.SetAttributeValue(CommonName.EndAttributeName, platform.End.transform.localPosition.ToFormattedString());

					if (transform.childCount > 0)
					{
						var elementsTransform = transform.FindChild(CommonName.ElementsGameObjectName);
						var elements = elementsTransform.gameObject.GetComponentsInChildren<Element>();
						foreach (var element in elements)
						{
							var elementPrefab = PrefabUtility.GetPrefabParent(element.gameObject);
							var childXElement = new XElement(CommonName.ElementXElementName);
							childXElement.SetAttributeValue(CommonName.NameAttributeName, element.name);
							childXElement.SetAttributeValue(CommonName.PrefabAttributeName, elementPrefab.name);
							childXElement.SetAttributeValue(CommonName.PositionAttributeName, element.transform.localPosition.ToFormattedString());
							childXElement.SetAttributeValue(CommonName.RotationAttributeName, element.transform.localRotation.ToFormattedString());
							childXElement.SetAttributeValue(CommonName.TypeAttributeName, element.ElementType);

							xElement.Add(childXElement);
						}
					}

					root.Add(xElement);
				}

				xDoc.Save("platforms.xml");
			}
		}
	}	
}
