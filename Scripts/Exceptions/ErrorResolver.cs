using System;

namespace Voltage.Witches.Exceptions
{
    public class ErrorResolver
    {
        public ErrorResolver()
        {
        }

        private const string ERROR_NAMESPACE = "Voltage.Witches.Exceptions.";
        private const string APPLICATION_MSG = "An Application Error occurred";
        private const string GENERIC_MSG = "An Unknown Error occurred";

        public string GetUserFacingErrorMessage(Exception e)
        {
            if (typeof(UserFacingException).IsInstanceOfType(e))
            {
                return e.Message;
            }
            else if (typeof(WitchesException).IsInstanceOfType(e))
            {
                return APPLICATION_MSG;
            }

            return GENERIC_MSG;
        }

        // unwind Unity's wrapping of exceptions to access the real exception
        public string GetErrorText(string condition, bool showRawText)
        {
            int separatorIndex = condition.IndexOf(':');

            string errorClass = GetClassName(condition, separatorIndex);

            string voltageExceptionName = ERROR_NAMESPACE + errorClass;

            if (showRawText)
            {
                // Always display the actual exception text
                return GetErrorMessage(condition, separatorIndex);
            }

            // otherwise make sure the error message is appropriate for the user
            Type t = Type.GetType(voltageExceptionName);
            if (t != null)
            {
                if ((t == typeof(UserFacingException)) || (t.IsSubclassOf(typeof(UserFacingException))))
                {
                    // User Facing Exceptions return text that is safe for output
                    return GetErrorMessage(condition, separatorIndex);
                }
                else
                {
                    // It's our exception, but the text should be sanitized
                    return APPLICATION_MSG;
                }
            }

            // otherwise, this isn't our exception, so just label it as a generic one
            return GENERIC_MSG;
        }

        private string GetErrorMessage(string condition, int separatorIndex)
        {
            if ((separatorIndex != -1) && (condition.Length > separatorIndex + 1))
            {
                return condition.Substring(separatorIndex + 1);
            }

            return string.Empty;
        }

        private string GetClassName(string context, int separatorIndex)
        {
            // The format of unity error messages is:
            // <ErrorClassName>: <ErrorMessage>
            // but if no error message is present, it just looks like:
            // <ErrorClassName>
            if (separatorIndex != -1)
            {
                return context.Substring(0, separatorIndex);
            }
            else
            {
                return context;
            }
        }
    }
}

