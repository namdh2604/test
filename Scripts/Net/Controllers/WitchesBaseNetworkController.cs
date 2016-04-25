
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Net
{
	using Voltage.Common.Net;
	using Voltage.Witches.Services;
	using UnityEngine;

//	using Voltage.Common.ID;

	public interface IBaseUrl
	{
		string BaseURL { get; set; }
	}

	public class WitchesBaseNetworkController : INetworkTimeoutController<WitchesRequestResponse>, IBaseUrl
    {
		private readonly INetworkTimeoutController<WitchesRequestResponse> _networkController;
		private readonly IBuildNumberService _versionService;

		public string BaseURL { get; set; }

		public WitchesBaseNetworkController(INetworkTimeoutController<WitchesRequestResponse> networkController, IBuildNumberService versionService, string baseURL="")
		{
			if(networkController == null || versionService == null)
			{
				throw new ArgumentNullException();
			}
			
			_networkController = networkController;
			_versionService = versionService;

			BaseURL = !string.IsNullOrEmpty(baseURL) ? baseURL : string.Empty;
		}

		public INetworkTransportLayer Send (string url, IDictionary<string,string> parms, Action<WitchesRequestResponse> onSuccess, Action<WitchesRequestResponse> onFailure, int timeout=30)
		{
//			UnityEngine.Debug.LogWarning (string.Format ("[POST] NetworkController ID: {0} ({1}...)", UniqueObjectID.Default.GetID (this), url));

			string fullURL = FormURL (url);
			parms = AddBaseAttributes(parms);

			return _networkController.Send (fullURL, parms, onSuccess, onFailure, timeout);
		}
		
		public INetworkTransportLayer Receive (string url, IDictionary<string,string> parms, Action<WitchesRequestResponse> onSuccess, Action<WitchesRequestResponse> onFailure, int timeout=30)
		{
//			UnityEngine.Debug.LogWarning (string.Format ("[GET] NetworkController ID: {0} ({1}...)", UniqueObjectID.Default.GetID (this), url));

			string fullURL = FormURL (url);
			parms = AddBaseAttributes(parms);

			return _networkController.Receive (fullURL, parms, onSuccess, onFailure, timeout);
		}

		private string FormURL(string endpoint)
		{
			return BaseURL + "/" + endpoint;		// could potentially do some url validation here
		}

		private IDictionary<string,string> AddBaseAttributes(IDictionary<string,string> parms)
		{
			IDictionary<string,string> newParms = (parms != null ? new Dictionary<string,string> (parms) : new Dictionary<string,string>());

			if(!newParms.ContainsKey ("build")) 
			{
				newParms.Add ("build", GetBuildVersion ());
			}

			if(!newParms.ContainsKey ("device")) 
			{
				newParms.Add ("device", GetDevice ());
			}

			return newParms;
		}

		private string GetBuildVersion()
		{
			return _versionService.GetBaseBuildVersion();
		}

		private string GetDevice ()
		{
			return Application.platform.ToString ();
		}

    }
    
}




