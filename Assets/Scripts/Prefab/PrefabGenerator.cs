using System;
using System.Collections.Generic;
using UnityEngine;

public class PrefabGenerator : MonoBehaviour
{
    public List<VoxelTile> prefabs = new List<VoxelTile>();
    private Transform parent;

    private void Awake()
    {
        parent = transform;
        int countBeforeAdding = prefabs.Count;
        for (int i = 0; i < countBeforeAdding; i++)
        {
            switch (prefabs[i].rotation)
            {
                case VoxelTile.RotationType.OnlyRotation:
                    break;

                case VoxelTile.RotationType.TwoRotations:
                    InstantiatePrefab(prefabs[i], 1);
                    break;

                case VoxelTile.RotationType.FourRotations:
                    InstantiatePrefab(prefabs[i], 1);
                    InstantiatePrefab(prefabs[i], 2);
                    InstantiatePrefab(prefabs[i], 3);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();

            }
        }
    }

    void InstantiatePrefab(VoxelTile prefab, int rotateIterationCount)
    {
        VoxelTile clone = Instantiate(prefab, 
            prefab.transform.position + Vector3.forward * rotateIterationCount, 
            Quaternion.Euler(0, 90 * rotateIterationCount, 0));
        GameObject cloneGameObject = clone.gameObject;
        cloneGameObject.transform.parent = parent;
        cloneGameObject.name = $"{prefab.name}_{rotateIterationCount * 90}";
        prefabs.Add(clone);
    }
}
