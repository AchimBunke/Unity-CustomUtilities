#if USE_ENTITIES_1_3_5
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace UnityUtilities.Entities.Parallax
{

    [WorldSystemFilter(WorldSystemFilterFlags.LocalSimulation | WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    public partial struct ParallaxBackgroundSystem : ISystem
    {
        void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ParallaxBackground>();
            state.RequireForUpdate<ParallaxTargetTag>();
        }

        void OnUpdate(ref SystemState state)
        {
            var parallaxTargetEntity = SystemAPI.GetSingletonEntity<ParallaxTargetTag>();
            if (!SystemAPI.HasComponent<LocalTransform>(parallaxTargetEntity))
                return;
            var parallaxTargetTransform = SystemAPI.GetComponentRO<LocalTransform>(parallaxTargetEntity);
            var targetPosition = parallaxTargetTransform.ValueRO.Position;

            foreach (var (parallaxBackground, transform) in SystemAPI.Query<RefRO<ParallaxBackground>, RefRW<LocalTransform>>()
               .WithAll<Simulate>())
            {
                float dist = targetPosition.x * -parallaxBackground.ValueRO.ParallaxFactor;
                float3 newPosition = parallaxBackground.ValueRO.StartPosition;
                newPosition.x += dist;
                transform.ValueRW.Position = newPosition;
            }
        }
    }
}
#endif