using UnityEngine;
using System;
//using System.Collections;
using System.Collections.Generic;

namespace Voltage.Witches.Resetter
{
	using Voltage.Common.Logging;
	using Voltage.Witches.Exceptions;

    public class ResetMonitor : MonoBehaviour
    {
		private IResetter _resetter;
		private float _timeoutDurationInMin;
		private DateTime _pausedTime;			// DateTime?

		private void Awake()
		{
//			if(Application.platform != RuntimePlatform.IPhonePlayer || Application.platform != RuntimePlatform.Android)
//			{
//				Destroy (this);
//			}
		}

		public void Init(IResetter resetter, float timeoutDurationInMin)
		{
//			AmbientLogger.Current.Log ("ResetMonitor >>> Init", LogLevel.INFO);
			_resetter = resetter;
			_timeoutDurationInMin = timeoutDurationInMin;
		}


		private void OnApplicationFocus(bool focused)
		{
//			AmbientLogger.Current.Log ("ResetMonitor >>> OnApplicationFocus", LogLevel.INFO);

			if(!focused)
			{
				OnPause();
			}
			else
			{
				OnUnpaused();
			}
		}



//		public void OnApplicationPause(bool paused)
//		{
//			AmbientLogger.Current.Log ("ResetMonitor >>> OnApplicationPause", LogLevel.INFO);
//
//			if(paused)
//			{
//				OnPause();
//			}
//			else
//			{
//				OnUnpaused();
//			}
//		}

//		private void OnGUI()	// resets immediately when game is running, but not OnApplicationFocus
//		{
//			if(GUILayout.Button("reset"))
//			{
//				_resetter.Reset();
//			}
//		}


		private void OnPause()
		{
			_pausedTime = DateTime.UtcNow;
//			AmbientLogger.Current.Log ("ResetMonitor >>> App Paused: " + _pausedTime.ToString(), LogLevel.INFO);
		}

		private void OnUnpaused()
		{
			DateTime unpausedTime = DateTime.UtcNow;
//			AmbientLogger.Current.Log ("ResetMonitor >>> App Unpaused: " + unpausedTime.ToString(), LogLevel.INFO);

			if(ExceededPauseTime(_pausedTime, unpausedTime))
			{
				if(_resetter != null)
				{
					AmbientLogger.Current.Log ("ResetMonitor >>> Resetting Game", LogLevel.INFO);
//					_resetter.Reset();
					StartCoroutine(ResetRoutine());
				}
				else
				{
					throw new WitchesException ("ResetMonitor::OnUnPaused >>> resetter is null");
				}
			} 
		}

		private System.Collections.IEnumerator ResetRoutine()	// hmm...upon OnApplicationFocus, Unity/something(?) needs time to get ready before I can call Reset()
		{
			yield return new WaitForSeconds (0.2f);
			_resetter.Reset();
		}
		
		private bool ExceededPauseTime(DateTime startTime, DateTime endTime)
		{
			return startTime != DateTime.MinValue && (endTime - startTime).TotalMinutes > _timeoutDurationInMin;	// startTime.HasValue
		}



    }

}



