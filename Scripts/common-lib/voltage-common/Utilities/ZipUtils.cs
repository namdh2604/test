using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace Voltage.Common.Utilities
{
    public static class ZipUtils
    {
    	public static void UnzipToDir(string zipFile, string destPath)
    	{
    		using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFile)))
    		{
    			ZipEntry entry;
    			while ((entry = s.GetNextEntry()) != null)
    			{
    				string dirName = Path.GetDirectoryName(entry.Name);
    				string fileName = Path.GetFileName(entry.Name);
    				string qualifiedPath;

    				if (!Path.IsPathRooted(dirName))
    				{
    					dirName = Path.Combine(destPath, dirName);
    					qualifiedPath = Path.Combine(dirName, fileName);
    				}
    				else
    				{
    					qualifiedPath = entry.Name;
    				}

    				if (dirName.Length > 0)
    				{
    					Directory.CreateDirectory(dirName);
    				}

    				if (fileName != string.Empty)
    				{
    					using (FileStream streamWriter = File.Create(qualifiedPath))
    					{
    						int size = 2048;
    						byte[] data = new byte[2048];
    						while (true)
    						{
    							size = s.Read(data, 0, data.Length);
    							if (size > 0)
    							{
    								streamWriter.Write(data, 0, size);
    							}
    							else
    							{
    								break;
    							}
    						}
    					}
    				}
    			}
    		}
    	}
    }
}

