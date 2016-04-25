using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

using ICSharpCode.SharpZipLib.Zip;

namespace Voltage.Witches.Services
{
    using Voltage.Common.Utilities;

    public class FilesystemService : IFilesystemService
    {
        private readonly string _root;
        public FilesystemService()
        {
            _root = StreamingAssetsHelper.GetPath();
        }

        public string[] ListAllFiles(string path, string pattern)
        {
            SearchOption option = SearchOption.AllDirectories;
            string fullpath = TranslatePath(path);
            var actualPaths = Directory.GetFiles(fullpath, pattern, option);
            int prefixLength = _root.Length + 1;
            return actualPaths.Select(x => x.Substring(prefixLength)).ToArray();
        }

        public string ReadAllText(string path)
        {
            string fullpath = TranslatePath(path);

            return File.ReadAllText(fullpath);
        }

        private string TranslatePath(string path)
        {
            return _root + "/" + path;
        }
    }
}

