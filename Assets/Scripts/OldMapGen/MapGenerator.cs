using System;
using System.Collections;
using System.Linq;
using ClassExtensions;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] Vector2Int mapSize;
    [Range(0.00005f, 1f)] public float tileGenerationTime;
    public PrefabGenerator prefabHolder;
    public VoxelTile[,] placedTiles;
    
    void Awake()
    {
        if (mapSize.x < 0) mapSize.x = -mapSize.x;
        if (mapSize.y < 0) mapSize.y = -mapSize.y;
        placedTiles = new VoxelTile[mapSize.x, mapSize.y];
    }

    public void StartGenerating()
    {
        StartCoroutine(GenerateMap());
    }

    private IEnumerator GenerateMap()
    {
        for (int y = 0; y < mapSize.x; y++) 
        {
            for (int x = 0; x < mapSize.y; x++)
            {

                List.Shuffle(prefabHolder.prefabs);
                foreach (VoxelTile tile in prefabHolder.prefabs)
                {
                    tile.colors.GenerateColors();
                    if (IsAppendPassed(tile, x, y))
                    {
                        PlaceTile(tile, x, y);
                        yield return new WaitForSeconds(tileGenerationTime);
                        break;
                    }
                }
            }
        }
        GetComponent<MapLogic>().SwitchAtlas();
    }
    
    public void PlaceTile(VoxelTile tile, int x, int y)
    {
        if (x < 0 || y < 0 || x >= mapSize.x || y >= mapSize.y)
        {
            throw new ArgumentOutOfRangeException();
        }
        else if(placedTiles[x, y] == null)
        {
            const float half = 0.5f;

            var currentPos = transform.position;
            Vector3 position = new Vector3(currentPos.x + half * x, currentPos.y, currentPos.z + half * y);
            VoxelTile clone = Instantiate(tile, position, tile.gameObject.transform.rotation);
            clone.transform.parent = transform;
            clone.name = $"tile [{x},{y}]";
            placedTiles[x, y] = clone;
            (placedTiles[x, y].x, placedTiles[x, y].y) = (x, y);
        }
    }

    private bool IsAppendPassed(VoxelTile tile, int x, int y) =>
        (IsOutOfLowerRange(x) || IsAppendLeftToNeighborRight(placedTiles[x - 1, y], tile)) &&
         (IsOutOfUpperRange(x, 0) || IsAppendRightToNeighborLeft(placedTiles[x + 1, y], tile)) &&
         (IsOutOfLowerRange(y) || IsAppendBackToNeighborFront(placedTiles[x, y - 1], tile)) &&
         (IsOutOfUpperRange(y, 1) || IsAppendFrontToNeighborBack(placedTiles[x, y + 1], tile));

    private bool IsAppendRightToNeighborLeft(VoxelTile neighborTile, VoxelTile tileToAppend) => 
        neighborTile == null || neighborTile.colors.rightSide.SequenceEqual(tileToAppend.colors.leftSide);
    
    private bool IsAppendLeftToNeighborRight(VoxelTile neighborTile, VoxelTile tileToAppend) => 
        neighborTile == null || neighborTile.colors.leftSide.SequenceEqual(tileToAppend.colors.rightSide);
    
    private bool IsAppendBackToNeighborFront(VoxelTile neighborTile, VoxelTile tileToAppend) => 
        neighborTile == null || neighborTile.colors.backSide.SequenceEqual(tileToAppend.colors.frontSide);
    
    private bool IsAppendFrontToNeighborBack(VoxelTile neighborTile, VoxelTile tileToAppend) => 
        neighborTile == null || neighborTile.colors.frontSide.SequenceEqual(tileToAppend.colors.backSide);
    
    public bool IsOutOfUpperRange(int i, int dimension) => i >= placedTiles.GetLength(dimension) - 1;

    public static bool IsOutOfLowerRange(int i) => i <= 0;
}
