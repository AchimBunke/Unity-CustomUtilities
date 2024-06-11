using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtilities.Timers;

namespace UnityUtilities.Timers
{
    public class TimerBehavior : MonoBehaviour
    {
        [SerializeField] protected float interval = 1f;
        [SerializeField] protected bool autoReset = true;

        protected ManualTimer _manualTimer;
        public ITimer Timer => _manualTimer;
        private void Awake()
        {
            _manualTimer = new ManualTimer(interval);
            _manualTimer.AutoReset = autoReset;
        }
        private void OnEnable()
        {
            _manualTimer.Start();
        }
        private void OnDisable()
        {
            _manualTimer.Stop();
        }
        private void FixedUpdate()
        {
            _manualTimer.Update(Time.fixedDeltaTime);
        }
    }
}
