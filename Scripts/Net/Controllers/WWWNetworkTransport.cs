
using System;
using System.Collections;
using System.Collections.Generic;

namespace Voltage.Common.Net
{
	using UnityEngine;
	using Voltage.Common.Unity;


	public abstract class WWWNetworkTransport : INetworkTransportLayer, ITimeout
	{
		public const int DEFAULT_TIMEOUT = 90;		// in seconds
		
		public int TimeoutDelay { get; private set; }
		public void SetTimeout (int seconds)
		{
			TimeoutDelay = seconds; //NetUtils.ConvertToMilliseconds(seconds);
		}
		
		public float Progress { get { return _www.progress; } }
//		public bool IsDone { get { return _www.isDone; } }
		
		private readonly WWW _www;
		private readonly Action<WWW> _onComplete;
		private readonly MonoBehaviour _mono;
		
		public WWWNetworkTransport (INetworkRequest request, Action<WWW> onComplete, int timeoutInSeconds=DEFAULT_TIMEOUT)
		{
			if(request == null || onComplete == null)
			{
				throw new ArgumentException();
			}
			
			_www = MakeWWW (request);
			_onComplete = onComplete;
			SetTimeout (timeoutInSeconds);
			
			_mono = UnitySingleton.Instance;
		}
		
		protected abstract WWW MakeWWW (INetworkRequest request);
		
		
		public void Send()
		{
			IEnumerator request = Request (_www, OnRequestCompleted);
			_mono.StartCoroutine (TimeoutCheckRoutine (_www, request, (float)TimeoutDelay));
		}
		
		private IEnumerator Request(WWW www, Action<WWW> onComplete)
		{
			yield return www;
			
			onComplete (www);
		}
		
		private IEnumerator TimeoutCheckRoutine (WWW www, IEnumerator request, float timeout)
		{
			_mono.StartCoroutine (request);
			yield return new WaitForSeconds(timeout);
			
			if (!www.isDone)	// timed out
			{
				OnRequestTimeout (www, request);
			}
			
		}
		
		protected void OnRequestTimeout (WWW www, IEnumerator request)
		{
			_mono.StopCoroutine(request);						// http://forum.unity3d.com/threads/new-added-stopcoroutine-coroutine-coroutine-override-function-causes-errors.287481/

			OnRequestCompleted (null);
		}
		
		
		private void OnRequestCompleted(WWW www)
		{	
			_onComplete (www);
		}

		
	}
    
}




