using System;
using System.Collections.Generic;

namespace Voltage.Witches.Net
{
	using Voltage.Common.Logging;

	using UnityEngine;
	using Voltage.Common.Net;

	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	public enum WitchesRequestStatus	// TODO: design a standard status, and have projects add custom statuses
	{
		FAILED = 0,
		SUCCESS,
		MAINTENANCE,
		TIMED_OUT,
		REQUEST_ERROR,	// General
		CLIENT_ERROR,
		SERVER_ERROR
	}


	public class WitchesNetworkResponseController : INetworkTimeoutController<WitchesRequestResponse>
    {

		public INetworkTransportLayer Send (string url, IDictionary<string,string> parms, Action<WitchesRequestResponse> onSuccess, Action<WitchesRequestResponse> onFailure, int timeout=30)
		{
			INetworkRequest request = CreateRequest (url, parms);
			WWWPostRequest post = new WWWPostRequest (request, (response) => OnCompletedRequest (response, request, onSuccess, onFailure), timeout);
			post.Send ();

			return post;
		}
		
		public INetworkTransportLayer Receive (string url, IDictionary<string,string> parms, Action<WitchesRequestResponse> onSuccess, Action<WitchesRequestResponse> onFailure, int timeout=30)
		{
			INetworkRequest request = CreateRequest (url, parms);
			WWWGetRequest get = new WWWGetRequest (request, (response) => OnCompletedRequest (response, request, onSuccess, onFailure), timeout);
			get.Send ();

			return get;
		}

		private INetworkRequest CreateRequest(string url, IDictionary<string,string> parms)
		{
			return new NetworkRequest (url, parms);
		}

		private void OnCompletedRequest(WWW www, INetworkRequest request, Action<WitchesRequestResponse> onSuccess, Action<WitchesRequestResponse> onFailure)
		{
			WitchesRequestStatus status = GetStatus (www);
			WitchesRequestResponse response = GetResponse (status, www, request);
//			AmbientLogger.Current.Log (string.Format("WitchesNetworkResponseController::OnCompletedResponse >>> {0} [{1}]", www.url, status.ToString ()), LogLevel.INFO);

			switch(status)
			{
				case WitchesRequestStatus.SUCCESS:		ReturnResponse(onSuccess, response); break;
				case WitchesRequestStatus.MAINTENANCE:	//OnServerMaintenance(); break;
				case WitchesRequestStatus.TIMED_OUT:	
				case WitchesRequestStatus.FAILED: 		
				default:								ReturnResponse(onFailure, response); break;
			}
		}

		private void ReturnResponse(Action<WitchesRequestResponse> action, WitchesRequestResponse response)
		{
			if(action != null)
			{
				action(response);
			}
			else
			{
				AmbientLogger.Current.Log ("WitchesNetworkResponseController::ReturnResponse >>> no response action provided", LogLevel.INFO);
			}
		}

		private WitchesRequestResponse GetResponse(WitchesRequestStatus status, WWW www, INetworkRequest request)
		{
			return new WitchesRequestResponse ()
			{
				Request = request,
				Status = status,
				WWW = www,
				Text = (www != null ? GetText(www) : string.Empty),
			};
		}

		private string GetText(WWW www)
		{
			if(string.IsNullOrEmpty(www.error))
			{
				return www.text;
			}
			else
			{
				return www.error;
			}
		}

		private WitchesRequestStatus GetStatus(WWW www)	// break out into Voltage.Witches
		{
			if(www != null)
			{
				try
				{
					if(string.IsNullOrEmpty(www.error) && !string.IsNullOrEmpty(www.text))
					{
						Dictionary<string,object> response = JsonConvert.DeserializeObject<Dictionary<string,object>>(www.text);	// can throw exception
						if(response.ContainsKey("status"))
						{	
							string status = response["status"].ToString();
							switch(status)
							{
								case "success": 	return WitchesRequestStatus.SUCCESS;
								case "failed":		return WitchesRequestStatus.FAILED; 
								case "maintenance":	return WitchesRequestStatus.MAINTENANCE;
//								default:			return WitchesRequestStatus.REQUEST_ERROR;
							}
						}
					}
					else
					{
						AmbientLogger.Current.Log ("Request failed! likely no Internet connection", LogLevel.WARNING);
						return WitchesRequestStatus.REQUEST_ERROR;
					}
				}
				catch(Exception e)
				{
					AmbientLogger.Current.Log (e.ToString(), LogLevel.WARNING);
				}
				
				return WitchesRequestStatus.REQUEST_ERROR;
			}
			else
			{
				return WitchesRequestStatus.TIMED_OUT;
			}
		}



    }
    
}



