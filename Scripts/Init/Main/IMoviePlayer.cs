
using System;
using System.Collections;

namespace Voltage.Witches.DI 
{
	using Voltage.Common.Logging;

	public interface IMoviePlayer
	{
		void Play (Action onComplete);
	}
	
	public class MobileIntroMoviePlayer : IMoviePlayer
	{
		private string _path = "QG_Large.mp4";
		
		public void Play (Action onComplete)
		{
			
			Voltage.Common.Unity.UnitySingleton.Instance.StartCoroutine (PlayRoutine (_path, onComplete));
		}
		
		private IEnumerator PlayRoutine(string path, Action onComplete)
		{
			UnityEngine.Handheld.PlayFullScreenMovie(path, UnityEngine.Color.black, UnityEngine.FullScreenMovieControlMode.CancelOnInput, UnityEngine.FullScreenMovieScalingMode.AspectFit);
			
			yield return new UnityEngine.WaitForEndOfFrame();
			
			if(onComplete != null)
			{
				onComplete();
			}
		}
		
	}

	public class EditorIntroMoviePlayer : IMoviePlayer
	{
		
		public ILogger Logger { get; private set; }
		
		public EditorIntroMoviePlayer (ILogger logger)
		{
			Logger = logger;
		}
		
		public void Play (Action onComplete)
		{
			Logger.Log ("Playing Movie", LogLevel.INFO);
			
			Voltage.Common.Unity.UnitySingleton.Instance.StartCoroutine (PlayRoutine (onComplete));
		}
		
		private IEnumerator PlayRoutine(Action onComplete)
		{
			var movieplayer = UnityEngine.Resources.Load ("MockMovie");		// TEMPORARY
			var go = UnityEngine.MonoBehaviour.Instantiate (movieplayer);

			yield return new UnityEngine.WaitForSeconds (2f);
			
			Logger.Log ("Ending Movie", LogLevel.INFO);
			UnityEngine.MonoBehaviour.Destroy (go);
			
			yield return new UnityEngine.WaitForEndOfFrame();
			
			if(onComplete != null)
			{
				onComplete();
			}
		}
		
	}
    
}




