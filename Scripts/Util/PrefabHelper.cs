using System.Diagnostics;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Voltage.Witches.Utilities
{
	public static class PrefabHelper
	{
		public static Object Instantiate(Object target)
		{
#if UNITY_EDITOR
			return PrefabUtility.InstantiatePrefab(target);
#else
			UnityEngine.Debug.Log("Instantiating: " + target.name);
			return GameObject.Instantiate(target);
#endif
		}
	}
}

