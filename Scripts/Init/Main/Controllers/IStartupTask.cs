
using System;

namespace Voltage.Common.Startup
{
	public interface IStartupTask
	{
		void Execute (Action onComplete);
	}

    public interface IStartupDataTask<T> : IStartupTask
    {
		T Data { get; }
    }
    
}



