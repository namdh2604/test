
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Services
{
	using Voltage.Common.Logging;
	using UnityEngine;

	public class UnityFileSystemService : IFilesystemService
    {

		public string[] ListAllFiles(string path, string pattern)
		{
			throw new NotImplementedException ("UnityFileSystemService::ListAllFiles >>> May not be relevant and need to come up with a good way to return all assets in resources without loading them");
		}

		public string ReadAllText(string path)
		{
//			AmbientLogger.Current.Log ("UnityFileSystemService::ReadAllText >>> " + (!string.IsNullOrEmpty(path)?path:"null"), LogLevel.INFO);

			TextAsset asset = Resources.Load<TextAsset> (path);

//			AmbientLogger.Current.Log ("UnityFileSystemService::ReadAllText >>> TextAsset: " + (asset!=null?asset.name:"null"), LogLevel.INFO);
//			AmbientLogger.Current.Log ("UnityFileSystemService::ReadAllText >>> TextAsset.Text: " + (asset!=null&&!string.IsNullOrEmpty(asset.text)?asset.text:"null"), LogLevel.INFO);

			return asset.text;
		}

    }
    
}




