using ClassExtensions;

public class TileSelector
{
    private PrefabGenerator _prefabHolder;
    public TileSelector(PrefabGenerator prefabHolder)
    {
        _prefabHolder = prefabHolder;
    }
    public VoxelTile SelectTile(VoxelTile[,] placedTiles, int x, int y)
    {
        List.Shuffle(_prefabHolder.prefabs);
        foreach (VoxelTile tile in _prefabHolder.prefabs)
        {
            tile.colors.GenerateColors();
            if (TileAppender.IsAppendPassed(placedTiles, tile, x, y))
            {
                return tile;
            }
        }

        return null;
    }
}
