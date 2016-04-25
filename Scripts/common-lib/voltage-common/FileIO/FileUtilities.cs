using System.IO;

namespace Voltage.Common.FileIO
{
	public class FileUtilities
	{
		public static void RenameDirectory (string srcPath, string newPath)
		{
			if (PathExists(srcPath) && !string.IsNullOrEmpty(newPath) && srcPath != newPath)
			{
				Directory.Move (srcPath, newPath);
			}
		}	
		
		public static void ClearDirectory(string path)
		{
			if(!string.IsNullOrEmpty(path) && PathExists(path))
			{
				DirectoryInfo dirInfo = new DirectoryInfo(path);
				
				foreach (FileInfo file in dirInfo.GetFiles())
				{
					file.Delete();
				}
				
				foreach (DirectoryInfo dir in dirInfo.GetDirectories())
				{
					dir.Delete();
				}
			}
		}
		
		public static bool PathExists (string path)
		{
			return !string.IsNullOrEmpty (path) && (Directory.Exists (path) || File.Exists (path));
		}

		public static void DeleteFile(string path)
		{
			if (File.Exists(path))
			{
				File.Delete(path);
			}
		}

		public static void DeleteDirectory(string path, bool recursive=true)
		{
			if (Directory.Exists(path))
			{
				Directory.Delete(path,recursive);
			}
		}

	}
}

