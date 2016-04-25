using UnityEngine;
using System.Collections;

public class SaveCameraScript : MonoBehaviour
{

	void Awake()
	{
		DontDestroyOnLoad(this);
	}
}
