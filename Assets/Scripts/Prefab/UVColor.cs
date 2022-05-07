using System;
using UnityEngine;

public class UVColor : MonoBehaviour
{
    [SerializeField] private int tileSize = 5;
    [Header("Material states")] 
    [SerializeField] [InspectorName("Enabled state")] private Material enabledMaterial;
    [SerializeField] [InspectorName("Disabled state")] private Material disabledMaterial;
    
    private const float VoxelSize = 0.1f;
    private const float Half = VoxelSize / 2;

    public bool isActivated;
    private MeshRenderer meshRenderer;
    
    [HideInInspector] public byte[] frontSide;
    [HideInInspector] public byte[] backSide;
    [HideInInspector] public byte[] leftSide;
    [HideInInspector] public byte[] rightSide;

    public void Awake()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        isActivated = false;
    }

    public UVColor(int size, bool isActivated)
    {
        tileSize = size;
    }
    public void GenerateColors()
    {
        Vector3 minPointMesh = GetComponentInChildren<MeshCollider>().bounds.min;
        frontSide = new byte[tileSize * tileSize];
        backSide = new byte[tileSize * tileSize];
        leftSide = new byte[tileSize * tileSize];
        rightSide = new byte[tileSize * tileSize];
        for (int height = 0; height < tileSize; height++)
        {
            for (int width = 0; width < tileSize; width++)
            {
                frontSide[width + tileSize * height] =
                    GetColor(new Vector3(
                            minPointMesh.x + Half + width * VoxelSize, // Axis X
                            minPointMesh.y + Half + height * VoxelSize, // Axis Y
                            minPointMesh.z + -Half),  // Axis Z
                        Vector3.forward);

                backSide[width + tileSize * height] =
                    GetColor(new Vector3(
                            minPointMesh.x + Half + width * VoxelSize, // Axis X
                            minPointMesh.y + Half + height * VoxelSize, // Axis Y
                            minPointMesh.z + Half + tileSize * VoxelSize),  // Axis Z
                        Vector3.back);

                rightSide[width + tileSize * height] =
                    GetColor(new Vector3(
                            minPointMesh.x + -Half, // Axis X
                            minPointMesh.y + Half + height * VoxelSize, // Axis Y
                            minPointMesh.z + Half + width * VoxelSize),  // Axis Z
                        Vector3.right);

                leftSide[width + tileSize * height] =
                    GetColor(new Vector3(
                            minPointMesh.x + Half + tileSize * VoxelSize, // Axis X
                            minPointMesh.y + Half + height * VoxelSize, // Axis Y
                            minPointMesh.z + Half + width * VoxelSize),  // Axis Z
                        Vector3.left);
            }
        }
    }

    byte GetColor(Vector3 rayStartPos, Vector3 rayDirection)
    {
        if (Physics.Raycast(new Ray(rayStartPos, rayDirection), out RaycastHit hit, VoxelSize))
            return (byte)(hit.textureCoord.x * 256);
        else
            return 0;
    }
    
    public void DisableMaterial()
    {
        meshRenderer.material = disabledMaterial; 
        isActivated = false;
    }

    public void EnableMaterial()
    {
        meshRenderer.material = enabledMaterial;
        isActivated = true;
    }
}
