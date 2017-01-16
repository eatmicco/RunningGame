using System;
using UnityEngine;
using System.Collections;

namespace Running
{
	public static class CommonExtension
	{
		public static string ToFormattedString(this Vector3 vector3)
		{
			return vector3.x + ", " + vector3.y + ", " + vector3.z;
		}

		public static string ToFormattedString(this Quaternion quaternion)
		{
			return quaternion.eulerAngles.x + ", " + quaternion.eulerAngles.y + ", " + quaternion.eulerAngles.z;
		}

		public static Vector3 ToVector3(this string vector3String)
		{
			var splits = vector3String.Trim().Split(',');

			if (splits.Length < 3)
			{
				throw new Exception("Couldn't Parse String to Vector3.");
			}

			var x = float.Parse(splits[0].Trim());
			var y = float.Parse(splits[1].Trim());
			var z = float.Parse(splits[2].Trim());

			return new Vector3(x, y, z);
		}

		public static Quaternion ToQuaternion(this string quaternionString)
		{
			var splits = quaternionString.Trim().Split(',');

			if (splits.Length < 3)
			{
				throw new Exception("Couldn't Parse String to Quaternion.");
			}

			var x = float.Parse(splits[0].Trim());
			var y = float.Parse(splits[1].Trim());
			var z = float.Parse(splits[2].Trim());

			return Quaternion.Euler(x, y, z);
		}
	}	
}
