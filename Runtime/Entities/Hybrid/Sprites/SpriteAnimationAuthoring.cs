#if USE_ENTITIES_1_3_5
using Unity.Entities;
using UnityEngine;

namespace UnityUtilities.Entities.Hybrid.Sprites
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class SpriteAnimationAuthoring : MonoBehaviour
    {
        public class SpriteAnimationBaker : Baker<SpriteAnimationAuthoring>
        {
            public override void Bake(SpriteAnimationAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                var animator = authoring.GetComponent<Animator>();


                AddComponent(entity, new SpriteAnimationData
                {
                    CurrentFrame = 0,
                    ElapsedTime = 0,
                });
                AddComponent<NewSpriteAnimationTag>(entity);

                AddComponentObject(entity, new AnimationClipReference()
                {
                    AnimationClip = animator.runtimeAnimatorController.animationClips[0],
                });

            }
        }


    }
}
#endif