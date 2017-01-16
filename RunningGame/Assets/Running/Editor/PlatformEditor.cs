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
			XDocument xDoc = new XDocument(new XElement("Platforms"));
			var root = xDoc.Root;
			var platforms = GameObject.FindObjectsOfType<Platform>();
			if (platforms != null)
			{
				foreach (var platform in platforms)
				{
					var transform = platform.transform;
					var xElement = new XElement("Platform");
					xElement.SetAttributeValue("Name", transform.name);
					xElement.SetAttributeValue("Start", platform.Start.transform.position);
					xElement.SetAttributeValue("End", platform.End.transform.position);

					if (transform.childCount > 0)
					{
						var elementsTransform = transform.FindChild("Elements");
						var elements = elementsTransform.gameObject.GetComponentsInChildren<Element>();
						foreach (var element in elements)
						{
							var childXElement = new XElement("Element");
							childXElement.SetAttributeValue("Name", element.name);
							childXElement.SetAttributeValue("Position", element.transform.localPosition.ToFormattedString());
							childXElement.SetAttributeValue("Rotation", element.transform.localRotation.ToFormattedString());
							childXElement.SetAttributeValue("Type", element.ElementType);

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
