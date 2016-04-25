using System;

namespace Voltage.Witches.Screens
{
	public interface IDialog
	{
		void Display(Action<int> callback);
	}
}

