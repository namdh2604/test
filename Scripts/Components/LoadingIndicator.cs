using UnityEngine;
using System.Collections;

public class LoadingIndicator 
{
	public static bool IsLoading { get { return _isLoading; } }
	private static bool _isLoading = false;

	public static LoadingIndicator Instance() { return _instance; }
	private static LoadingIndicator _instance = null;

	public LoadingIndicator()
	{
		_instance = this;
	}

	public static void Init()
	{
		UnityEngine.Debug.Log("Start init on Loading Indicator");
		_isLoading = true;
		var enumerator = PseudoCoroutine().GetEnumerator();
		while(enumerator.MoveNext())
		{
			var answer = enumerator.Current;
			UnityEngine.Debug.Log(answer.ToString() + " came back from the enumerator");
		}
	}

	public static void Done(int answer)
	{
		UnityEngine.Debug.Log("Loading Indicator should be killed now");
		_isLoading = false;
	}

	public static IEnumerable PseudoCoroutine()
	{
		int t = 0;
		#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
		Handheld.SetActivityIndicatorStyle(UnityEngine.iOS.ActivityIndicatorStyle.Gray);
		Handheld.StartActivityIndicator();
		#elif UNITY_ANDROID && !UNITY_EDITOR
		Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle.Large);
		Handheld.StartActivityIndicator();
		#else
		UnityEngine.Debug.Log("Load indicator display");
		#endif

		while(_isLoading)
		{
			++t;
			UnityEngine.Debug.LogWarning("Is loading...");
			yield return (t);
		}

		#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
		Handheld.StopActivityIndicator();
		#elif UNITY_ANDROID && !UNITY_EDITOR
		Handheld.StopActivityIndicator();
		#else
		UnityEngine.Debug.Log("Load indicator display done");
		#endif
	}
}
