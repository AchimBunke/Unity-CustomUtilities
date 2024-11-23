#if USE_ENTITIES_1_3_5
using Unity.Entities;
using UnityEngine;

namespace UnityUtilities.Entities.Hybrid.Sprites
{
    public struct NewSpriteAnimationTag : IComponentData { }
    public struct SpriteAnimationData : IComponentData
    {
        public float ElapsedTime;
        public float CurrentFrameDuration;
        public int CurrentFrame;
        public int TotalFrames;

        public bool RequireSpriteChange;
    }
    public struct SpriteAnimationFrame : IBufferElementData
    {
        public int FrameId;
        public float FrameDuration;
    }

    public class AnimationClipReference : IComponentData
    {
        public AnimationClip AnimationClip;
    }
}
#endif