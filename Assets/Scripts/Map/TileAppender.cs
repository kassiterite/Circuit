using System.Linq;

public static class TileAppender
{
    public static bool IsAppendPassed(VoxelTile[,] placedTiles, VoxelTile tile, int x, int y) =>
        (IsOutOfLowerRange(x) || IsAppendLeftToNeighborRight(placedTiles[x - 1, y], tile)) &&
        (IsOutOfUpperRange(x, 0, placedTiles) || IsAppendRightToNeighborLeft(placedTiles[x + 1, y], tile)) &&
        (IsOutOfLowerRange(y) || IsAppendBackToNeighborFront(placedTiles[x, y - 1], tile)) &&
        (IsOutOfUpperRange(y, 1, placedTiles) || IsAppendFrontToNeighborBack(placedTiles[x, y + 1], tile));

    private static bool IsAppendRightToNeighborLeft(VoxelTile neighborTile, VoxelTile tileToAppend) => 
        neighborTile == null || neighborTile.colors.rightSide.SequenceEqual(tileToAppend.colors.leftSide);
    
    private static bool IsAppendLeftToNeighborRight(VoxelTile neighborTile, VoxelTile tileToAppend) => 
        neighborTile == null || neighborTile.colors.leftSide.SequenceEqual(tileToAppend.colors.rightSide);
    
    private static bool IsAppendBackToNeighborFront(VoxelTile neighborTile, VoxelTile tileToAppend) => 
        neighborTile == null || neighborTile.colors.backSide.SequenceEqual(tileToAppend.colors.frontSide);
    
    private static bool IsAppendFrontToNeighborBack(VoxelTile neighborTile, VoxelTile tileToAppend) => 
        neighborTile == null || neighborTile.colors.frontSide.SequenceEqual(tileToAppend.colors.backSide);
    
    public static bool IsOutOfUpperRange(int i, int dimension, VoxelTile[,] placedTiles) => i >= placedTiles.GetLength(dimension) - 1;

    public static bool IsOutOfLowerRange(int i) => i <= 0;
}
