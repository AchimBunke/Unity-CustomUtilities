#if USE_ENTITIES_1_3_5
using Unity.Entities;
using UnityEngine;

namespace UnityUtilities.Entities.Parallax
{
    public class ParallaxTargetAuthoring : MonoBehaviour
    {
        public class ParallaxTargetBaker : Baker<ParallaxTargetAuthoring>
        {
            public override void Bake(ParallaxTargetAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<ParallaxTargetTag>(entity);

            }
        }

    }
}
#endif