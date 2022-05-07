using System.Collections;
using System.Linq;
using ClassExtensions;
using UnityEngine;
using Random = UnityEngine.Random;
using Direction = UnityEngine.UI.Slider.Direction;

[RequireComponent(typeof(MapGenerator))]

public class WayGenerator : MonoBehaviour
{ 
    public Vector2Int startTilePos = new Vector2Int(0, 0); 
    public Vector2Int endTilePos = new Vector2Int(9, 9);
    [SerializeField] private int conductUVColor = 251;
    private PrefabGenerator _prefabHolder;
    private MapGenerator _mapGenerator;

    private void Start()
    {
        _mapGenerator = GetComponent<MapGenerator>();
        _prefabHolder = _mapGenerator.prefabHolder;
        if (startTilePos.x > endTilePos.x && startTilePos.y > endTilePos.y)
            (startTilePos, endTilePos) = (endTilePos, startTilePos);
        if (startTilePos == endTilePos)
            Debug.LogError("Positions start and end tiles are equals");
        else
        {
            PlaceUniqueTile(startTilePos.x, startTilePos.y, VoxelTile.TypeTile.StartTile, Direction.LeftToRight);
            PlaceUniqueTile(endTilePos.x, endTilePos.y, VoxelTile.TypeTile.EndTile, Direction.RightToLeft);
            StartCoroutine(GenerateWay());
        }
    }
    private void PlaceUniqueTile(int x, int y, VoxelTile.TypeTile type, Direction direction)
    {
        List.Shuffle(_prefabHolder.prefabs);
        foreach (var tile in from VoxelTile tile in _prefabHolder.prefabs
                 where tile.type == type
                 select tile)
        {
            tile.colors.GenerateColors();
            if ((direction == Direction.LeftToRight && tile.colors.leftSide[12] == conductUVColor) ||
                (direction == Direction.RightToLeft && tile.colors.rightSide[12] == conductUVColor 
                                                    && tile.colors.frontSide[12] == conductUVColor))
            {
                _mapGenerator.PlaceTile(tile, x, y);
                break;
            }
        }
    }
    private IEnumerator GenerateWay()
    {
        int x = startTilePos.x;
        int y = startTilePos.y;
        bool isMoveToRightDir = true;
        while(true)
        {
            if (isMoveToRightDir)
            {
                int offsetX = Random.Range(1, endTilePos.x - x);
                if (offsetX + x > endTilePos.x) offsetX = endTilePos.x - x;
                for (int j = 0; j <= offsetX; j++)
                { 
                    if (j == offsetX && offsetX != endTilePos.x) AppendAndPlaceTile(j + x, y, new Vector2Int(1,-1));
                    else AppendAndPlaceTile(j + x, y, new Vector2Int(1,0));
                }
                x += offsetX;
                isMoveToRightDir = false;
            }
            else
            {
                int offsetY = Random.Range(1, endTilePos.y - y);
                if (offsetY + y > endTilePos.y) offsetY = endTilePos.y - y;
                for (int j = 0; j <= offsetY; j++)
                { 
                    if (j == offsetY && offsetY != endTilePos.y) AppendAndPlaceTile(x, y + j, new Vector2Int(-1,1));
                    else AppendAndPlaceTile(x, y + j, new Vector2Int(0,1));
                }
                y += offsetY;
                isMoveToRightDir = true;
            }
            if (x == endTilePos.x && y == endTilePos.y)
            {
                _mapGenerator.StartGenerating();
                break;
            }
            yield return new WaitForSeconds(_mapGenerator.tileGenerationTime);
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
               tile.colors.leftSide[12] == conductUVColor && tile.colors.frontSide[12] == conductUVColor) ||
               (direction == new Vector2Int(1,-1) && 
                tile.colors.rightSide[12] == conductUVColor && tile.colors.backSide[12] == conductUVColor) ||
               (direction == new Vector2Int(1,0) && 
                tile.colors.rightSide[12] == conductUVColor && tile.colors.leftSide[12] == conductUVColor) ||
               (direction == new Vector2Int(0,1) && 
               tile.colors.backSide[12] == conductUVColor && tile.colors.frontSide[12] == conductUVColor))
            {
                _mapGenerator.PlaceTile(tile, x, y);
                break;
            }
        }
    }
}
