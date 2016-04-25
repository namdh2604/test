using System.Collections.Generic;
using Voltage.Witches.Controllers;
using System;

namespace Voltage.Witches.Util
{
	public static class ScreenPathUtil
	{
		private const string ROOT = "/";

		public static string GetPath(Stack<IScreenController> screens)
		{
			int screenLength = screens.Count;

			string[] names = new string[screenLength];

			IScreenController[] screenArray = screens.ToArray();
			for (int i = screens.Count - 1; i >= 0; --i)
			{
				names[screenLength - i - 1] = screenArray[i].Name;
			}

			return ROOT + string.Join("/", names);
		}
	}
}

