using Running.Game;
using UnityEditor;
using UnityEngine;

namespace Running.Editor
{
	[CustomEditor(typeof(Element))]
	public class ElementEditor : UnityEditor.Editor
	{
		private Element _target;

		private void OnEnable()
		{
			_target = (Element)target;
		}

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			GUILayout.BeginHorizontal("");
			for (int i = 0; i < Settings.LaneCount; ++i)
			{
				if (GUILayout.Button("Lane " + (i + 1)))
				{
					var floor = Mathf.FloorToInt((float)Settings.LaneCount/2);
					float z = 0.0f;
					if (i + 1 < floor)
					{
						z = -(floor - i) * Settings.Instance.LaneWidth;
					}
					else
					{
						z = (i - floor) * Settings.Instance.LaneWidth;
					}
					_target.transform.localPosition = new Vector3(_target.transform.localPosition.x, _target.transform.localPosition.y, z);
				}
			}
			GUILayout.EndHorizontal();
		}
	}	
}
