using System;
using System.Collections.Generic;

namespace Voltage.Witches.DI
{
	using Voltage.Common.Net;
	using Voltage.Witches.Net;
	using Voltage.Witches.Services;
	using UnityEngine;
    using Voltage.Witches.Exceptions;

	public class EnvironmentDataFetcher
	{
		private readonly INetworkTimeoutController<WitchesRequestResponse> _networkController;
		private readonly IBuildNumberService _versionService;
		private readonly IEnvironmentParser _dataParser;
		
        public EnvironmentDataFetcher(INetworkTimeoutController<WitchesRequestResponse> networkController, IBuildNumberService versionService, IEnvironmentParser dataParser)
		{
            if (networkController == null || versionService == null || dataParser == null)
			{
				throw new ArgumentNullException();
			}
			
			_networkController = networkController;
			_versionService = versionService;
			_dataParser = dataParser;
		}
		
		public void Fetch(Action<Exception, EnvironmentData> callback)
		{
			Dictionary<string,string> parms = new Dictionary<string,string>		// hesitantly not relying on networkcontroller to provide this information
			{
				{"build", GetBuildVersion()},
				{"device", GetDevice()}
			};
			
            _networkController.Send(URLs.POST_BUILDVERSION, parms, (response) => GetEnvironmentSuccess(response, callback), (response) => GetEnvironmentFailed(response, callback)); 
		}
		
		private string GetBuildVersion()
		{
			return _versionService.GetBaseBuildVersion();
		}
		
		private string GetDevice()
		{
			return Application.platform.ToString ();
		}
		
		private void GetEnvironmentSuccess(WitchesRequestResponse response, Action<Exception, EnvironmentData> onComplete)//, Action onComplete)
		{
			EnvironmentData data = _dataParser.Parse(response.Text);
			onComplete(null, data);
		}
		
		private void GetEnvironmentFailed(WitchesRequestResponse payload, Action<Exception, EnvironmentData> onComplete)
		{
            onComplete(new EnvironmentMissingException(), null);
		}
		
	}
    
}




