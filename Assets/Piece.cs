using System;
using Grid;
using Scenes;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteAlways]
[RequireComponent(typeof(GridTile), typeof(MeshFilter), typeof(MeshRenderer))]
public class Piece : MonoBehaviour
{
    MeshBuilder builder = new MeshBuilder();
    
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
        CheckCurrentState();
        
        MakeCenter();
        
        //builder.CreateQuad(new Vector3(-0.5f, 0, -0.5f),new Vector3(-0.5f, 0, 0.5f), new Vector3(0.5f, 0, 0.5f), 
        //    new Vector3(0.5f, 0, -0.5f), new Vector3(0, 1, 0), (int)currentState);
        
        builder.Build(mesh);
    }

    enum state
    {
        Grass,
        Solid,
        Water,
        Bridge
    }
    
    private void CheckCurrentState()
    {
        if (tile.GetProperty(GridTileProperty.Solid) && tile.GetProperty(GridTileProperty.Water))
        {
            currentState = state.Bridge;
        }
        else
        {
            if (tile.GetProperty(GridTileProperty.Solid))
            {
                currentState = state.Solid;
            }
            else if (tile.GetProperty(GridTileProperty.Water))
            {
                currentState = state.Water;
            }
            else
            {
                currentState = state.Grass;
            }
        }
    }

    private state CheckNeighborCurrentState(int neighbor)
    {
        if (tile.GetNeighbourProperty(neighbor, GridTileProperty.Solid) && tile.GetNeighbourProperty(neighbor, GridTileProperty.Water))
        {
            return state.Bridge;
        }
        else
        {
            if (tile.GetNeighbourProperty(neighbor, GridTileProperty.Solid))
            {
                return state.Solid;
            }
            else if (tile.GetNeighbourProperty(neighbor, GridTileProperty.Water))
            {
                return state.Water;
            }
            else
            {
                return state.Grass;
            }
        }
    }

    private void MakeCenter()
    {
        if (currentState == state.Solid)
        {
            builder.CreateQuad(new Vector3(-1/6f, -0.3f, -1/6f),new Vector3(-1/6f, -0.3f, 1/6f), new Vector3(1/6f, -0.3f, 1/6f), 
                new Vector3(1/6f, -0.3f, -1/6f), new Vector3(0, 1, 0), (int)currentState);

            for (int i = 0; i < 4; i++)
            {
                if(CheckNeighborCurrentState(i*2) == state.Solid)
                {
                    /*Matrix4x4 mat =
                        Matrix4x4.Translate(new Vector3(-0.5f, 0, i * 0.5f)) *
                        Matrix4x4.Rotate(Quaternion.AngleAxis(-90, new Vector3(0, 1, 0)));

                    builder.VertexMatrix = mat;*/
                    
                    builder.CreateQuad(new Vector3(1/6f, -0.3f, -1/6f),new Vector3(1/6f, -0.3f, 1/6f), new Vector3(0.5f, -0.3f, 1/6f), 
                        new Vector3(0.5f, -0.3f, -1/6f), new Vector3(0, 1, 0), (int)currentState);
                }
            }
            
        }
        else if (currentState == state.Grass)
        {
            
            builder.CreateQuad(new Vector3(-0.5f, 0, -0.5f),new Vector3(-0.5f, 0, 0.5f), new Vector3(0.5f, 0, 0.5f), 
                new Vector3(0.5f, 0, -0.5f), new Vector3(0, 1, 0), (int)currentState);
        }
        else if (currentState == state.Water)
        {
            builder.CreateQuad(new Vector3(-0.5f, 0, -0.5f),new Vector3(-0.5f, 0, 0.5f), new Vector3(0.5f, 0, 0.5f), 
                new Vector3(0.5f, 0, -0.5f), new Vector3(0, 1, 0), (int)currentState);
        }
        else if (currentState == state.Bridge)
        {
            builder.CreateQuad(new Vector3(-0.5f, 0, -0.5f),new Vector3(-0.5f, 0, 0.5f), new Vector3(0.5f, 0, 0.5f), 
                new Vector3(0.5f, 0, -0.5f), new Vector3(0, 1, 0), (int)currentState);
        }
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
    
    
}
