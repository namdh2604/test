using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEngine.Advertisements;
using System.Collections;

public class IntroMovieScript : MonoBehaviour 
{
	[SerializeField]
	string moviePath = string.Empty;

	void Awake()
	{
//		CreateDataManager();

//		if(Advertisement.isSupported)
//		{
//			Advertisement.allowPrecache = true;
//			Advertisement.Initialize("30844",true);
//			Debug.Log("Ads Initialized");
//		}
//		else
//		{
//			Debug.Log("Platform not supported");
//		}

//		#if (UNITY_IOS || UNITY_IPHONE) && !(UNITY_ANDROID)
//		Advertisement.Initialize(30844);
//		#elif (UNITY_ANDROID) && (!UNITY_IOS || !UNITY_IPHONE)
//		Advertisement.Initialize(30843);
//		#endif
	}

	void Start () 
	{
		moviePath = "QG_Large.mp4";
		StartCoroutine(PlayIntroMovie(moviePath));
	}

	IEnumerator PlayIntroMovie(string videoPath)
	{
		Handheld.PlayFullScreenMovie(videoPath, Color.black, FullScreenMovieControlMode.CancelOnInput, FullScreenMovieScalingMode.AspectFit);
		yield return new WaitForEndOfFrame();
		Debug.Log("Video stopped");
		SceneManager.LoadScene("cookbook");
	}	

//	void CreateDataManager()
//	{
//		new GameObject("DataManager",typeof(DataManager));
//	}
}
