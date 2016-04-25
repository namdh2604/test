using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using System.Net;
using Voltage.Common.Unity;


namespace Voltage.Common.Net
{
	//**********************************************//
	//												//
	//					Unity (WWW)					//
	//												//
	//**********************************************//

	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;



	public sealed class UnityGetRequest : UnityNetworkTransport
	{
		public UnityGetRequest (INetworkRequest request, Action<WWWNetworkPayload> onSuccess, Action<WWWNetworkPayload> onFailure, int timeout) : base (request, onSuccess, onFailure, timeout) {}
		
		protected override WWW MakeWWW (INetworkRequest request)
		{
			if(request != null && !string.IsNullOrEmpty(request.URL))
			{
				//				return new WWW (request.ToString());
				return new WWW (CreateGetURL(request));
			}
			else
			{
				throw new ArgumentNullException("WWWPostRequest::MakeWWW");
			}
		}
		
		private string CreateGetURL(INetworkRequest request)
		{
			return string.Format ("{0}{1}", request.URL, InlineParameters(request.Parameters));
		}
		
		// /test/demo_form.asp?name1=value1&name2=value2
		private string InlineParameters(IDictionary<string,string> parms)
		{
			string inline = string.Empty;
			
			if(parms != null && parms.Count > 0)
			{
				inline += "?";
				
				List<string> keyList = new List<string>(parms.Keys);
				for(int i=0; i < keyList.Count; ++i)
				{
					string parameter = string.Format("{0}={1}", keyList[i], parms[keyList[i]]);
					parameter += (i < parms.Count-1 ? "&" : string.Empty);
					
					inline += parameter;
				}
			}
			
			return inline;
		}
		
	}
	
	public sealed class UnityPostRequest : UnityNetworkTransport
	{
		public UnityPostRequest (INetworkRequest request, Action<WWWNetworkPayload> onSuccess, Action<WWWNetworkPayload> onFailure, int timeout) : base (request, onSuccess, onFailure, timeout) {}
		
		protected override WWW MakeWWW (INetworkRequest request)
		{
			if(request != null && !string.IsNullOrEmpty(request.URL))
			{
				WWWForm form = new WWWForm();
				
				if(request.Parameters.Count > 0)
				{
					foreach (KeyValuePair<string,string> kvp in request.Parameters)
					{
						string value = (!string.IsNullOrEmpty(kvp.Value) ? kvp.Value : string.Empty);
						
						form.AddField (kvp.Key, value);
					}
				}
				else
				{
					form.AddField(string.Empty, string.Empty);
				}
				
				return new WWW (request.URL, form);
			}
			else
			{
				throw new ArgumentNullException("WWWPostRequest::MakeWWW");
			}
		}
		
		//		private void FillInDummyValue(WWWForm form)
		//		{
		//			form.AddField (string.Empty, string.Empty);
		//		}
	}



	public abstract class UnityNetworkTransport : NetworkTransportWithTimeout	// FIXME: Doing too much, break up
	{
		protected readonly Action<WWWNetworkPayload> _onSuccess;
		protected readonly Action<WWWNetworkPayload> _onFailure;

		protected readonly MonoBehaviour _mono;

		protected WWW _www;
		public override float Progress { get { return _www != null ? _www.progress : 0f; } }

		public UnityNetworkTransport (INetworkRequest request, Action<WWWNetworkPayload> onSuccess, Action<WWWNetworkPayload> onFailure, int timeout, MonoBehaviour mono=null) : base (request, timeout)
		{
			_onSuccess = onSuccess;
			_onFailure = onFailure;

			_mono = mono != null? mono : UnitySingleton.Instance;
		}

		public override void Send ()
		{
			if (_mono != null && HasValidRequest)
			{
				_www = MakeWWW (_request); // (_request.URL, _request.Parameters);
				IEnumerator request = SendRequest (_www, OnSuccess, OnFailure, _request);

				_mono.StartCoroutine (RequestRoutine (_www, _mono, request, (float)TimeoutDelay));
			}
			else
			{
				throw new ArgumentNullException("UnityNeteworkTransport");
			}
		}

		protected abstract WWW MakeWWW (INetworkRequest request);


		protected IEnumerator SendRequest (WWW www, Action<WWW> onSuccess, Action<WWW> onFailure, INetworkRequest request)
		{
//			using (WWW www = new WWW(url))
			{
				yield return www;

//				if(www.error != null)
				if(RequestIsSuccessful(www))
				{
					onSuccess(www);
				}
				else
				{
					onFailure(www);
				}	
			}
		}

