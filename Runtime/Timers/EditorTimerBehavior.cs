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
        [SerializeField] protected bool _runInPlayMode = false;

        private void OnValidate()
        {
            if (_intervalCheck != interval)
            {
                _manualTimer.Interval = interval;
                _intervalCheck = interval;
            }

            if (_autoResetCheck != autoReset)
            {
                _manualTimer.AutoReset = autoReset;
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
            if(!_runInPlayMode && Application.isPlaying)
            {
                this.enabled = false;
            }
            if (_manualTimer == null)
            {
                _manualTimer = new ManualTimer(interval);
                _manualTimer.AutoReset = autoReset;
            }
            if(!Application.isPlaying)
                EditorApplication.focusChanged += OnEditorFocusChanged;
            StartTimer();
        }
        private void OnDisable()
        {
            StopTimer();
            if(!Application.isPlaying)
                EditorApplication.focusChanged -= OnEditorFocusChanged;
        }
        private double lastUpdateTime = 0;
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
    [CustomEditor(typeof(EditorTimerBehavior))]
    public class EditorTimerBehaviorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorTimerBehavior editorTimerBehavior = (EditorTimerBehavior)target;
            if (editorTimerBehavior.isActiveAndEnabled)
            {
                if (GUILayout.Button(editorTimerBehavior.Enabled ? "Stop" : "Start"))
                {
                    if (editorTimerBehavior.Enabled)
                        editorTimerBehavior.StopTimer();
                    else
                        editorTimerBehavior.StartTimer();
                }
            }

        }
    }
}
