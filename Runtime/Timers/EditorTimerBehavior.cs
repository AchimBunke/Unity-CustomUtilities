using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityUtilities.Timers
{
    [ExecuteInEditMode]
    public class EditorTimerBehavior : TimerBehavior
    {
        [SerializeField] protected float unfocusedEditorInterval = 60000f;
        [SerializeField, HideInInspector] float _intervalCheck;
        [SerializeField, HideInInspector] bool _autoResetCheck;

        private void OnValidate()
        {
            if (_intervalCheck != interval)
            {
                Timer.Interval = interval;
                _intervalCheck = interval;
            }

            if (_autoResetCheck != autoReset)
            {
                Timer.AutoReset = autoReset;
                _autoResetCheck = autoReset;
            }
        }
        private void Awake()
        {
            _manualTimer = new ManualTimer(interval);
            _manualTimer.AutoReset = autoReset;
        }
        private void OnEnable()
        {
            if (_manualTimer == null)
                _manualTimer = new ManualTimer(interval);
            EditorApplication.focusChanged += OnEditorFocusChanged;
            _manualTimer.Start();
        }
        private void OnDisable()
        {
            _manualTimer.Stop();
            EditorApplication.focusChanged -= OnEditorFocusChanged;
        }
        private static double lastUpdateTime = 0;
        private void Update()
        {
            double currentTime = EditorApplication.timeSinceStartup;
            double deltaTime = currentTime - lastUpdateTime;
            _manualTimer.Update((float)deltaTime);
            lastUpdateTime = currentTime;
        }
        private void OnEditorFocusChanged(bool hasFocus)
        {
            _manualTimer.Interval = hasFocus ? interval : unfocusedEditorInterval;
        }
        private void FixedUpdate()
        {
            
        }

    }
}
