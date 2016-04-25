using System;

namespace Voltage.Story.Exceptions
{
    public class NodeConstructionError : Exception
    {
        public int LineNumber { get; protected set; }
        public string Body { get; protected set; }

        public NodeConstructionError(int lineNumber, string body, string message)
            : base(CreateErrorMessage(lineNumber, body, message))
        {
            LineNumber = lineNumber;
            Body = body;
        }

        private static string CreateErrorMessage(int lineNumber, string body, string message)
        {
            string errorFmt = "Line {0}: {1}. Contents {2}";
            return string.Format(errorFmt, lineNumber, message, body);
        }
    }
}

