using System.Collections.Generic;

namespace Voltage.Common.Utilities.Extensions
{
	public static class DictionaryExtensions
	{
		// copy/paste > http://stackoverflow.com/questions/3982448/adding-a-dictionary-to-another
		public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> source)
		{
			if(target != null && source != null)
			{
				foreach (var element in source)
				{
					target.Add(element);
				}
			}
		}
	}
}

