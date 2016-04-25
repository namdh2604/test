
using System;

namespace Voltage.Common.Utils
{
    public interface ITimer
    {
		void StartTimer(float duration);
//		void Reset();
		void StopTimer();

		event Action OnTimeOut;
    }
    
}



