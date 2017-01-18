using UnityEngine;

namespace Running.Game
{
	public class Platform : MonoBehaviour
	{
		public Transform Start;
		public Transform End;
		
		private void Update()
		{
			transform.Translate(Vector3.back * Settings.Instance.Speed * Time.deltaTime, Space.World);
		}
	}
}
