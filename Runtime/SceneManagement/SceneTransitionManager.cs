using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUtilities.Singleton;

namespace UnityUtilities.SceneManagement
{
    public class SceneTransitionManager : SingletonMonoBehaviour<SceneTransitionManager>
    {
        /// <summary>
        /// loading progress of scene
        /// </summary>
        public event Action<float> LoadingProgress;
        /// <summary>
        /// waiting for CanCompleteTransition
        /// </summary>
        public event Action<bool> LoadingProgressWaiting;

        public event Action<bool> PausedRequested;
        public bool CanCompleteTransition { get; set; }

        public void AutoTransition(string targetSceneName,
            string transitionSceneName,
            float minTransitionTime = 0,
            bool autoUnloadTransitionScene = true,
            bool backgroundActivation = true)
        {
            if (SceneManager.GetSceneByName(targetSceneName).isLoaded)
                return;
            StopAllCoroutines();
            StartCoroutine(TransitionRoutine(targetSceneName, transitionSceneName, minTransitionTime, autoUnloadTransitionScene, backgroundActivation));
        }

        IEnumerator TransitionRoutine(string targetSceneName,
            string transitionSceneName,
            float minTransitionTime,
            bool autoUnloadTransitionScene,
            bool backgroundActivation)
        {
            var transitionOp = SceneManager.LoadSceneAsync(transitionSceneName, LoadSceneMode.Single);
            var gameOp = SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Additive);


            gameOp.allowSceneActivation = backgroundActivation;
            float transitionTime = 0;
            PausedRequested?.Invoke(true); // prevent anything from happening until loading screen finished
                                                        // While loading or below min transition time
            while (!gameOp.isDone || transitionTime < minTransitionTime)
            {
                LoadingProgress?.Invoke(gameOp.progress);
                // if scene finished loading and above transition time and wait for player signal (otherwise progress stuck at 0.9 and isDone always false)
                if (!backgroundActivation && gameOp.progress >= 0.9f && transitionTime >= minTransitionTime)
                {
                    break;
                }
                yield return new WaitForEndOfFrame();
                transitionTime += Time.unscaledDeltaTime;
            }
            if (!autoUnloadTransitionScene)
            {
                LoadingProgressWaiting?.Invoke(true);
                // Wait for user input to finish transition
                while (!CanCompleteTransition)
                {
                    yield return new WaitForEndOfFrame();
                }
                CanCompleteTransition = false;
            }
            if (!backgroundActivation) // we need to wait until fully loaded
            {
                gameOp.allowSceneActivation = true;
                while (!gameOp.isDone)
                {
                    yield return new WaitForEndOfFrame();
                }
            }
            var targetScene = SceneManager.GetSceneByName(targetSceneName);
            SceneManager.SetActiveScene(targetScene);

            var unloadOp = SceneManager.UnloadSceneAsync(transitionSceneName);  // Finished loading so unload transition scene
            while (!unloadOp.isDone)
            {
                yield return new WaitForEndOfFrame();
            }

            PausedRequested?.Invoke(false);
        }

        public void AutoTransition(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            var op = SceneManager.LoadSceneAsync(sceneName, mode);
        }
        public void UnloadCurrentScene(GameObject obj)
        {
            var scene = obj.scene;
            SceneManager.UnloadSceneAsync(scene.buildIndex);
        }
        public void UnloadScene(string SceneName)
        {
            SceneManager.UnloadSceneAsync(SceneName);
        }
    }
}
