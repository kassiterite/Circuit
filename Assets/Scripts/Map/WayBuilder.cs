using System.Linq;
using ClassExtensions;
using UnityEngine;
using Random = UnityEngine.Random;
using Direction = UnityEngine.UI.Slider.Direction;

public class WayBuilder
{ 
    private Vector2Int _startTilePosition;
    private Vector2Int _endTilePosition;
    private PrefabGenerator _prefabHolder;
    private TilePlacer _tilePlacer;
    private MapObject.TileProperties _tileProperties;
    private int _conductVoxelPosition;
    private VoxelTile[,] _placedTiles;

    public WayBuilder(MapObject.WayProperties wayProperties, 
        MapObject.TileProperties tileProperties,
        PrefabGenerator prefabHolder, TilePlacer tilePlacer, ref VoxelTile[,] placedTiles)
    {
        _prefabHolder = prefabHolder;
        _tileProperties = tileProperties;
        _tilePlacer = tilePlacer;
        _placedTiles = placedTiles;
        _conductVoxelPosition = _tileProperties.ConductUVPosition.x + _tileProperties.ConductUVPosition.y * 5;
        _startTilePosition = wayProperties.UniqueTiles.First(x => x.Type == VoxelTile.TypeTile.StartTile).Position;
        _endTilePosition = wayProperties.UniqueTiles.First(x => x.Type == VoxelTile.TypeTile.EndTile).Position;
        if (_startTilePosition.x > _endTilePosition.x && _startTilePosition.y > _endTilePosition.y)
            (_startTilePosition, _endTilePosition) = (_endTilePosition, _startTilePosition);
        if (_startTilePosition == _endTilePosition)
            Debug.LogError("Positions start and end tiles are equals");
    }
    
    public void PlaceUniqueTile(int x, int y, VoxelTile.TypeTile type, Direction direction)
    {
        List.Shuffle(_prefabHolder.prefabs);
        foreach (var tile in from VoxelTile tile in _prefabHolder.prefabs
                 where tile.type == type
                 select tile)
        {
            tile.colors.GenerateColors();
            if ((direction == Direction.LeftToRight && tile.colors.leftSide[_conductVoxelPosition] == _tileProperties.ConductUVColor) ||
                (direction == Direction.RightToLeft && tile.colors.rightSide[_conductVoxelPosition] == _tileProperties.ConductUVColor 
                                                    && tile.colors.frontSide[_conductVoxelPosition] == _tileProperties.ConductUVColor))
            {
                // TODO: fix problem null exception
                _placedTiles[x, y] = tile;
                _tilePlacer.PlaceTile(tile, x, y);
                (_placedTiles[x, y].x, _placedTiles[x, y].y) = (x, y);
                break;
            }
        }
    }

    public void BuildWay()
    {
        PlaceUniqueTile(_startTilePosition.x, _startTilePosition.y, VoxelTile.TypeTile.StartTile, Direction.LeftToRight); 
        PlaceUniqueTile(_endTilePosition.x, _endTilePosition.y, VoxelTile.TypeTile.EndTile, Direction.RightToLeft);
        int x = _startTilePosition.x;
        int y = _startTilePosition.y;
        bool isMoveToRightDir = true;
        while(true)
        {
            if (isMoveToRightDir)
            {
                int offsetX = Random.Range(1, _endTilePosition.x - x);
                if (offsetX + x > _endTilePosition.x) offsetX = _endTilePosition.x - x;
                for (int j = 0; j <= offsetX; j++)
                { 
                    if (j == offsetX && offsetX != _endTilePosition.x) AppendAndPlaceTile(j + x, y, new Vector2Int(1,-1));
                    else AppendAndPlaceTile(j + x, y, new Vector2Int(1,0));
                }
                x += offsetX;
                isMoveToRightDir = false;
            }
            else
            {
                int offsetY = Random.Range(1, _endTilePosition.y - y);
                if (offsetY + y > _endTilePosition.y) offsetY = _endTilePosition.y - y;
                for (int j = 0; j <= offsetY; j++)
                { 
                    if (j == offsetY && offsetY != _endTilePosition.y) AppendAndPlaceTile(x, y + j, new Vector2Int(-1,1));
                    else AppendAndPlaceTile(x, y + j, new Vector2Int(0,1));
                }
                y += offsetY;
                isMoveToRightDir = true;
            }
            if (x == _endTilePosition.x && y == _endTilePosition.y)
            {
                break;
            }
        }
    }
    
    private void AppendAndPlaceTile(int x, int y, Vector2Int direction)
    {
        List.Shuffle(_prefabHolder.prefabs);
        foreach (var tile in _prefabHolder.prefabs.
                     Where(tile => tile.type == VoxelTile.TypeTile.ConductTile || tile.type == VoxelTile.TypeTile.GateTile))
        {
            tile.colors.GenerateColors();
            if((direction == new Vector2Int(-1,1) && 
               tile.colors.leftSide[_conductVoxelPosition] == _tileProperties.ConductUVColor && tile.colors.frontSide[_conductVoxelPosition] == _tileProperties.ConductUVColor) ||
               (direction == new Vector2Int(1,-1) && 
                tile.colors.rightSide[_conductVoxelPosition] == _tileProperties.ConductUVColor && tile.colors.backSide[_conductVoxelPosition] == _tileProperties.ConductUVColor) ||
               (direction == new Vector2Int(1,0) && 
                tile.colors.rightSide[_conductVoxelPosition] == _tileProperties.ConductUVColor && tile.colors.leftSide[_conductVoxelPosition] == _tileProperties.ConductUVColor) ||
               (direction == new Vector2Int(0,1) && 
               tile.colors.backSide[_conductVoxelPosition] == _tileProperties.ConductUVColor && tile.colors.frontSide[_conductVoxelPosition] == _tileProperties.ConductUVColor))
            {
                if (_placedTiles[x, y] == null)
                {
                    _placedTiles[x, y] = tile;
                    _tilePlacer.PlaceTile(tile, x, y);
                    (_placedTiles[x, y].x, _placedTiles[x, y].y) = (x, y);
                }
                break;
            }
        }
    }
}
