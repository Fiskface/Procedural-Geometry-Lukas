using Grid;
using UnityEngine;

[RequireComponent(typeof(GridTile))]
public class Piece : MonoBehaviour 
{
    private void OnDrawGizmos() {
        var tile = GetComponent<GridTile>();
        if (tile.GetProperty(GridTileProperty.Solid)) {
            Gizmos.color = Color.red;
        } else {
            Gizmos.color = Color.green;
        }
        
        if (tile.GetNeighbourProperty(0, GridTileProperty.Solid)) {
            Gizmos.color = Color.yellow;
        }
        
        Gizmos.DrawCube(transform.position, new Vector3(1, 0.1f, 1));
    }
}
