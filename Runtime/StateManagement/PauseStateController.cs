using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtilities.Singleton;

namespace UnityUtilities.StateManagement
{
    public class PauseStateController : SingletonMonoBehaviour<PauseStateController>
    {
        private float previousTimeScale = 0;

        private bool pause;
        public bool Pause
        {
            get { return pause; }
            set
            {
                if (pause == value)
                    return;
                pause = value;
                OnPause(pause);
            }
        }
        public event Action<bool> PauseChanged;
        private void OnPause(bool pause)
        {
            if (pause)
                previousTimeScale = Time.timeScale;
            Time.timeScale = pause ? 0 : previousTimeScale;
            AudioListener.pause = pause;
            PauseChanged?.Invoke(pause);
        }
    }
}