using System;
using UnityEngine;

namespace Voltage.Common.Unity
{
	public class UnitySingleton : MonoBehaviour
	{
		private static UnitySingleton instance;
		public static UnitySingleton Instance 
		{ 
			get 
			{ 
				if (!HasInstance())
				{
					GameObject go = new GameObject();
					go.name = "UnitySingleton";
					go.AddComponent<UnitySingleton>();
				}
				
				if (!HasInstance())
				{
					throw new Exception("UnitySingleton: Failed to create instance");
				}
				
				return instance; 
			} 
		}	

		private void Awake()
		{
			if (HasInstance())
			{
				Debug.LogWarning("UnitySingleton: already exists, self-destructing!");
				Destroy(gameObject);
			}
			else
			{
				AssignInstance();
			}		
		}

		private void AssignInstance()
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		
		public static bool HasInstance()
		{
			return instance != null;
		}
	}
}

