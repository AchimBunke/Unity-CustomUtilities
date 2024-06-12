using System;

namespace UnityUtilities.Timers
{
    public interface ITimer
    {
        public event Action Elapsed;
        public bool AutoReset { get; set; }
        public bool Enabled { get; set; }
        public float Interval { get; set; }
        public void StartTimer();
        public void StopTimer();
    }
}
