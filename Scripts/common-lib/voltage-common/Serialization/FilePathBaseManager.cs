using Voltage.Common.Logging;

namespace Voltage.Common.Serialization
{
	public class FilePathBaseManager : IReferenceAFile	
	{
		
		public ILogger Logger { get; protected set; }
		
		public string Path { get; protected set; }
		public string Filename { get; protected set; }
		public string Type { get; protected set; }
		public string FullPath 
		{ 
			get 
			{ 
				string fullpath = string.Empty;
				if(!string.IsNullOrEmpty(Path) && !string.IsNullOrEmpty(Filename))
				{
					fullpath = string.Format ("{0}/{1}{2}", Path, Filename, !string.IsNullOrEmpty(Type) ? "."+Type : string.Empty); 
				}
				
				return fullpath;
			} 
		}

		protected FilePathBaseManager(string path, string filename, string type, ILogger logger)	// FIXME: correct to allow for fullpath
		{	
			ConfigurePath (path, filename, type);
			Logger = logger;
		}
		
		private void ConfigurePath(string path, string filename, string type)
		{
			if (!string.IsNullOrEmpty(path))
			{
				path = path.EndsWith("/") ? path.Substring(0, path.Length-1) : path;
			}

			Path = path;
			Filename = filename;
			Type = type;
		}
		
	}
}

