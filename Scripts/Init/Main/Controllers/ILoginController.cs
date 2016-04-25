
using System;
using Voltage.Witches.User;

namespace Voltage.Witches.Login
{
	using Voltage.Witches.DI;

	public interface ILoginController
	{
		void Execute (StartupData startupData, Action<Exception, bool> onComplete);
	}
}