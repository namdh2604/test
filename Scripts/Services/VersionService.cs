
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Services
{
	using UnityEngine;

    public class VersionService : IVersionService
    {
		private readonly IBuildNumberService _buildNumService;

//		private readonly ClientEnvironment _environment;

		public VersionService(IBuildNumberService buildNumService)
		{
			if(buildNumService == null)
			{
				throw new ArgumentNullException();
			}

			_buildNumService = buildNumService;
//			_environment = GetEnvironment (buildNumService);
		}

		private ClientEnvironment GetEnvironment(IBuildNumberService buildNumService)
		{
			string baseVersion = buildNumService.GetBaseBuildVersion();

			string[] splitVersion = baseVersion.Split('_');
			string buildEnv = string.Empty;

			if(splitVersion.Length > 1)
			{
				buildEnv = splitVersion[1];
			}

			return EvaluateEnvironmentVersion (buildEnv);
		}

		private ClientEnvironment EvaluateEnvironmentVersion (string version="")
		{
			switch (version)
			{
				case "" :	return ClientEnvironment.PRODUCTION;
//				case "s":	return ClientEnvironment.STAGING;
				case "d":	return ClientEnvironment.DEVELOPMENT;
				default :	return ClientEnvironment.CUSTOM;		// maybe should default to DEVELOPMENT
			}
		}
	


		public ClientEnvironment Environment
		{
			get
			{
				return GetEnvironment(_buildNumService);
			}
		}

		public string BuildNumber
		{
			get
			{
				return _buildNumService.GetBaseBuildVersion();
			}
		}
		



    }
    
}




