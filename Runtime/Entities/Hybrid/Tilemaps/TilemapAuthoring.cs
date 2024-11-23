#if USE_ENTITIES_1_3_5 && USE_PHYSICS_1_3_5 && UNITY_PHYSICS_CUSTOM

using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityUtilities.Entities.Hybrid.Tilemaps
{
    [RequireComponent(typeof(Tilemap))]
    [RequireComponent(typeof(TilemapRenderer))]
    public class TilemapAuthoring : MonoBehaviour
    {
        public bool GenerateColliders = true;
        public ECSTile TilePrefab;

        class TilemapBaker : Baker<TilemapAuthoring>
        {
            public override void Bake(TilemapAuthoring authoring)
            {
                //var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
                var tilemap = authoring.GetComponent<Tilemap>();
                var tileMapRenderer = authoring.GetComponent<TilemapRenderer>();
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                if (tilemap == null || tileMapRenderer == null)
                    return;
                var bounds = tilemap.cellBounds;

                for (int x = bounds.xMin; x < bounds.xMax; x++)
                {
                    for (int y = bounds.yMin; y < bounds.yMax; y++)
                    {
                        // Get the tile at the current position
                        var cellPosition = new Vector3Int(x, y, 0);
                        TileBase tile = tilemap.GetTile(cellPosition);
                        if (tile != null)
                        {

                            // Calculate the position for the entity
                            var worldPos = tilemap.CellToWorld(cellPosition);
                            float3 position = worldPos + tilemap.cellSize / 2f; // Center the tile
                            var colliderType = authoring.GenerateColliders ? tilemap.GetColliderType(new Vector3Int(x, y, 0)) : Tile.ColliderType.None;
                            var sprite = tilemap.GetSprite(new Vector3Int(x, y, 0));
                            //tileCopy.AddComponent<PhysicsShapeAuthoring>();

                            switch (colliderType)
                            {
                                case Tile.ColliderType.None:
                                    break;
                                case Tile.ColliderType.Grid:
                                    break;
                                case Tile.ColliderType.Sprite:
                                    break;
                            }

                            // Create a new entity for each tile
                            var tileEntity = CreateAdditionalEntity(TransformUsageFlags.Dynamic);
                            AddComponent<NewTilePlaceholderTag>(tileEntity);
                            AddComponentObject(tileEntity, new TileData()
                            {
                                ColliderType = colliderType,
                                Sprite = sprite,
                                Position = position,
                                TilePrefab = GetEntity(authoring.TilePrefab, TransformUsageFlags.Dynamic),
                                SortingLayerID = tileMapRenderer.sortingLayerID,
                                SortingOrder = tileMapRenderer.sortingOrder,
                            });
                        }
                    }
                }
            }


        }

    }
}
#endif