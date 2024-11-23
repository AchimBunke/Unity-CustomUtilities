#if USE_ENTITIES_1_3_5 && USE_PHYSICS_1_3_5 && UNITY_PHYSICS_CUSTOM


using Unity.Physics.Authoring;
using UnityEngine;

namespace UnityUtilities.Entities.Hybrid.Tilemaps
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(PhysicsShapeAuthoring))]
    public class ECSTile : MonoBehaviour
    {
    }
}
#endif