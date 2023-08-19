using UnityEngine;

namespace UnityUtilities.SceneManagement
{
    public class ItemSwitch : MonoBehaviour
    {
        /// <summary>The new object to instantiate in place of the old object.</summary>
        public GameObject newItem;
        /// <summary>The old objects, intended to be swapped out for iterations of 
        /// the new object.</summary>
        public GameObject[] oldGameObjects;

        public void SwapAll()
        {
            // Store a boolean to detect if we intend to swap this game object.
            bool swappingSelf = false;

            // For each game object in the oldGameObjects array, 
            for (int i = 0; i < oldGameObjects.Length; i++)
            {
                // If the current game object is this game object, 
                if (oldGameObjects[i] == gameObject)
                {
                    // Enable the flag to swap this game object at the end, so we
                    // do not destroy it before the script an complete its task.
                    swappingSelf = true;
                }
                else
                {
                    // Else, we are not dealing with the game object local to this 
                    // script, so we can swap the prefabs, immediately. 
                    SwapPrefabs(oldGameObjects[i]);
                }
            }

            // If we have flagged the local game object to be swapped, 
            if (swappingSelf)
            {
                // Swap the local game object.
                SwapPrefabs(gameObject);
            }
        }
        /// <summary>Swaps the desired oldGameObject for a newPrefab.</summary>
        /// <param name="oldGameObject">The old game object.</param>
        void SwapPrefabs(GameObject oldGameObject)
        {
            // Determine the rotation and position values of the old game object.
            // Replace rotation with Quaternion.identity if you do not wish to keep rotation.
            Quaternion rotation = oldGameObject.transform.rotation;
            Vector3 position = oldGameObject.transform.position;

            return;/*
        // Instantiate the new game object at the old game objects position and rotation.
        GameObject newGameObject = PrefabUtility.InstantiatePrefab(newItem) as GameObject;
        newGameObject.transform.position = position;
        newGameObject.transform.rotation = rotation;

        // If the old game object has a valid parent transform,
        // (You can remove this entire if statement if you do not wish to ensure your
        // new game object does not keep the parent of the old game object.
        if (oldGameObject.transform.parent != null)
        {
            // Set the new game object parent as the old game objects parent.
            newGameObject.transform.SetParent(oldGameObject.transform.parent);
        }

        // Destroy the old game object, immediately, so it takes effect in the editor.
        DestroyImmediate(oldGameObject);*/
        }

    }
}