using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityUtilities.SceneManagement;

namespace UnityUtilities.AssetManagement
{
    public class EditorSceneCompositor
    {
        [MenuItem("Tools/Save Scene Setup")]
        static void SaveSceneSetup()
        {
            var sceneSetup = EditorSceneManager.GetSceneManagerSetup();
            var activeScene = EditorSceneManager.GetActiveScene();
            var sceneSetupData = new SceneSetupData() { IncludedScenes = sceneSetup.Select(s => s.path).ToArray() };
            string sceneDataLocation = GetSceneSetupLocation(activeScene);
            if (!Directory.Exists(Path.GetDirectoryName(sceneDataLocation)))
                Directory.CreateDirectory(Path.GetDirectoryName(sceneDataLocation));
            AssetDatabase.CreateAsset(sceneSetupData, sceneDataLocation);
            var importer = AssetImporter.GetAtPath(sceneDataLocation);
            importer.assetBundleName = $"{SceneManager.GetActiveScene().name}";
        }

        [MenuItem("Tools/Load Scene Setup")]
        static void LoadSceneSetup()
        {
            var activeScene = EditorSceneManager.GetActiveScene();
            string sceneDataLocation = GetSceneSetupLocation(activeScene);
            var data = AssetDatabase.LoadAssetAtPath<SceneSetupData>(sceneDataLocation);
            var sceneSetup = data.IncludedScenes.Select(
                s => new SceneSetup()
                {
                    path = s,
                    isActive = SceneManager.GetActiveScene().path == s,
                    isLoaded = true,

                }).ToArray();
            EditorSceneManager.RestoreSceneManagerSetup(sceneSetup);
            EditorSceneManager.SaveOpenScenes();
        }

        static string GetSceneSetupLocation(Scene activeScene)
        {
            return "Assets/Scenes/" + activeScene.name + "/" + "SceneSetupData.asset";
        }
    }
}
