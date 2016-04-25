using System;
using System.Collections.Generic;

namespace Voltage.Common.Net
{
//	using System.Net;
//	using System.Text;
//	using System.IO;
//	using System.Threading;



//	public class TransportLogLayer : INetworkTransport
//	{
//		private ILogger _logger;
//		private INetworkTransport _request;
//
//		public TransportLogLayer(INetworkTransport request, ILogger logger)
//		{
//			_request = request;
//			_logger = logger;
//		}
//
//		public void Send()
//		{
//			if(_request != null)
//			{
//				Log (string.Format("Network Request <{0}> Sent", _request.GetType()), LogLevel.INFO);
//
//				_request.Send();
//         	}
//		}
//
//		private void ReceiveSuccessful<T>(T response, Action<T> callback)
//		{
//			if(_request != null)
//			{
//				Log (string.Format("Network Request <{0}> Received", _request.GetType()), LogLevel.INFO);
//
//				if(callback != null)
//				{
//					callback(response);
//				}
//			}
//		}
//
//		private void ReceiveFailed<T>(HttpStatusCode status, T response, Action<T> callback)
//		{
//			if(_request != null)
//			{
//				Log (string.Format("Network Request <{0}> Received", _request.GetType()), LogLevel.INFO);
//				
//				if(callback != null)
//				{
//					callback(status, response);
//				}
//			}
//		}
//
//
//		private void Log(string msg, LogLevel level=LogLevel.INFO)
//		{
//			if(_logger != null)
//			{
//				_logger.Log(msg, level);
//			}
//		}
//	}






