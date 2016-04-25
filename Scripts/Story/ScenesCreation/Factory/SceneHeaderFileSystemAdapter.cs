
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Services
{
	using Voltage.Common.Logging;
	using UnityEngine;

	using System.Text;

	public class SceneHeaderFileSystemAdapter : IFilesystemService
	{
		
		public string[] ListAllFiles(string path, string pattern)
		{
			throw new NotImplementedException ("UnityFileSystemService::ListAllFiles >>> May not be relevant and need to come up with a good way to return all assets in resources without loading them");
		}
		
		public string ReadAllText(string path)	// expects this to be the same format as SceneManifest
		{
			StringBuilder strBuilder = new StringBuilder (path);
			strBuilder.Replace ("Story/Scenes", "JSON/STORY/Headers");
			strBuilder.Replace (".json", string.Empty);

			return Resources.Load<TextAsset> (strBuilder.ToString()).text;	// Resources.UnloadAsset?
		}
		
	}
    
}




