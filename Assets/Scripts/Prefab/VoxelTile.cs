using UnityEngine;

[RequireComponent(typeof(UVColor))]

public class VoxelTile : MonoBehaviour
{
    public TypeTile type;
    public RotationType rotation;
    [HideInInspector] public UVColor colors;
    [HideInInspector] public int x;
    [HideInInspector] public int y;
    
    public enum TypeTile
    {
        StartTile,
        EndTile,
        ConductTile,
        GateTile,
        PlugTile
    }

    public enum RotationType
    {
        OnlyRotation,
        TwoRotations,
        FourRotations
    }

    private void Awake()
    {
        colors = GetComponent<UVColor>();
    }
}