	//**********************************************//
	//												//
	//					WebClient					//
	//												//
	//**********************************************//

/*
	// FIXME: doesn't work with async timeouts on iOS devices
	public abstract class WebClientStringRequest : NetworkTransportWithTimeout
	{
		protected readonly Action<string> _onSuccess;
		protected readonly Action<HttpStatusCode,string> _onFailure;
		
		public WebClientStringRequest (INetworkRequest request, Action<string> onSuccess, Action<HttpStatusCode,string> onFailure, int timeout) : base (request, timeout)
		{
			_onSuccess = onSuccess;
			_onFailure = onFailure;
		}
		
		public override void Send ()
		{
			if(HasValidRequest)
			{
				using (var client = new SyncTimeoutWebClient(TimeoutDelay))//new WebClient())
				{
					client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
					
					// TODO: consolidate AsyncCompletedEventHandler here instead of in children
					
					DeliveryMethod (client);
					StartTimer (client.CancelAsync);	// https://bugzilla.xamarin.com/show_bug.cgi?id=2483, http://stackoverflow.com/questions/10689860/monotouch-webclient-cancelasync-threadinterruptedexception
				}
			}
		}
		
		protected abstract void DeliveryMethod (WebClient client);
		
//		public override INetworkTransport Create()
//		{
//
//		}

//		protected void StartTimer (Action onTimeout)		// System.Threading.Timer
//		{
//			DateTime start = DateTime.UtcNow;
//			Timer timeoutTimer = null; 
//			timeoutTimer = new Timer 
//				(
//					(obj) => 
//					{ 
////					Console.WriteLine(string.Format("Timedout<{0}>, Actual: {1} ", TimeoutDelay, (DateTime.UtcNow-start).TotalMilliseconds)); 
//					if(onTimeout != null)
//					{ 
//						onTimeout(); 
//					} 
//					timeoutTimer.Dispose();
////					timeoutTimer.Change(Timeout.Infinite, Timeout.Infinite);
//				}, 
//				null, TimeoutDelay, Timeout.Infinite
//				);
//		}		
		
		protected void StartTimer (Action onTimeout)		// Systems.Timers.Timer
		{
			System.Timers.Timer timer = new System.Timers.Timer(TimeoutDelay);
			timer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) => 
			{
				if (onTimeout != null)
				{
					onTimeout();
				}
				timer.Stop();
			};
			timer.Start();
		}
		
		
		protected HttpStatusCode GetStatus (Exception e)
		{
			WebException we = e as WebException;
			if (we != null)
			{
				if (we.Status == WebExceptionStatus.ProtocolError)
				{
					HttpWebResponse response = we.Response as HttpWebResponse;
					if(response != null)
					{
						return response.StatusCode;
					}
				}
				else
				{
					return HttpStatusCode.RequestTimeout;
				}
			}
			
			return HttpStatusCode.BadRequest;
		}
	}
	
	
	public sealed class WebClientStringGetRequest : WebClientStringRequest
	{
		public WebClientStringGetRequest (INetworkRequest request, Action<string> onSuccess, Action<HttpStatusCode,string> onFailure, int timeout) : base (request, onSuccess, onFailure, timeout) {}
		
		protected override void DeliveryMethod (WebClient client)
		{
			if (client != null && _request != null)
			{
				client.DownloadStringCompleted += (sender, e) =>  	// DownloadStringCompletedEventHandler , AsyncCompletedEventHandler
				{
					if(!e.Cancelled && e.Error == null)
					{
//						byte[] data = (byte[])e.Result;
//						string textData = System.Text.Encoding.UTF8.GetString (data);
						_onSuccess(e.Result);
					}
					else 
					{
						if(e.Error != null)
						{
							_onFailure(GetStatus(e.Error), e.Error.Message);
						}
						else
						{
							_onFailure(HttpStatusCode.RequestTimeout, "Request Timed Out");		// strange e.Error is now null on timeout/cancel?
						}
					}
					
				};
				
				client.DownloadStringAsync(new Uri(_request.URL), _request.URL);
			}
		}
		
		
	}
	
	public sealed class WebClientStringPostRequest : WebClientStringRequest
	{
		public WebClientStringPostRequest (INetworkRequest request, Action<string> onSuccess, Action<HttpStatusCode,string> onFailure, int timeout) : base (request, onSuccess, onFailure, timeout) {}
		
		protected override void DeliveryMethod (WebClient client)
		{
			if (client != null && _request != null)
			{
				client.UploadStringCompleted += (sender, e) =>  	// UploadStringCompletedEventHandler , AsyncCompletedEventHandler
				{
					if(!e.Cancelled && e.Error == null)
					{
//						byte[] data = (byte[])e.Result;
//						string textData = System.Text.Encoding.UTF8.GetString (data);
						_onSuccess(e.Result);
					}
					else 
					{
						if(e.Error != null)
						{
							_onFailure(GetStatus(e.Error), e.Error.Message);
						}
						else
						{
							_onFailure(HttpStatusCode.RequestTimeout, "Request Timed Out");		// strange e.Error is now null on timeout/cancel?
						}
					}
					
				};
				
				client.UploadStringAsync(new Uri(_request.URL), GetQueryString(_request.Parameters));
			}
		}
		
		private string GetQueryString (Dictionary<string,string> parms)
		{
			string data = string.Empty;
			
			if (parms != null)
			{
				foreach(KeyValuePair<string,string> kvp in parms)
				{
					data += string.Format("{0}={1}&", kvp.Key, kvp.Value);
				}
			}
			
			return data;
		}
	}
*/





