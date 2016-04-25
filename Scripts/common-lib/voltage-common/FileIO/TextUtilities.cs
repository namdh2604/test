using System;

namespace Voltage.Common.FileIO
{
	using System.IO;

	using Voltage.Common.Serialization;


	public class TextUtilities : ISerializer<string> //ISaveable<string>
	{

		public bool Serialize (string input, string filepath)	// FIXME: conform with ISerializer!!!
		{
			return Write (input, filepath, true);
		}


		public static bool WriteToFile(string input, string path, bool overwrite=false)	// FIXME: conform with IFileWriter!!!
		{
			return new TextUtilities ().Write (input, path, overwrite);
		}

		public bool Write(string input, string path, bool overwrite=false)	// FIXME: conform with IFileWriter!!!
		{
			if(!string.IsNullOrEmpty(path))
			{
				string output = string.Empty;

				if(!overwrite && FileUtilities.PathExists(path))
				{
					output = File.ReadAllText(path);
				}

				output += input;					// NOTE: adds input inline!!!
				File.WriteAllText(path, output);

				return true;
			}

			return false;
		}


		public static void ReplaceLineInFileMatching (Predicate<string> predicate, string newText, string path)
		{
//			Debug.Log(string.Format ("Match Replace line with <{0}> in: {1}", newText, path));
			
//			try
//			{
				string targetLine = FindLineInFile (predicate, path);
				
				if (!string.IsNullOrEmpty(targetLine))
				{
					ReplaceLineInFile(targetLine, newText, path);
				}
				else
				{
//					Debug.LogWarning ("QGBuildEditor::ReplaceLineInFileMatching >>>> Could not match line");
				}
//			}
//			catch(Exception e)
//			{
//				Debug.LogWarning(string.Format("QGBuildEditor::ReplaceLineInFileMatching  >>> {0}: {1}\n[{2}, {3}]\n\n{4}", e.GetType(), e.Message, newText, path, e.StackTrace));
//			}
		}
		
		public static string FindLineInFile(Predicate<string> predicate, string path)
		{
			if (FileUtilities.PathExists(path))
			{
				foreach(string line in File.ReadAllLines(path))
				{
					if(predicate(line))
					{
						return line;
					}
				}
			}
			
			return string.Empty;
		}
		
		public static void ReplaceLineInFile (string oldText, string newText, string path)
		{
//			Debug.Log(string.Format ("Replacing line <{0}> with <{1}> in: {2}", oldText, newText, path));
			
//			try
//			{
				if (FileUtilities.PathExists(path))
				{
					string text = File.ReadAllText(path);
					text = text.Replace(oldText, newText);
					File.WriteAllText(path, text);
				}
//			}
//			catch(Exception e)
//			{
//				Debug.LogWarning(string.Format("QGBuild::ReplaceLineInFile >>> {0}: {1}\n[{2}, {3}, {4}]\n\n{5}", e.GetType(), e.Message, oldText, newText, path, e.StackTrace));
//			}
		}

	}
}