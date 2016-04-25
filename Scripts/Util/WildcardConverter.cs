using System.Text.RegularExpressions;

namespace Voltage.Witches.Util
{
    public static class WildcardConverter
    {
        public static string ToRegex(string pattern)
        {
            return "^" + Regex.Escape(pattern)
                .Replace(@"\*", ".*")
                .Replace(@"\?", ".") + "$";
        }
    }
}

