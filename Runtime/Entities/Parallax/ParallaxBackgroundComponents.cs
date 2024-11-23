#if USE_ENTITIES_1_3_5
using Unity.Entities;
using Unity.Mathematics;

namespace UnityUtilities.Entities.Parallax
{
    public struct ParallaxBackground : IComponentData
    {
        public float ParallaxFactor;
        public float3 StartPosition;
    }
    public struct ParallaxTargetTag : IComponentData { }
}
#endif