		private bool RequestIsSuccessful(WWW www)
		{
			try
			{
				if(www.error == null && !string.IsNullOrEmpty(www.text))
				{
					Dictionary<string,object> response = JsonConvert.DeserializeObject<Dictionary<string,object>>(www.text);
					if(response.ContainsKey("status"))
					{
						return response["status"].ToString() == "success";
					}
				}
			}
			catch(Exception e) 	// cast exception
			{
				Debug.LogWarning("UnityNetworkTransport::RequestIsSuccessful >>> FAILED: " + e.ToString());
			}
			
			return false;	
		}


		protected IEnumerator RequestRoutine (WWW www, MonoBehaviour mono, IEnumerator request, float timeout)
		{
			if (mono != null && request != null)
			{
				mono.StartCoroutine (request);
				yield return new WaitForSeconds(timeout);

				if (!www.isDone)	// timed out
				{
					OnTimeout (www, request);
				}
			}
			else
			{
				throw new ArgumentNullException ("UnityNetworkTransport::RequestRoutine");
			}
		}

		protected void OnSuccess (WWW www)
		{
			if (_onSuccess != null && www != null) // && !string.IsNullOrEmpty(content))
			{
				WWWNetworkPayload payload = CreatePayload (www, _request);
				_onSuccess (payload);
			}
			else
			{
				throw new ArgumentNullException("UnityNetworkTransport::OnSuccess");
			}
		}
		
		protected void OnFailure (WWW www)
		{
			if (_onFailure != null && www != null)
			{
				WWWNetworkPayload payload = CreatePayload (www, _request);
				_onFailure (payload);
			}
			else
			{
				// warning...sometimes maybe want to do nothing
				throw new ArgumentNullException("UnityNetworkTransport::OnSuccess");
			}
		}

		protected void OnTimeout (WWW www, IEnumerator request)
		{
			if(_onFailure != null && www != null)
			{
				_mono.StopCoroutine(request);						// http://forum.unity3d.com/threads/new-added-stopcoroutine-coroutine-coroutine-override-function-causes-errors.287481/

				WWWNetworkPayload payload = new WWWNetworkPayload();
				payload.Request = _request;
//				payload.Status = HttpStatusCode.RequestTimeout;
				payload.Text = "Request Timed Out";

				_onFailure (payload);
			}
			else
			{
				throw new ArgumentNullException("UnityNetworkTransport::OnTimeout");
			}
		}


		private WWWNetworkPayload CreatePayload(WWW www, INetworkRequest request)
		{
			WWWNetworkPayload payload = new WWWNetworkPayload ();
			payload.Request = request;
//			payload.Status = GetStatus (www);

//			Debug.Log ("Status: " + payload.Status);
//			payload.Text = (ValidResponse (payload.Status) && www != null ? www.text : string.Empty); 

			payload.WWW = www;

			return payload;
		}

		private bool ValidResponse(HttpStatusCode status)
		{
			return (int)status >= 200 && (int)status < 300;
		}


		private HttpStatusCode GetStatus (WWW www)		// HACK: Simple implementation to circumvent mobile bug with Unity www.responseheader and Status
		{
			if(www != null && string.IsNullOrEmpty(www.error))
			{
				return HttpStatusCode.OK;
			}
			
			return HttpStatusCode.BadRequest;
		}




		public override string ToString ()
		{
			return string.Format ("{0} ::: {1}", this.GetType(), (_request != null ? _request.ToString() : "null"));
		}

	}
}








//private HttpStatusCode GetStatusDetailed (WWW www)	// http://issuetracker.unity3d.com/issues/www-ios-android-status-and-other-keys-missing-from-www-dot-responseheaders-on-mobile
//{													// http://issuetracker.unity3d.com/issues/www-dot-responseheaders-status-key-is-null-in-android
//	if(www != null)
//	{
//		//				foreach(var kvp in www.responseHeaders)
//		//				{
//		//					Debug.Log ("KVP: " + kvp.Key.ToString() + ", " + kvp.Value.ToString());
//		//				}
//		
//		if(www.responseHeaders != null && www.responseHeaders.ContainsKey("STATUS"))		// Unity Bug
//		{
//			Debug.Log ("Status: " + www.responseHeaders["STATUS"]);
//			
//			string[] status = www.responseHeaders["STATUS"].Split(' ');		// HTTP/1.1 200 OK
//			
//			int statusCode = 0;
//			if (Int32.TryParse(status[1], out statusCode))
//			{
//				return (HttpStatusCode)statusCode;
//			}
//			else
//			{
//				Debug.LogWarning ("GetStatusDetailed: Failed TryParse");
//			}
//		}
//		else
//		{
//			Debug.LogWarning ("GetStatusDetailed: response header does not contain STATUS");
//		}
//	}
//	
//	return HttpStatusCode.BadRequest;
//}
















