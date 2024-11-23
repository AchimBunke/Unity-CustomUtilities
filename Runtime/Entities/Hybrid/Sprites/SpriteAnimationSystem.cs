#if USE_ENTITIES_1_3_5
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace UnityUtilities.Entities.Hybrid.Sprites
{
    [WorldSystemFilter(WorldSystemFilterFlags.LocalSimulation |
      WorldSystemFilterFlags.ClientSimulation |
      WorldSystemFilterFlags.ThinClientSimulation)]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct SpriteAnimationDataUpdateSystem : ISystem
    {
        EntityQuery query;
        void OnCreate(ref SystemState state)
        {
            query = state.EntityManager.CreateEntityQuery(
                ComponentType.ReadWrite<SpriteAnimationData>(),
                ComponentType.Exclude<NewSpriteAnimationTag>());
        }

        void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            var job = new AnimationFrameDataUpdateJob
            {
                DeltaTime = deltaTime,
                AnimationFrameBufferLookup = SystemAPI.GetBufferLookup<SpriteAnimationFrame>(true),
            };
            state.Dependency = job.Schedule(query, state.Dependency);
        }

        [BurstCompile]
        public partial struct AnimationFrameDataUpdateJob : IJobEntity
        {
            [ReadOnly] public BufferLookup<SpriteAnimationFrame> AnimationFrameBufferLookup;
            [ReadOnly] public float DeltaTime;
            public void Execute(Entity entity, ref SpriteAnimationData animData)
            {
                animData.ElapsedTime += DeltaTime;
                if (AnimationFrameBufferLookup.TryGetBuffer(entity, out var animationFrameBuffer))
                {
                    if(animData.ElapsedTime >= animData.CurrentFrameDuration)
                    {
                        animData.ElapsedTime -= animData.CurrentFrameDuration;

                        // switch frames
                        animData.CurrentFrame = (animData.CurrentFrame + 1) % animData.TotalFrames;
                        // set frame duration
                        animData.CurrentFrameDuration = animationFrameBuffer[animData.CurrentFrame].FrameDuration;
                        animData.RequireSpriteChange = true;
                        
                    }
                }
            }
        }
    }
    [WorldSystemFilter(WorldSystemFilterFlags.LocalSimulation |
       WorldSystemFilterFlags.ClientSimulation |
       WorldSystemFilterFlags.ThinClientSimulation)]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(SpriteAnimationDataUpdateSystem))]
    public partial class SpriteAnimationRendererUpdateSystem : SystemBase
    {
        protected override void OnCreate()
        {
        }
        protected override void OnUpdate()
        {
            Entities.WithoutBurst()
                .WithAll<SpriteRenderer, SpriteAnimationData>()
                .WithNone<NewSpriteAnimationTag>()
                .ForEach((SpriteRenderer spriteRenderer, ref SpriteAnimationData animData, in DynamicBuffer<SpriteAnimationFrame> animFrameBuffer) =>
                {
                    if (animData.RequireSpriteChange)
                    {
                        var sprite = GetSpriteForIndex(animFrameBuffer[animData.CurrentFrame].FrameId);
                        if (sprite != null)
                            spriteRenderer.sprite = sprite;
                        animData.RequireSpriteChange = false;
                    }
                }).Run();
        }

        Sprite GetSpriteForIndex(int index)
        {
            return SpriteLibrary.Instance.GetSprite(index);
        }
    }
}
#endif