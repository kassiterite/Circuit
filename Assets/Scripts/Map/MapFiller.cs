
using UnityEngine;

public class MapFiller
{
    private VoxelTile[,] _placedTiles;
    private TileSelector _tileSelector;
    private TilePlacer _tilePlacer;
    public MapFiller(VoxelTile[,] placedTiles, PrefabGenerator prefabHolder, Transform transform)
    {
        _placedTiles = placedTiles;
        _tileSelector = new TileSelector(prefabHolder);
        _tilePlacer = new TilePlacer(transform);
    }

    public VoxelTile[,] FillMap()
    {
        for (int y = 0; y < _placedTiles.GetLength(1); y++)
        {
            for (int x = 0; x < _placedTiles.GetLength(0); x++)
            {
                if (_placedTiles[x, y] == null)
                {
                    _placedTiles[x, y] = _tileSelector.SelectTile(_placedTiles, x, y);
                    _tilePlacer.PlaceTile(_placedTiles[x, y], x, y);
                    (_placedTiles[x, y].x, _placedTiles[x, y].y) = (x, y);
                }
            }
        }
        return _placedTiles;
    }
}
