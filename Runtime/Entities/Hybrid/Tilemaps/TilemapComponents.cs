#if USE_ENTITIES_1_3_5 && USE_PHYSICS_1_3_5 && UNITY_PHYSICS_CUSTOM

using static UnityEngine.Tilemaps.Tile;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace UnityUtilities.Entities.Hybrid.Tilemaps
{
    public class TileData : IComponentData
    {
        public Entity TilePrefab;
        public ColliderType ColliderType;
        public float3 Position;
        public Sprite Sprite;
        public BlobAssetReference<TileDataBlob> MeshData;
        public int SortingLayerID;
        public int SortingOrder;
        public Material Material;
    }

    public struct TileDataBlob
    {
        public BlobArray<float3> Vertices;
        public BlobArray<int3> Triangles;
    }

    public struct NewTilePlaceholderTag : IComponentData { }
    public struct NewTileTag : IComponentData { }
}
#endif