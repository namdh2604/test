
using System;
using System.Collections.Generic;

namespace Voltage.Common.Android.ExpansionFile
{
	using Voltage.Story.General;
	using Voltage.Common.Net;

	using Voltage.Witches.Screens;

	using Voltage.Witches.Services;


    public class OBBFetcherFactory : IFactory<string,IExpansionFileFetcher>
    {
		private const string CDN_URL = "http://172.16.100.201/cdn";		

		private readonly string _publicKey;
		private readonly IScreenFactory _screenFactory;
		private readonly IVersionService _versionService;


		public OBBFetcherFactory(string publicKey, IScreenFactory screenFactory, IVersionService versionService)
		{
			if(string.IsNullOrEmpty(publicKey) || screenFactory == null)
			{
				throw new ArgumentNullException();
			}

			_publicKey = publicKey;
			_screenFactory = screenFactory;
			_versionService = versionService;
		}

		public IExpansionFileFetcher Create(string url)
		{		
			if (RequiresOBB)
			{
				if(FromGoogle(url))		
				{
					return new GooglePlayOBBFetcher(_publicKey, _screenFactory);
				}
				else if(FromLocalCDN(url))
				{
					return new RemoteOBBFetcher(CDN_URL, _screenFactory);
				}
				else
				{
					throw new ArgumentException("OBBFetcherFactory::Create >>> Unrecognized fetch state: " + url);
				}
			}
			else
			{
				return new PassThruOBBFetcher();
			}
		}

		private bool FromGoogle(string url)
		{
			return url == "GooglePlay";
		}

		private bool FromLocalCDN(string url)
		{
			return string.IsNullOrEmpty (url);
		}

		private bool RequiresOBB
		{
			get { return _versionService.Environment == ClientEnvironment.PRODUCTION && !UnityEngine.Application.isEditor; }
		}



    }
    
}




