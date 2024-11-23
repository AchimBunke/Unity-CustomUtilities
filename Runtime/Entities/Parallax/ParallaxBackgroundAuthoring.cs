#if USE_ENTITIES_1_3_5
using Unity.Entities;
using UnityEngine;

namespace UnityUtilities.Entities.Parallax
{
    public class ParallaxBackgroundAuthoring : MonoBehaviour
    {
        public float ParallaxFactor = 0f;
        public class ParallaxBackgroundBaker : Baker<ParallaxBackgroundAuthoring>
        {
            public override void Bake(ParallaxBackgroundAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ParallaxBackground
                {
                    ParallaxFactor = authoring.ParallaxFactor,
                    StartPosition = authoring.gameObject.transform.position,
                });
            }
        }
    }
}
#endif