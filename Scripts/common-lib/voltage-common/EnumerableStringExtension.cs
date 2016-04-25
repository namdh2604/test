using System.Collections.Generic;

namespace Voltage.Common.DebugTools.Misc
{
	public static class EnumerableStringExtension
	{
		public static string ToEnumerableString<T> (this IEnumerable<T> enumerable) //, string format="{0}\n", params Object[] args)
		{
			string content = string.Empty;

			foreach (T element in enumerable)
			{
				content += string.Format("{0}\n", element.ToString());
			}

			return content;
		}
	}
}

