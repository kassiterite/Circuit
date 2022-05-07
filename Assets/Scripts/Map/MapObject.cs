using System;
using System.Linq;
using UnityEngine;

public class MapObject : MonoBehaviour
{
    [SerializeField] private MapProperties mapProperties;
    [SerializeField] private WayProperties wayProperties;
    [SerializeField] private TileProperties tileProperties;

    private VoxelTile[,] _placedTiles;
    public VoxelTile[,] PlacedTiles => _placedTiles;

    [Serializable]
    public struct MapProperties
    {
        [SerializeField] private PrefabGenerator prefabHolder;
        [SerializeField] private Vector2Int mapSize;

        public readonly PrefabGenerator PrefabHolder => prefabHolder;
        public readonly Vector2Int MapSize => mapSize;
    }

    [Serializable]
    public struct TileProperties
    {
        [SerializeField] private int conductUVColor;
        [SerializeField] private Vector2Int conductUVPosition;

        public readonly int ConductUVColor => conductUVColor;
        public readonly Vector2Int ConductUVPosition => conductUVPosition;
    }
    
    [Serializable]
    public struct WayProperties
    {
        [SerializeField] private UniqueTile[] uniqueTiles;

        public readonly UniqueTile[] UniqueTiles => uniqueTiles;
    }

    [Serializable]
    public struct UniqueTile
    {
        [SerializeField] private VoxelTile.TypeTile type;
        [SerializeField] private Vector2Int position;
        public readonly VoxelTile.TypeTile Type => type;
        public readonly Vector2Int Position => position;
    }

    void Start()
    {
        _placedTiles = new VoxelTile[mapProperties.MapSize.x, mapProperties.MapSize.y];
        MapFiller mapFiller = new MapFiller(_placedTiles, mapProperties.PrefabHolder, transform);
        _placedTiles = mapFiller.FillMap();
    }

    private void OnValidate()
    {
        if (wayProperties.UniqueTiles.All(x => x.Type != VoxelTile.TypeTile.StartTile))
            Debug.LogError("Unique tiles do not contain start tile");
        if (wayProperties.UniqueTiles.All(x => x.Type != VoxelTile.TypeTile.EndTile))
            Debug.LogError("Unique tiles do not contain end tile");
        if (mapProperties.MapSize.x <= 0 || mapProperties.MapSize.y <= 0)
            Debug.LogError("X or Y of map size is below or equal to zero.");
    }
}
