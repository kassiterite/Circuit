using UnityEngine;

public class TilePlacer : MonoBehaviour
{
    private Transform _transform;
    public TilePlacer(Transform transform)
    {
        _transform = transform;
    }
    public void PlaceTile(VoxelTile tile, int x, int y)
    {
        const float half = 0.5f;
        var currentPos = _transform.position;
        Vector3 position = new Vector3(currentPos.x + half * x, currentPos.y, currentPos.z + half * y); 
        VoxelTile clone = Instantiate(tile, position, tile.gameObject.transform.rotation); 
        clone.transform.parent = _transform; 
        clone.name = $"tile [{x},{y}]";
    }
}
