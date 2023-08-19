using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityUtilities.SceneManagement;

namespace UnityUtilities.AssetManagement
{
    /// <summary>Custom Editor for our PrefabSwitch script, to allow us to perform actions
    /// from the editor.</summary>
    [CustomEditor(typeof(ItemSwitch))]
    public class PrefabSwitchEditor : Editor
    {
        /// <summary>Calls on drawing the GUI for the inspector.</summary>
        public override void OnInspectorGUI()
        {
            // Draw the default inspector.
            DrawDefaultInspector();

            // Grab a reference to the target script, so we can identify it as a 
            // PrefabSwitch, instead of a simple Object.
            ItemSwitch prefabSwitch = (ItemSwitch)target;

            // Create a Button for "Swap By Tag",
            //if (GUILayout.Button("Swap By Tag"))
            //{
            //    // if it is clicked, call the SwapAllByTag method from prefabSwitch.
            //    prefabSwitch.SwapAll();
            //}

            // Create a Button for "Swap By Array", 
            if (GUILayout.Button("Swap By Array"))
            {
                // if it is clicked, call the SwapAllByArray method from prefabSwitch.
                prefabSwitch.SwapAll();
            }
        }
    }
}
