using System;
using System.Runtime.InteropServices.WindowsRuntime;

namespace UnityUtilities.Timers
{
    public class ManualTimer : ITimer
    {
        public event Action Elapsed;
        public bool AutoReset { get; set; } = true;
        private bool _enabled = false;
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled == value)
                    return;
                _enabled = value;
            }
        }

        private float _interval = 100;
        public float Interval
        {
            get => _interval;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Interval must be greater than 0");
                _elapsedTime = 0;
                _interval = value;
            }
        }
        public ManualTimer()
        {
        }
        public ManualTimer(float interval) : this()
        {
            Interval = interval;
        }
        private float _elapsedTime = 0;
        public void Update(float deltaTimeSeconds)
        {
            if (!Enabled)
                return;
            _elapsedTime += deltaTimeSeconds;
            if(_elapsedTime >= Interval)
            {
                Elapsed?.Invoke();
                if (!AutoReset)
                    Enabled = false;
                _elapsedTime = 0;
            }

        }
        public void StartTimer()
        {
            Enabled = true;
        }
        public void StopTimer()
        {
            Enabled = false;
        }
    }
}
