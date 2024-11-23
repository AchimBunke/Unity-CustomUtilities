#if USE_ENTITIES_1_3_5
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtilities.Entities.Hybrid.Sprites
{
    /// <summary>
    /// Cache sprites for sprite animations
    /// </summary>
    public class SpriteLibrary
    {
        private static SpriteLibrary _instance;
        public static SpriteLibrary Instance => _instance ?? (_instance = new SpriteLibrary());
        private List<Sprite> allSprites;
        private Dictionary<Sprite, int> spriteIndexMap;

        private SpriteLibrary()
        {
            allSprites = new();
            spriteIndexMap = new();
        }
        public Sprite GetSprite(int index)
        {
            if (index >= allSprites.Count)
                return null;
            return allSprites[index];
        }
        public int AddSprite(Sprite sprite)
        {
            if (spriteIndexMap.TryGetValue(sprite, out int existingIndex))
            {
                return existingIndex; // Sprite already exists, return its ID
            }

            // Sprite is new, add it to both List and Dictionary
            int newIndex = allSprites.Count;
            allSprites.Add(sprite);
            spriteIndexMap[sprite] = newIndex;
            return newIndex;
        }
    }
}
#endif