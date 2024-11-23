#if USE_ENTITIES_1_3_5
#if USE_PHYSICS_1_3_5

using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityUtilities.Entities.Hybrid.Tilemaps
{
    public partial struct TilePlaceholderInitializationSystem : ISystem
    {
        void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NewTilePlaceholderTag>();
        }
        void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (tileData, parent, entity) in
                SystemAPI.Query<TileData, Parent>()
                .WithAny<NewTilePlaceholderTag>()
                .WithEntityAccess())
            {

                var tileEntity = ecb.Instantiate(tileData.TilePrefab);
                ecb.AddComponent(tileEntity, new Parent { Value = parent.Value });
                ecb.AddComponent<NewTileTag>(tileEntity);
                ecb.AddComponent(tileEntity, tileData);
                ecb.SetName(tileEntity, "Tile");
                ecb.DestroyEntity(entity);
            }
            ecb.Playback(state.EntityManager);
        }
    }
    public partial struct TileInitializationSystem : ISystem
    {
        private NativeHashMap<int, BlobAssetReference<Unity.Physics.Collider>> colliderCache;

        void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NewTileTag>();
            colliderCache = new NativeHashMap<int, BlobAssetReference<Unity.Physics.Collider>>(256, Allocator.Persistent);
        }
        public void OnDestroy(ref SystemState state)
        {
            // Dispose of all MeshColliders when the system is destroyed
            foreach (var collider in colliderCache.GetValueArray(Allocator.Temp))
            {
                collider.Dispose();
            }
            colliderCache.Dispose();
        }
        void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (transform, tileData, physicsCollider, entity) in
                SystemAPI.Query<RefRW<LocalTransform>, TileData, RefRW<PhysicsCollider>>()
                .WithAny<NewTileTag>()
                .WithEntityAccess())
            {
                transform.ValueRW.Position = tileData.Position;

                var renderer = state.EntityManager.GetComponentObject<SpriteRenderer>(entity);
                renderer.sprite = tileData.Sprite;
                renderer.sortingLayerID = tileData.SortingLayerID;
                renderer.sortingOrder = tileData.SortingOrder;

                switch (tileData.ColliderType)
                {
                    case Tile.ColliderType.None:
                        ecb.RemoveComponent<PhysicsCollider>(entity);
                        break;
                    case Tile.ColliderType.Sprite:
                        {

                            var innerCollider = physicsCollider.ValueRW.Value.Value;
                            var sprite = tileData.Sprite;
                            int spriteHash = sprite.GetHashCode();
                            if (!colliderCache.TryGetValue(spriteHash, out var colliderBlob))
                            {
                                // Create colliderBlob
                                List<Vector2> physicsShape = new List<Vector2>();
                                int shapeCount = sprite.GetPhysicsShapeCount();
                                for (int i = 0; i < shapeCount; i++)
                                {
                                    sprite.GetPhysicsShape(i, physicsShape);
                                }

                                NativeArray<float3> vertices = new NativeArray<float3>(physicsShape.Count, Allocator.Temp);
                                for (int i = 0; i < physicsShape.Count; i++)
                                {
                                    // Convert Vector2 to float3 (setting z to 0)
                                    vertices[i] = new float3(physicsShape[i], 0);
                                }
                                int triangleCount = physicsShape.Count - 2;
                                NativeArray<int3> triangles = new NativeArray<int3>(triangleCount, Allocator.Temp);
                                for (int i = 0; i < triangleCount; i++)
                                {
                                    // Create a triangle fan around the first vertex
                                    triangles[i] = new int3(0, i + 1, i + 2);
                                }
                                colliderBlob = Unity.Physics.MeshCollider.Create(vertices, triangles);
                                colliderCache.Add(spriteHash, colliderBlob);
                                vertices.Dispose();
                                triangles.Dispose();
                            }
                            physicsCollider.ValueRW.Value = colliderBlob;
                        }
                        break;
                    case Tile.ColliderType.Grid:
                        break;
                }
                ecb.RemoveComponent<NewTileTag>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
#endif
#endif