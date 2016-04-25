using System;
using UnityEngine;

namespace Voltage.Witches.Scheduling
{
    using Voltage.Witches.Scheduling.Tasks;

    public class SchedulingTest : MonoBehaviour
    {
        private UnityTaskScheduler _scheduler;
        public int SecondsFromNow = 60;
//        private System.Random _gen;



        private void Awake()
        {
//            _gen = new System.Random();
            _scheduler = GameObject.FindObjectOfType<UnityTaskScheduler>();
        }

        public void ScheduleTask()
        {
            _scheduler.ScheduleRecurring(new DummyTask(5));
//            int key = _gen.Next();
//            DateTime now = DateTime.UtcNow;
//            DateTime scheduledFor = now.AddSeconds(SecondsFromNow);
//
//            _scheduler.Schedule(new DoStuff(), scheduledFor);
//            Debug.LogWarning(string.Format("{0}: Queued up task {1} which should trigger at {2}", now, key, scheduledFor));
        }

        private class DoStuff : ITask
        {
            public void Execute()
            {
                Debug.LogWarning("I got triggered");
            }

            public string Name { get { return "DoStuff"; } }
        }

    }

}

