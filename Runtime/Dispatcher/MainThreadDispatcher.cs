using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityUtilities.Dispatcher
{
    [ExecuteAlways]
    public class MainThreadDispatcher : MonoBehaviour
    {
        private static readonly ConcurrentQueue<Action> _executionQueue = new ConcurrentQueue<Action>();
        private static MainThreadDispatcher _instance;
        private static bool applicationIsQuitting = false;
        public static MainThreadDispatcher Instance
        {
            get
            {
                if (applicationIsQuitting)
                {
                    Debug.LogWarning("[MainThreadDispatcher] Instance already destroyed. Returning null.");
                    return null;
                }

                // If instance is already set, return it
                if (_instance != null)
                {
                    return _instance;
                }

                // Try to find existing instance in the scene
                _instance = FindObjectOfType<MainThreadDispatcher>();

                // If no instance found, create a new one
                if (_instance == null)
                {
                    var obj = new GameObject("MainThreadDispatcher");
                    _instance = obj.AddComponent<MainThreadDispatcher>();

                    if (Application.isPlaying)
                    {
                        DontDestroyOnLoad(obj);
                    }
                    else
                    {
                        obj.hideFlags = HideFlags.HideAndDontSave;
                    }
                }

                return _instance;
            }
        }

        private void OnDestroy()
        {
            if(Application.isPlaying)
                applicationIsQuitting = true;
        }
        private void OnEnable()
        {
#if UNITY_EDITOR
            EditorApplication.update += EditorUpdate;
#endif
        }
        private void OnDisable()
        {
#if UNITY_EDITOR
            EditorApplication.update -= EditorUpdate;
#endif
        }
        private void Update()
        {
            ProcessQueue();
        }

#if UNITY_EDITOR
        private void EditorUpdate()
        {
            if (!Application.isPlaying)
            {
                ProcessQueue();
            }
        }
#endif
        private void ProcessQueue()
        {
            while (_executionQueue.TryDequeue(out Action action))
            {
                action?.Invoke();
            }
        }
        public void Enqueue(Action action)
        {
            if (action != null)
            {
                _executionQueue.Enqueue(action);
            }
        }

        public static void RunOnMainThread(Action action)
        {
            Instance.Enqueue(action);
        }
    }
}
