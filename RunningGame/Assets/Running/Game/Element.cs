using UnityEngine;
using System.Collections;

namespace Running.Game
{
	public enum ElementType
	{
		Obstacle,
		Collectible,
		SceneryItem
	}

	public class Element : MonoBehaviour
	{
		public ElementType ElementType;
	}	
}
