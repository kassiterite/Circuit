using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
[RequireComponent(typeof(MapGenerator))]
public class MapLogic : MonoBehaviour
{
    [SerializeField] private int conductUVColor = 251;
    private VoxelTile[,] _placedTiles;
    private MapGenerator _mapGenerator;
    private WayGenerator _wayGenerator;
    private void Start()
    {
        _mapGenerator = GetComponent<MapGenerator>();
        _wayGenerator = GetComponent<WayGenerator>();
        _placedTiles = _mapGenerator.placedTiles;
        EventManager.onGateStateChange += SwitchAtlas;
    }

    public void SwitchAtlas()
    {
        foreach (VoxelTile tile in _placedTiles)
        {
            if(tile != null) 
                tile.colors.DisableMaterial();
        }
        _placedTiles[_wayGenerator.startTilePos.x,_wayGenerator.startTilePos.y].colors.EnableMaterial();
        List<VoxelTile> tilesToCheck = new List<VoxelTile> 
            { _placedTiles[_wayGenerator.startTilePos.x,_wayGenerator.startTilePos.y] };
        while (tilesToCheck.ToArray().Length != 0)
        {
            VoxelTile tile = tilesToCheck[0];
            if (tile.colors.isActivated && (!IsGate(tile) || IsGateOpen(tile)))
            {
                (int x, int y) = (tile.x, tile.y);
                if (!MapGenerator.IsOutOfLowerRange(x) && _placedTiles[x - 1, y] != null && 
                    IsConductLeftToNeighborRight(x, y) && !_placedTiles[x - 1, y].colors.isActivated)
                {
                    _placedTiles[x - 1, y].colors.EnableMaterial(); 
                    tilesToCheck.Add(_placedTiles[x - 1, y]);
                }
                if (!_mapGenerator.IsOutOfUpperRange(x, 0) && _placedTiles[x + 1, y] != null && 
                    IsConductRightToNeighborLeft(x, y) && !_placedTiles[x + 1, y].colors.isActivated)
                {
                    _placedTiles[x + 1, y].colors.EnableMaterial();
                    tilesToCheck.Add(_placedTiles[x + 1, y]);
                }
                if (!MapGenerator.IsOutOfLowerRange(y) && _placedTiles[x, y - 1] != null && 
                    IsConductBackToNeighborFront(x, y) && !_placedTiles[x, y - 1].colors.isActivated)
                {
                    _placedTiles[x, y - 1].colors.EnableMaterial();
                    tilesToCheck.Add(_placedTiles[x, y - 1]);
                }
                if (!_mapGenerator.IsOutOfUpperRange(y, 1) && _placedTiles[x, y + 1] != null && 
                    IsConductFrontToNeighborBack(x, y) && !_placedTiles[x, y + 1].colors.isActivated)
                {
                    _placedTiles[x, y + 1].colors.EnableMaterial();
                    tilesToCheck.Add(_placedTiles[x, y + 1]);
                }
            }
            
            tilesToCheck.Remove(tile);
        }

        if(_placedTiles[_wayGenerator.endTilePos.x, _wayGenerator.endTilePos.y].colors.isActivated)
            EventManager.SendEndReached();
    }
    private bool IsConductLeftToNeighborRight(int x, int y) =>
        IsCanConduct(_placedTiles[x - 1, y].colors.leftSide) && IsCanConduct(_placedTiles[x, y].colors.rightSide);
    
    private bool IsConductRightToNeighborLeft(int x, int y) =>
        IsCanConduct(_placedTiles[x + 1, y].colors.rightSide) && IsCanConduct(_placedTiles[x, y].colors.leftSide);
    
    private bool IsConductBackToNeighborFront(int x, int y) =>
        IsCanConduct(_placedTiles[x, y - 1].colors.backSide) && IsCanConduct(_placedTiles[x, y].colors.frontSide);
    
    private bool IsConductFrontToNeighborBack(int x, int y) =>
        IsCanConduct(_placedTiles[x, y + 1].colors.frontSide) && IsCanConduct(_placedTiles[x, y].colors.backSide);
    
    private bool IsCanConduct(byte[] side) => side[12] == conductUVColor;

    private bool IsGateOpen(VoxelTile tile) =>
        tile.GetComponentInChildren<ActivateTorch>().isEnable;

    private bool IsGate(VoxelTile tile) =>
        tile.type == VoxelTile.TypeTile.GateTile;
}
