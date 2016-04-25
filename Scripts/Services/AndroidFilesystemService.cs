using System;
using System.IO;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using UnityEngine;
using System.Text.RegularExpressions;

namespace Voltage.Witches.Services
{
    using Voltage.Witches.Util;

    public class AndroidFilesystemService : IFilesystemService
    {
        private const string PATH_PREFIX = "assets";

        public AndroidFilesystemService()
        {
        }

        public string[] ListAllFiles(string path, string pattern)
        {
            List<string> names = new List<string>();

            using (Stream inStream = File.OpenRead(Application.dataPath))
            {
                ZipFile zipfile = new ZipFile(inStream);

                string prefix = TranslatePath(path);
                string regexPattern = WildcardConverter.ToRegex(pattern);

                foreach (ZipEntry entry in zipfile)
                {
                    if ((entry.Name.StartsWith(prefix)) && (Regex.IsMatch(entry.Name, regexPattern)))
                    {
                        var entryName = entry.Name.Substring(PATH_PREFIX.Length + 1);
                        names.Add(entryName);
                    }
                }
            }

            return names.ToArray();
        }

        public string ReadAllText(string path)
        {
            using (Stream inStream = File.OpenRead(Application.dataPath))
            {
                ZipFile zipfile = new ZipFile(inStream);

                string fullPath = TranslatePath(path);
                ZipEntry entry = zipfile.GetEntry(fullPath);
                Stream zipStream = zipfile.GetInputStream(entry);

                StreamReader reader = new StreamReader(zipStream);
                return reader.ReadToEnd();
            }
        }

        private string TranslatePath(string path)
        {
            return PATH_PREFIX + "/" + path;
        }
    }
}

