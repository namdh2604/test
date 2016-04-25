using System;

namespace Voltage.Witches.Scheduling
{
    public interface ITaskScheduler
    {
        void Schedule(ITask task, DateTime time);
        void ScheduleRecurring(IRepeatableTask task);
        void CancelTask(ITask task);
    }
}

