using UnityEngine;

namespace Running.Game
{
	public class Platform : MonoBehaviour
	{
		public Transform Start;
		public Transform End;
		public Transform EndConnectedPlatform;
		public System.Action OnHidden;

		private void Update()
		{
			transform.Translate(Vector3.back * Settings.Instance.Speed * Time.deltaTime, Space.World);

			if (End.position.z < Settings.Instance.HideDepth)
			{
				if (OnHidden != null)
				{
					OnHidden.Invoke();
				}
			}

			if (EndConnectedPlatform != null)
			{
				var diff = EndConnectedPlatform.position - Start.position;
				if (diff.sqrMagnitude > 0)
				{
					transform.Translate(diff, Space.World);
				}
			}
		}
	}
}