//public class WWWGetRequest : UnityNetworkTransport
//{
//	public WWWGetRequest (INetworkRequest request, Action<WWW> onSuccess, Action<HttpStatusCode,WWW> onFailure, int timeout) : base (request, onSuccess, onFailure, timeout) {}
//}
//
//public class WWWPostRequest : UnityNetworkTransport
//{
//	public WWWPostRequest (INetworkRequest request, Action<WWW> onSuccess, Action<HttpStatusCode,WWW> onFailure, int timeout) : base (request, onSuccess, onFailure, timeout) {}
//}
//
//
//public abstract class UnityNetworkTransport : NetworkTransportWithTimeout
//{
//	protected readonly Action<WWW> _onSuccess;
//	protected readonly Action<HttpStatusCode,WWW> _onFailure;
//	
//	protected readonly MonoBehaviour _mono;
//	
//	public UnityNetworkTransport (INetworkRequest request, Action<WWW> onSuccess, Action<HttpStatusCode,WWW> onFailure, int timeout, MonoBehaviour mono=null) : base (request)
//	{
//		TimeoutDelay = timeout;
//		
//		_onSuccess = onSuccess;
//		_onFailure = onFailure;
//		
//		_mono = mono != null? mono : UnitySingleton.Instance;
//	}
//	
//	public override void Send ()
//	{
//		if (_mono != null && HasValidRequest)
//		{
//			WWW www = MakeWWW (_request.URL, _request.Parameters);
//			IEnumerator request = SendRequest (www, OnSuccess, OnFailure);
//			
//			_mono.StartCoroutine (RequestRoutine (www, _mono, request, (float)TimeoutDelay));
//		}
//		else
//		{
//			// error
//		}
//	}
//	
//	protected WWW MakeWWW (string url, Dictionary<string,string> parms)
//	{
//		if(!string.IsNullOrEmpty(url))
//		{
//			if(parms != null)
//			{
//				WWWForm form = new WWWForm();
//				foreach (KeyValuePair<string,string> kvp in parms)
//				{
//					form.AddField(kvp.Key, kvp.Value);
//				}
//				
//				return new WWW (url, form);		// POST
//			}
//			else
//			{
//				return new WWW(url);			// GET
//			}
//			
//		}
//		else
//		{
//			throw new NullReferenceException();
//		}			 
//	}
//	
//	protected IEnumerator SendRequest (WWW www, Action<WWW> onSuccess, Action<HttpStatusCode,WWW> onFailure)
//	{
//		//			using (WWW www = new WWW(url))
//		{
//			yield return www;
//			
//			if(www.error != null)
//			{
//				onFailure(GetStatus(www), www);
//			}
//			else
//			{
//				onSuccess(www);
//			}	
//		}
//	}
//	
//	protected IEnumerator RequestRoutine (WWW www, MonoBehaviour mono, IEnumerator request, float timeout)
//	{
//		if (mono != null && request != null)
//		{
//			mono.StartCoroutine (request);
//			yield return new WaitForSeconds(timeout);
//			
//			if (!www.isDone)	// timed out
//			{
//				mono.StopCoroutine(request);	// http://forum.unity3d.com/threads/new-added-stopcoroutine-coroutine-coroutine-override-function-causes-errors.287481/
//				OnFailure(HttpStatusCode.RequestTimeout, www);
//			}
//		}
//		else
//		{
//			// error
//		}
//	}
//	
//	protected void OnSuccess (WWW www)
//	{
//		if (_onSuccess != null) // && !string.IsNullOrEmpty(content))
//		{
//			_onSuccess (www);
//		}
//		else
//		{
//			// warning
//		}
//	}
//	
//	protected void OnFailure (HttpStatusCode status, WWW www)
//	{
//		if (_onFailure != null) // && !string.IsNullOrEmpty(content))
//		{
//			_onFailure (status, www);
//		}
//		else
//		{
//			// warning...sometimes maybe want to do nothing
//		}
//	}
//	
//	private HttpStatusCode GetStatus (WWW www)
//	{
//		if(www != null)
//		{
//			
//			if(www.responseHeaders != null && www.responseHeaders.ContainsKey("STATUS"))
//			{
//				int statusCode = 0;
//				if (Int32.TryParse(www.responseHeaders["STATUS"], out statusCode))
//				{
//					return (HttpStatusCode)statusCode;
//				}
//			}
//		}
//		
//		return HttpStatusCode.BadRequest;
//	}
//	
//	public override string ToString ()
//	{
//		return string.Format ("{0} ::: {1}", this.GetType(), (_request != null ? _request.ToString() : "null"));
//	}
//	
//}












