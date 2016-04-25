using UnityEngine;
using System.Collections;

public class CameraSwitchScript : MonoBehaviour
{

	void Awake() 
	{
		GameObject[] cameras = GameObject.FindGameObjectsWithTag("MainCamera");

		if(cameras.Length > 1)
		{
			gameObject.SetActive(false);
		}
	}

}
