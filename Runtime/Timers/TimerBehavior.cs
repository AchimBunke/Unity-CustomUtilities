using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtilities.Timers;

namespace UnityUtilities.Timers
{
    public class TimerBehavior : MonoBehaviour, ITimer
    {
        [SerializeField] protected float interval = 5f;
        [SerializeField] protected bool autoReset = true;

        protected ManualTimer _manualTimer;

        public event Action Elapsed
        {
            add => _manualTimer.Elapsed += value;
            remove => _manualTimer.Elapsed -= value;
        }

        public bool AutoReset { get => _manualTimer.AutoReset; set => _manualTimer.AutoReset = value; }
        public bool Enabled { get => _manualTimer.Enabled; set => _manualTimer.Enabled = value; }
        public float Interval 
        { 
            get => _manualTimer.Interval;
            set => _manualTimer.Interval = value;
        }

        private void Awake()
        {
            _manualTimer = new ManualTimer(interval);
            _manualTimer.AutoReset = autoReset;
        }
        private void OnEnable()
        {
            StartTimer();
        }
        private void OnDisable()
        {
            StopTimer();
        }
        private void FixedUpdate()
        {
            _manualTimer.Update(Time.fixedDeltaTime);
        }

        public void StartTimer()
        {
            _manualTimer.StartTimer();
        }

        public void StopTimer()
        {
            _manualTimer.StopTimer();
        }
    }
}
