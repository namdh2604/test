using System.IO;

namespace Voltage.Common.Filesystem
{
    public static class UnityPath
    {
        public static string GetContainingFolderName(string path)
        {
            return Path.GetFileName(Path.GetDirectoryName(path));
        }
    }
}

