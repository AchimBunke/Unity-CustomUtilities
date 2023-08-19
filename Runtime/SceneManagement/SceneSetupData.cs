using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtilities.SceneManagement
{
    [CreateAssetMenu(fileName = "SceneSetupData", menuName = "ScriptableObjects/SceneManagement/SceneSetupData")]
    public class SceneSetupData : ScriptableObject
    {
        public string[] IncludedScenes;
    }
}
