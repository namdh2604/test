using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FindMainCameraScript : MonoBehaviour 
{
	private Camera _mainCam = null;

	void Awake()
	{
		_mainCam = Camera.main;
		Canvas myCanvas = gameObject.GetComponent<Canvas>();
		myCanvas.worldCamera = _mainCam;
		Debug.Log(myCanvas.worldCamera.name);

		MatchScreen();
	}

	void MatchScreen ()
	{
		CanvasScaler myScalar = gameObject.GetComponent<CanvasScaler>();
		Vector2 screenRes = new Vector2(Screen.width,Screen.height);
		myScalar.referenceResolution = screenRes;
	}
}
