using System;

namespace Voltage.Witches.Services
{
    public static class TimeService
    {
        private static ITimeService _current;

        public static ITimeService Current {
            get
            {
                if (_current == null)
                {
                    _current = new DefaultTimeService();
                }
                return _current;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("time service cannot be null");
                }
                _current = value;
            }
        }
    }

    public interface ITimeService
    {
        DateTime UtcNow { get; }
    }

    public class DefaultTimeService : ITimeService
    {
        public DateTime UtcNow { get { return DateTime.UtcNow; } }
    }
}

