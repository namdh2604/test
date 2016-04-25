using System;
using UnityEngine;

namespace Voltage.Witches.Scheduling.Tasks
{
    public class DummyTask : IRepeatableTask
    {
        private int _count;
        private DateTime _next;
        private readonly int _delay;

        public DummyTask(int delay)
        {
            _count = 0;
            _next = DateTime.UtcNow;
            _delay = delay;
        }

        public void Execute()
        {
            Debug.LogWarning(_count + ": Executing Dummy task");
            _next = _next.AddSeconds(_delay);
        }

        public DateTime NextExecutionTime
        {
            get { return _next; }
        }

        public string Name { get { return "DummyTask"; } }
    }
}

