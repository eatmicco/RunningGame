using UnityEngine;
using System.Collections;

namespace Running.Game
{
	public enum ElementType
	{
		Obstacle,
		Scenery
	}

	public class Element : MonoBehaviour
	{
		public ElementType ElementType;
	}	
}