	//**********************************************//
	//												//
	//				HttpWebRequest					//
	//												//
	//**********************************************//

/*

	// FIXME: doesn't work with async timeouts on iOS devices
	public abstract class AsyncHttpWebRequest: NetworkTransportWithTimeout
	{
		protected readonly Action<string> _onSuccess;
		protected readonly Action<HttpStatusCode,string> _onFailure;

		public AsyncHttpWebRequest(INetworkRequest request, Action<string> onSuccess, Action<HttpStatusCode,string> onFailure, int timeout=DEFAULT_TIMEOUT) : base (request, timeout) 
		{
			_onSuccess = onSuccess;
			_onFailure = onFailure;
		}




		// https://holyhoehle.wordpress.com/2010/01/15/making-an-asynchronous-webrequest/
		
		protected abstract HttpWebRequest MakeRequest (string url);

		protected void SendNetworkRequestAsync (string url, int timeout)
		{
			try
			{
				HttpWebRequest request = MakeRequest (url);

				RequestState state = new RequestState();
				state.Request = request;
				
				IAsyncResult result = request.BeginGetResponse(new AsyncCallback(ResponseCallback), state);

//				UnityEngine.Debug.LogWarning ("BeginGetResponse");

				// Timeout comes here
				ThreadPool.RegisterWaitForSingleObject(result.AsyncWaitHandle, new WaitOrTimerCallback((obj,method) => TimeOutCallback(obj,method,state)), request, timeout, true);
			}
			catch (Exception ex)	// System.Net.WebException
			{
				// Error handling
				Console.WriteLine(ex);
				OnFailure(HttpStatusCode.BadRequest, string.Empty);
			}
		}
		
		protected void TimeOutCallback(object obj, bool timedOut, RequestState state)
		{
//			UnityEngine.Debug.LogWarning ("TimeoutCallback!");

			if (timedOut)
			{
//				UnityEngine.Debug.LogWarning ("Timed out!");

				state.TimedOut = timedOut;

				HttpWebRequest request = obj as HttpWebRequest;
				if (request != null)
					request.Abort();

				OnFailure(HttpStatusCode.RequestTimeout, "Request Timed Out");
			}
		}
		
		protected void ResponseCallback(IAsyncResult result)
		{
//			UnityEngine.Debug.LogWarning ("ResponseCallback!");
			try
			{
				// Get and fill the RequestState
				RequestState state = (RequestState)result.AsyncState;

				if(!state.TimedOut)
				{
					HttpWebRequest request = state.Request;
					// End the Asynchronous response and get the actual resonse object
					state.Response = (HttpWebResponse)request.EndGetResponse(result);
					Stream responseStream = state.Response.GetResponseStream();
					state.ResponseStream = responseStream;

//					UnityEngine.Debug.LogWarning ("BeginRead!");

					// Begin async reading of the contents
	//				IAsyncResult readResult = responseStream.BeginRead(state.BufferRead, 0, state.BufferSize, new AsyncCallback (ReadCallback), state);
					responseStream.BeginRead(state.BufferRead, 0, state.BufferSize, new AsyncCallback (ReadCallback), state);
				}
			}
			catch (Exception ex)
			{
				// Error handling
				Console.WriteLine(ex);
				RequestState state = (RequestState)result.AsyncState;
				if (state.Response != null)
				{
//					UnityEngine.Debug.LogWarning ("ResponseCallback::Exception onFailure A!");

					OnFailure(state.Response.StatusCode, string.Empty);
					state.Response.Close();
				}
				else
				{
//					UnityEngine.Debug.LogWarning ("ResponseCallback::Exception onFailure B!");

					OnFailure(HttpStatusCode.BadRequest, string.Empty);
				}
			}
		}
		
		private void ReadCallback(IAsyncResult result)
		{
//			UnityEngine.Debug.LogWarning ("ReadCallback!");
			try
			{
				// Get RequestState
				RequestState state = (RequestState)result.AsyncState;
				// determine how many bytes have been read
				int bytesRead = state.ResponseStream.EndRead(result);
				
				if (bytesRead > 0) // stream has not reached the end yet
				{
					// append the read data to the ResponseContent and...
					state.ResponseContent.Append(Encoding.ASCII.GetString(state.BufferRead, 0, bytesRead));
					// ...read the next piece of data from the stream
					state.ResponseStream.BeginRead(state.BufferRead, 0, state.BufferSize,
					                               new AsyncCallback(ReadCallback), state);
				}
				else // end of the stream reached
				{
//					UnityEngine.Debug.LogWarning ("end of stream reached!");

					if (state.ResponseContent.Length > 0)
					{
						// do something with the response content, e.g. fill a property or fire an event
						string AsyncResponseContent = state.ResponseContent.ToString();
						// close the stream and the response
						state.ResponseStream.Close();
						state.Response.Close();
						OnAsyncResponseArrived(AsyncResponseContent);
					}
				}
			}
			catch (Exception ex)
			{
				// Error handling
				Console.WriteLine(ex);
				RequestState state = (RequestState)result.AsyncState;
				if (state.Response != null)
				{
//					UnityEngine.Debug.LogWarning ("ReadCallback::Exception onFailure A!");

					OnFailure(state.Response.StatusCode, string.Empty);
					state.Response.Close();
				}
				else
				{
//					UnityEngine.Debug.LogWarning ("ReadCallback::Exception onFailure A!");
					OnFailure(HttpStatusCode.BadRequest, string.Empty);
				}
			}
		}
		
		private void OnAsyncResponseArrived(string asyncResponseContent)
		{
//			UnityEngine.Debug.LogWarning ("OnSuccess!");
			OnSuccess (asyncResponseContent);
		}
		
		protected class RequestState
		{
			public int BufferSize { get; private set; }
			public StringBuilder ResponseContent { get; set; }
			public byte[] BufferRead { get; set; }
			public HttpWebRequest Request { get; set; }
			public HttpWebResponse Response { get; set; }
			public Stream ResponseStream { get; set; }
			public bool TimedOut { get; set; }
			
			public RequestState()
			{
				BufferSize = 1024;
				BufferRead = new byte[BufferSize];
				ResponseContent = new StringBuilder();
				Request = null;
				ResponseStream = null;
				TimedOut = false;
			}
		}



		protected void OnSuccess (string content)
		{
			if (_onSuccess != null) // && !string.IsNullOrEmpty(content))
			{
//				CompletedRequest = true;
				_onSuccess (content);
			}
			else
			{
				Console.WriteLine ();
			}
		}
		
		protected void OnFailure (HttpStatusCode status, string msg="")
		{
			if (_onFailure != null) // && !string.IsNullOrEmpty(content))
			{
//				CompletedRequest = true;
				_onFailure (status, msg);
			}
			else
			{
				Console.WriteLine (msg);
			}
		}
	}


	public class AsyncStringGetRequest : AsyncHttpWebRequest
	{
		public AsyncStringGetRequest (INetworkRequest request, Action<string> onSuccess, Action<HttpStatusCode,string> onFailure, int timeout) : base (request, onSuccess, onFailure, timeout) {}

		public override void Send ()
		{
			if(HasValidRequest)
			{
				SendNetworkRequestAsync (_request.URL, TimeoutDelay);
			}
		}

		protected override HttpWebRequest MakeRequest (string url)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = "GET";
			request.Proxy = null;
			request.Timeout = TimeoutDelay;

			return request;
		}
	}

	public class AsyncStringPostRequest : AsyncHttpWebRequest
	{
		public AsyncStringPostRequest (INetworkRequest request, Action<string> onSuccess, Action<HttpStatusCode,string> onFailure, int timeout) : base (request, onSuccess, onFailure, timeout) {}
		
		public override void Send ()
		{
			if(HasValidRequest)
			{
				SendNetworkRequestAsync (_request.URL, TimeoutDelay);
			}
		}

		protected override HttpWebRequest MakeRequest (string url)
		{
			byte[] data = GetQueryString (_request.Parameters);
			HttpWebRequest request = SetupRequest (url, data.Length);

			using (Stream stream = request.GetRequestStream())
			{
				stream.Write(data, 0, data.Length);
			}

			return request;
		}


		protected HttpWebRequest SetupRequest (string url, int dataLength)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			request.ContentLength = dataLength;
			request.AllowWriteStreamBuffering = true;
			request.Timeout = TimeoutDelay;
			
			return request;
		}

		private byte[] GetQueryString(Dictionary<string,string> parms)
		{
			string data = string.Empty;
			
			if (parms != null)
			{
				foreach(KeyValuePair<string,string> kvp in parms)
				{
					data += string.Format("{0}={1}&", kvp.Key, kvp.Value);
				}
			}
			
			return System.Text.Encoding.ASCII.GetBytes(data);
		}


	}

	public class SyncTimeoutWebClient : WebClient
	{
		public int Timeout { get; private set; }
		
		public SyncTimeoutWebClient(int timeout=30)		// only works synchronously
		{
			Timeout = timeout;
		}
		
		protected override WebRequest GetWebRequest(Uri uri)
		{
			WebRequest request = base.GetWebRequest(uri);
			request.Timeout = Timeout * 1000;
			return request;
		}
	}
*/
}

