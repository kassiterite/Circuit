namespace Map
{
    public class ConductAppender
    {
        private VoxelTile[,] _placedTiles;
        private MapObject.TileProperties _tileProperties;
        private int _tileSize = 5;
        
        public ConductAppender(VoxelTile[,] placedTiles, MapObject.TileProperties tileProperties)
        {
            _placedTiles = placedTiles;
            _tileProperties = tileProperties;
        }
        
        private bool IsConductLeftToNeighborRight(int x, int y) =>
            IsCanConduct(_placedTiles[x - 1, y].colors.leftSide) && IsCanConduct(_placedTiles[x, y].colors.rightSide);
    
        private bool IsConductRightToNeighborLeft(int x, int y) =>
            IsCanConduct(_placedTiles[x + 1, y].colors.rightSide) && IsCanConduct(_placedTiles[x, y].colors.leftSide);
    
        private bool IsConductBackToNeighborFront(int x, int y) =>
            IsCanConduct(_placedTiles[x, y - 1].colors.backSide) && IsCanConduct(_placedTiles[x, y].colors.frontSide);
    
        private bool IsConductFrontToNeighborBack(int x, int y) =>
            IsCanConduct(_placedTiles[x, y + 1].colors.frontSide) && IsCanConduct(_placedTiles[x, y].colors.backSide);
    
        private bool IsCanConduct(byte[] side) => side[_tileProperties.ConductUVPosition.x + _tileProperties.ConductUVPosition.y * _tileSize] == _tileProperties.ConductUVColor;

        private bool IsGateOpen(VoxelTile tile) =>
            tile.GetComponentInChildren<ActivateTorch>().isEnable;

        private bool IsGate(VoxelTile tile) =>
            tile.type == VoxelTile.TypeTile.GateTile;
    }
}