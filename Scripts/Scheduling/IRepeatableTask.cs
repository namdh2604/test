using System;

namespace Voltage.Witches.Scheduling
{
    public interface IRepeatableTask : ITask
    {
        DateTime NextExecutionTime { get; }
    }
}
