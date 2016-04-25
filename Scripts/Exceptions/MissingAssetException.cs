using System;

namespace Voltage.Witches.Exceptions
{
    public class MissingAssetException : WitchesException
    {
        public string Path { get; protected set; }

        public MissingAssetException(string path) : base(path)
        {
            Path = path;
        }
    }
}

