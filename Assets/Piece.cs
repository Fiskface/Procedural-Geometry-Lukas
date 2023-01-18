using System;
using Grid;
using Scenes;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(GridTile), typeof(MeshFilter), typeof(MeshRenderer))]
public class Piece : MonoBehaviour
{
    private Mesh mesh;
    private GridTile tile;
    private state currentState;
    
    private void Start()
    {
        tile = GetComponent<GridTile>();
        var meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh { name = "Piece" };
        meshFilter.sharedMesh = mesh;
    }

    private void Update()
    {
        MeshBuilder builder = new MeshBuilder();
        
        
        
        
        builder.CreateQuad(new Vector3(-0.5f, 0, -0.5f),new Vector3(-0.5f, 0, 0.5f), new Vector3(0.5f, 0, 0.5f),
            new Vector3(0.5f, 0, -0.5f), new Vector3(0, 1, 0), );
        
        builder.Build(mesh);
    }

    private void OnDrawGizmos() {
        
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

    enum state
    {
        Grass,
        Solid,
        Water,
        Bridge
    }
    

}
