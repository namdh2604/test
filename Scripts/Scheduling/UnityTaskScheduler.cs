using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voltage.Witches.Scheduling
{
    using Voltage.Witches.Services;

    public class UnityTaskScheduler : MonoBehaviour, ITaskScheduler
    {
        private Dictionary<ITask, Coroutine> _coroutines;
        private HashSet<ITask> _executingTasks;

        private Dictionary<ITask, Coroutine> _repeatingCoroutines;

        private void Awake()
        {
            // HACK: Lifetime management should be included in DI, not here.
            DontDestroyOnLoad(gameObject);

            _coroutines = new Dictionary<ITask, Coroutine>();
            _executingTasks = new HashSet<ITask>();

            _repeatingCoroutines = new Dictionary<ITask, Coroutine>();
        }

        public void Schedule(ITask task, DateTime time)
        {
            var routine = StartCoroutine(ScheduleCoroutine(task, time));
            if (_executingTasks.Contains(task))
            {
                _coroutines[task] = routine;
            }
        }

        public void ScheduleRecurring(IRepeatableTask task)
        {
            var routine = StartCoroutine(ScheduleRecurringCoroutine(task));
            _repeatingCoroutines[task] = routine;
        }

        private IEnumerator ScheduleCoroutine(ITask task, DateTime time)
        {
            _executingTasks.Add(task);

            float numSeconds = ConvertTimeToSeconds(time);
            yield return new WaitForSeconds(numSeconds);
            task.Execute();

            _executingTasks.Remove(task);
            _coroutines.Remove(task);
        }

        private IEnumerator ScheduleRecurringCoroutine(IRepeatableTask task)
        {
            DateTime startTime = TimeService.Current.UtcNow;

            while (true)
            {
                var routine = StartCoroutine(ScheduleCoroutine(task, startTime));
                if (_executingTasks.Contains(task))
                {
                    _coroutines[task] = routine;
                }
                yield return routine;
                startTime = task.NextExecutionTime;
            }
        }

        // will cancel both repeating and normal tasks
        public void CancelTask(ITask task)
        {
            if (_repeatingCoroutines.ContainsKey(task))
            {
                StopCoroutine(_repeatingCoroutines[task]);
                _repeatingCoroutines.Remove(task);
            }

            if (_coroutines.ContainsKey(task))
            {
                StopCoroutine(_coroutines[task]);
                _coroutines.Remove(task);
            }
        }

        private float ConvertTimeToSeconds(DateTime time)
        {
            DateTime currentTime = TimeService.Current.UtcNow;
            TimeSpan span = time.Subtract(currentTime);
            float totalSeconds = (span.TotalSeconds < 0.0f) ? 0.0f : (float)span.TotalSeconds;
            return totalSeconds;
        }

		private void OnDestroy()
		{
            // TODO: Possible memory leak if the contained coroutine references are not cleared
			StopAllCoroutines();
		}

        public int GetRecurringCount()
        {
            return _repeatingCoroutines.Count;
        }

        public int GetTaskCount()
        {
            return _coroutines.Count;
        }
    }
}

