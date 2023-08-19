using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUtilities.AssetManagement;
using UnityUtilities.Singleton;

namespace UnityUtilities.SceneManagement
{
    public class RuntimeSceneCompositor : MonoBehaviour
    {
        private void Awake()
        {
            LightProbes.needsRetetrahedralization += LightProbes_needsRetetrahedralization;
            LoadSceneSetup();
        }
        string GetSceneSetupLocation(Scene activeScene)
        {
            return "Assets/Scenes/" + activeScene.name + "/" + "SceneSetupData.asset";
        }
        string GetSceneName(string sceneLocation)
        {
            return Path.GetFileNameWithoutExtension(sceneLocation);
        }

        private void LoadSceneSetup()
        {
            var location = GetSceneSetupLocation(gameObject.scene);
            var bundle = AssetBundleManager.LoadAssetBundle(Path.Combine(Application.streamingAssetsPath, $"{gameObject.scene.name}"));
            if (bundle == null)
            {
                Debug.Log("Failed to load AssetBundle!");
            }
            var sceneSetupData = bundle.LoadAsset<SceneSetupData>("SceneSetupData");
            var missingScenes = sceneSetupData.IncludedScenes.Where(s => !SceneManager.GetSceneByPath(s).IsValid());
            foreach (var scene in missingScenes)
            {
                var sceneName = GetSceneName(scene);
                SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            }

        }

        private void LightProbes_needsRetetrahedralization()
        {
            LightProbes.Tetrahedralize();
        }
    }
}
