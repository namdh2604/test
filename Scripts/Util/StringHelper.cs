using System;

namespace Voltage.Witches.Util
{
    // various helper methods for strings
    public static class StringHelper
    {
        // an extension method to capitalize the first character of a string
        public static string Capitalize(this string value)
        {
            if (value.Length <= 0)
            {
                return value;
            }

            return value[0].ToString().ToUpper() + value.Substring(1);
        }
    }
}

