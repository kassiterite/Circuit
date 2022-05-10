using System.Collections.Generic;
using System.Linq;
using UnityEngine;

    public class AtlasSwitcher
    {
        private readonly VoxelTile[,] _placedTiles;
        private Vector2Int _startTilePosition;
        private Vector2Int _endTilePosition;
        private readonly ConductAppender _conductAppender;

        public AtlasSwitcher(MapObject.TileProperties tileProperties, MapObject.WayProperties wayProperties, ref VoxelTile[,] placedTiles)
        {
            _placedTiles = placedTiles;
            _startTilePosition = wayProperties.UniqueTiles.First(x => x.Type == VoxelTile.TypeTile.StartTile).Position;
            _endTilePosition = wayProperties.UniqueTiles.First(x => x.Type == VoxelTile.TypeTile.EndTile).Position;
            _conductAppender = new ConductAppender(ref _placedTiles, tileProperties);
        }

        public void SwitchAtlas() 
        { 
            foreach (VoxelTile tile in _placedTiles)
            {
                if(tile != null) 
                    tile.colors.DisableMaterial();
            }
            _placedTiles[_startTilePosition.x,_startTilePosition.y].colors.EnableMaterial();
            Debug.Log(_placedTiles[_startTilePosition.x,_startTilePosition.y].colors.isActivated);
            List<VoxelTile> tilesToCheck = new List<VoxelTile> 
                { _placedTiles[_startTilePosition.x, _startTilePosition.y] };
            while (tilesToCheck.ToArray().Length != 0)
            {
                VoxelTile tile = tilesToCheck[0];
                if (tile.colors.isActivated && (!ConductAppender.IsGate(tile) || ConductAppender.IsGateOpen(tile)))
                {
                    (int x, int y) = (tile.x, tile.y);
                    if (!TileAppender.IsOutOfLowerRange(x) && _placedTiles[x - 1, y] != null && 
                        _conductAppender.IsConductLeftToNeighborRight(x, y) && !_placedTiles[x - 1, y].colors.isActivated)
                    {
                        _placedTiles[x - 1, y].colors.EnableMaterial(); 
                        tilesToCheck.Add(_placedTiles[x - 1, y]);
                    }
                    if (!TileAppender.IsOutOfUpperRange(x, 0, _placedTiles) && _placedTiles[x + 1, y] != null && 
                        _conductAppender.IsConductRightToNeighborLeft(x, y) && !_placedTiles[x + 1, y].colors.isActivated)
                    {
                        _placedTiles[x + 1, y].colors.EnableMaterial();
                        tilesToCheck.Add(_placedTiles[x + 1, y]);
                    }
                    if (!TileAppender.IsOutOfLowerRange(y) && _placedTiles[x, y - 1] != null && 
                        _conductAppender.IsConductBackToNeighborFront(x, y) && !_placedTiles[x, y - 1].colors.isActivated)
                    {
                        _placedTiles[x, y - 1].colors.EnableMaterial();
                        tilesToCheck.Add(_placedTiles[x, y - 1]);
                    }
                    if (!TileAppender.IsOutOfUpperRange(y, 1, _placedTiles) && _placedTiles[x, y + 1] != null && 
                        _conductAppender.IsConductFrontToNeighborBack(x, y) && !_placedTiles[x, y + 1].colors.isActivated)
                    {
                        _placedTiles[x, y + 1].colors.EnableMaterial();
                        tilesToCheck.Add(_placedTiles[x, y + 1]);
                    }
                }
            
                tilesToCheck.Remove(tile);
            }

            if(_placedTiles[_endTilePosition.x, _endTilePosition.y].colors.isActivated)
                EventManager.SendEndReached();
        }
    }