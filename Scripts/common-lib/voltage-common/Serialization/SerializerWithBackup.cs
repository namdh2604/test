using System;
using System.IO;
using Voltage.Common.Logging;

namespace Voltage.Common.Serialization
{
	public class SerializerWithBackup<T> : ISerializer<T>, IDeserializer<T> //where T: class
	{
		public ISerializer<T> Serializer { get; set; }
		public IDeserializer<T> Deserializer { get; set; }
		public ILogger Logger { get; private set; }
		
		public SerializerWithBackup (ISerializer<T> serializer, IDeserializer<T> deserializer, ILogger logger)
		{
			Serializer = serializer;
			Deserializer = deserializer;
			Logger = logger;
		}
		
		public string ComposeFullPath (string path, string filename, string type="")	// FIXME: 
		{
			string fullpath = string.Empty;
			
			if(!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(filename))
			{
				path = path.Replace("//","/");
				path = path.EndsWith("/") ? path.Substring(0, path.Length-1) : path;
				fullpath = string.Format ("{0}/{1}{2}", path, filename, !string.IsNullOrEmpty(type) ? "."+type : string.Empty);
			}
			
			return fullpath;
		}
		
		
		public bool Serialize (T data, string path, string filename, string type="", bool overwrite=true)	
		{
			if(!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(filename))
			{
				string filepath = ComposeFullPath (path, filename, type);
				return Serialize (data, filepath, overwrite);
			}
			
			return false;
		}
		public bool Serialize (T data, string filepath) { return Serialize (data, filepath, true); }
		public bool Serialize (T data, string filepath, bool overwrite) //=true)	// TODO: implement overwrite	
		{
			if(!string.IsNullOrEmpty(filepath) && Serializer != null)
			{
				Logger.Log ("start of writing out file: " + filepath, LogLevel.INFO);
				
				string backup = filepath + ".bak";
				if (File.Exists(filepath) && !File.Exists(backup))	// should check if backup doesn't exist ? 
				{
					Logger.Log ("backing up file: " + filepath, LogLevel.INFO);
					File.Move(filepath, backup);
				}
				
				if(Serializer.Serialize (data, filepath))
				{
					if(File.Exists(backup))
					{
						Logger.Log ("removing backup", LogLevel.INFO);
						File.Delete(backup);
					}
					
					Logger.Log ("successfully wrote out file: " + filepath, LogLevel.INFO);
					return true;
				}
				else 
				{
					File.Delete(filepath);		// cleanup incomplete file
					
					if(File.Exists(backup))
					{
						Logger.Log("error, restoring backup", LogLevel.WARNING);
						File.Move(backup, filepath);
					}
				}
			}
			
			Logger.Log ("serialization error", LogLevel.WARNING);
			return false;
		}
		
		
		public bool ValidPath (string path, string file, string type)
		{
			throw new NotImplementedException ();
		}
		
		
		//		string path = string.Format ("{0}/{1}", Path, fileNameAndType);
		public T Deserialize (string path, string filename, string type="")
		{
			if(!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(filename))
			{
				string filepath = ComposeFullPath (path, filename, type);
				return Deserialize (filepath);
			}
			
			return default(T);
		}
		public T Deserialize (string filepath) //, T reference)
		{
			if(!string.IsNullOrEmpty(filepath) && Deserializer != null)
			{
				Logger.Log ("reading in file: " + filepath, LogLevel.INFO);
				if (File.Exists (filepath))
				{
					return Deserializer.Deserialize(filepath);
				}
				else
				{
					Logger.Log ("no file at path: " + filepath, LogLevel.WARNING);
				}
			}
			
			Logger.Log ("error reading in file", LogLevel.WARNING);
			return default(T);
		}
	}
}
