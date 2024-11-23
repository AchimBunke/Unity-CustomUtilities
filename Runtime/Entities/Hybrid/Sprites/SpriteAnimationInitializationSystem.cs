#if USE_ENTITIES_1_3_5
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

namespace UnityUtilities.Entities.Hybrid.Sprites
{
    [WorldSystemFilter(WorldSystemFilterFlags.LocalSimulation | 
        WorldSystemFilterFlags.ClientSimulation | 
        WorldSystemFilterFlags.ThinClientSimulation)]
    public partial struct SpriteAnimationInitializationSystem : ISystem
    {
        void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NewSpriteAnimationTag>();
        }

        void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (animData, clipRef, entity) in SystemAPI.Query<SpriteAnimationData, AnimationClipReference>()
                .WithAll<NewSpriteAnimationTag>()
                .WithEntityAccess())
            {

                var animDataBuffer = ecb.AddBuffer<SpriteAnimationFrame>(entity);
                var (frameCount ,firstFrameDuration) = BakeAnimation(clipRef.AnimationClip, ecb, entity);
                ecb.SetComponent(entity, new SpriteAnimationData
                {
                    CurrentFrame = 0,
                    ElapsedTime = 0,
                    CurrentFrameDuration = firstFrameDuration,
                    TotalFrames = frameCount,
                });
                ecb.RemoveComponent<NewSpriteAnimationTag>(entity);
                ecb.RemoveComponent<AnimationClipReference>(entity);
            }
            ecb.Playback(state.EntityManager);
        }

        private (int frameCount, float firstFrameDuration) BakeAnimation(AnimationClip clip, EntityCommandBuffer ecb, Entity entity)
        {
            float clipLength = clip.length;

            var bindings = AnimationUtility.GetObjectReferenceCurveBindings(clip);

            List<Sprite> sprites = new List<Sprite>();
            List<float> spriteDurations = new List<float>();

            List<float> spriteTimes = new List<float>();
            foreach (var binding in bindings)
            {
                // Find the binding that targets the SpriteRenderer's sprite
                if (binding.propertyName == "m_Sprite")
                {
                    var keyframes = AnimationUtility.GetObjectReferenceCurve(clip, binding);

                    // Loop through each keyframe
                    foreach (var keyframe in keyframes)
                    {
                        Sprite sprite = keyframe.value as Sprite;
                        if (sprite == null) continue;
                        var time = keyframe.time;

                        if (sprites.Count == 0 || sprites[sprites.Count - 1] != sprite)
                        {
                            sprites.Add(sprite);
                            spriteTimes.Add(time); // Store time at which the sprite appears
                        }
                    }
                }
            }
            float firstFrameDuration = sprites.Count > 1 ? spriteTimes[1] - spriteTimes[0] : float.MaxValue;
            for (int i = 0; i < sprites.Count; ++i)
            {
                var spriteId = SpriteLibrary.Instance.AddSprite(sprites[i]);

                ecb.AppendToBuffer(entity, new SpriteAnimationFrame
                {
                    FrameDuration = ((i + 1) < sprites.Count ? spriteTimes[i + 1] : clipLength) - spriteTimes[i],
                    FrameId = spriteId,
                });
            }
            return (sprites.Count, firstFrameDuration);
        }
    }
}
#endif