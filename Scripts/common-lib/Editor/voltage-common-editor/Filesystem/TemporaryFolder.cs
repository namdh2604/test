using System;
using System.IO;

using UnityEditor;

namespace Voltage.Common.Filesystem
{
    public class TemporaryFolder : IDisposable
    {
        private readonly string _path;

        public TemporaryFolder(string path)
        {
            if (Directory.Exists(path))
            {
                throw new Exception("Temporary Directory already exists!");
            }

            _path = path;
            Directory.CreateDirectory(_path);
        }

        public void Dispose()
        {
            AssetDatabase.DeleteAsset(_path);
        }
    }
}