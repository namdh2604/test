using System;
using System.Collections;
using UnityEngine;
using Voltage.Common.Unity;

namespace Voltage.Witches.DI
{
	// Controls movie playback. The wrapper is used for future compatibility, in the event we wish to support an editor version that behaves the same way
	public class SplashMovieController
	{
		private static bool _wasDisplayed = false;

		public SplashMovieController()
		{
		}

		public void Show(Action callback)
		{
			UnitySingleton.Instance.StartCoroutine(ShowRoutine(callback));
		}

		private IEnumerator ShowRoutine(Action callback)
		{
			if (!_wasDisplayed)
			{
				FullScreenMovieScalingMode scalingMode = FullScreenMovieScalingMode.AspectFit;

				// HACK -- There seems to be a bug with android (or at least the HTC One M8), where aspect fit doesn't work correctly.
				// The end result is slight framing on all sides. If you change it to aspect fill, the assumption would be that it bleeds over,
				// but it seems that actually works incorrectly too, functioning like you'd imagine aspect fit should -- MR
				if (Application.platform == RuntimePlatform.Android)
				{
					scalingMode = FullScreenMovieScalingMode.AspectFill;
				}

				Handheld.PlayFullScreenMovie("KissesnCurses_compressed.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput, scalingMode);
				_wasDisplayed = true;

                yield return new WaitForEndOfFrame(); // Need to call this twice 
                yield return new WaitForEndOfFrame(); // to make sure we don't start loading after movie is done.
			}

			callback();
		}
	}
}

