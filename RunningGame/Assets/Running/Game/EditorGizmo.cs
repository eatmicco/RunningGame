using UnityEngine;
using System.Collections;

namespace Running.Editor
{
	public class EditorGizmo : MonoBehaviour
	{
		private void OnDrawGizmos()
		{
			// Draw Red Arrow
			Gizmos.color = Color.red;
			Gizmos.DrawLine(new Vector3(0, 0, -2), Vector3.zero);
			Gizmos.DrawLine(new Vector3(0.5f, 0, -1), Vector3.zero);
			Gizmos.DrawLine(new Vector3(-0.5f, 0, -1), Vector3.zero);
		}
	}
}