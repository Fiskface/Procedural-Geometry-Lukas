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

    private float adjustForStretchInSolid = (float)Math.Sqrt((1 / 6f) * (1 / 6f) + 0.2f * 0.2f);
    
    private void Start()
    {
        tile = GetComponent<GridTile>();
        var meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh { name = "Piece" };
        meshFilter.sharedMesh = mesh;
    }

    private void Update()
    {
        builder.ClearLists();
        CheckCurrentState();
        MakePiece();
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
        if (neighbor < 0)
            neighbor += 8;
        if (neighbor > 7)
            neighbor -= 8;
        
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

    private bool Check2StepsClockwise(int i, state state)
    {
        if (i <= 1)
            i += 8;
        i -= 2;
        return CheckNeighborCurrentState(i) == state;
    }

    private bool Check2StepsCounterClockwise(int i, state state)
    {
        if (i >= 6)
            i -= 8;
        i += 2;
        return CheckNeighborCurrentState(i) == state;
    }

    private void MakePiece()
    {
        //Stoppar texurerna från att snurra hela tiden.
        builder.VertexMatrix = Matrix4x4.identity;
        
        if (currentState == state.Solid)
        {
            builder.CreateQuad(new Vector3(-1/6f, -0.2f, -1/6f),new Vector3(-1/6f, -0.2f, 1/6f), new Vector3(1/6f, -0.2f, 1/6f), 
                new Vector3(1/6f, -0.2f, -1/6f), (int)currentState, 0);
            
            for (int i = 0; i < 4; i++)
            {
                builder.VertexMatrix = Matrix4x4.Rotate(Quaternion.AngleAxis(-90 * i, new Vector3(0, 1, 0)));
                
                //Gräshörn                 
                builder.CreateQuad(new Vector3(1/3f, 0f, 0.5f),new Vector3(0.5f, 0f, 0.5f), new Vector3(0.5f, 0f, 1/3f), 
                    new Vector3(1/3f, 0f, 1/3f), (int)state.Grass, i);
                                 
                if(CheckNeighborCurrentState(i*2) == state.Solid)
                {
                    //Plattan som connectar vägen.
                    builder.CreateQuad(new Vector3(1/6f, -0.2f, -1/6f),new Vector3(1/6f, -0.2f, 1/6f), new Vector3(0.5f, -0.2f, 1/6f), 
                        new Vector3(0.5f, -0.2f, -1/6f), (int)currentState, i);

                    
                    //Make smooth corners in wall when path is turning
                    if (Check2StepsClockwise(i*2, state.Solid))
                    {
                        builder.CreateQuad(new Vector3(0.5f, -0.2f, -1 / 6f), new Vector3(0.5f, 0, -1 / 3f),
                            new Vector3(1 / 3f, 0, -1 / 3f), new Vector3(1 / 6f, -0.2f, -1 / 6f), (int)currentState, i,
                            new Vector2(0.5f, -1/6f), new Vector2(0.5f, -1/6f - adjustForStretchInSolid), 
                            new Vector2(1/3f, -1/6f - adjustForStretchInSolid),new Vector2(1/6f, -1/6f));
                    }
                    else
                    {
                        builder.CreateQuad(new Vector3(0.5f, -0.2f, -1/6f),new Vector3(0.5f, 0, -1/3f),
                            new Vector3(1/6f, 0, -1/3f), new Vector3(1/6f,-0.2f, -1/6f), (int)currentState, i, 
                            new Vector2(0.5f, -1/6f), new Vector2(0.5f, -1/6f - adjustForStretchInSolid), 
                            new Vector2(1/6f, -1/6f - adjustForStretchInSolid),new Vector2(1/6f, -1/6f));
                    }
                    if (Check2StepsCounterClockwise(i*2, state.Solid))
                    {
                        builder.CreateQuad(new Vector3(1/6f, -0.2f, 1/6f),new Vector3(1/3f, 0, 1/3f), 
                            new Vector3(0.5f, 0, 1/3f), new Vector3(0.5f, -0.2f, 1/6f), (int)currentState, i, 
                            new Vector2(1/6f, 1/6f), new Vector2(1/3f, 1/6f + adjustForStretchInSolid), 
                            new Vector2(0.5f, 1/6f + adjustForStretchInSolid),new Vector2(0.5f, 1/6f));
                    }
                    else
                    {
                        builder.CreateQuad(new Vector3(1/6f, -0.2f, 1/6f),new Vector3(1/6f, 0, 1/3f),
                            new Vector3(0.5f, 0, 1/3f), new Vector3(0.5f, -0.2f, 1/6f), (int)currentState, i,
                            new Vector2(1/6f, 1/6f), new Vector2(1/6f, 1/6f + adjustForStretchInSolid), 
                            new Vector2(0.5f, 1/6f + adjustForStretchInSolid),new Vector2(0.5f, 1/6f));
                    }
                }
                
                //If it's not a path
                else
                {
                    builder.CreateQuad(new Vector3(1/6f, -0.2f, 1/6f),new Vector3(1/3f, 0, 1/6f), 
                        new Vector3(1/3f, 0, -1/6f), new Vector3(1/6f, -0.2f, -1/6f), (int)currentState, i, 
                        new Vector2(1/6f, 1/6f), new Vector2(1/6f + adjustForStretchInSolid, 1/6f), 
                        new Vector2(1/6f + adjustForStretchInSolid, -1/6f),new Vector2(1/6f, -1/6f));
                    
                    builder.CreateQuad(new Vector3(1/3f, 0f, 1/3f),new Vector3(0.5f, 0f, 1/3f), new Vector3(0.5f, 0, -1/3f), 
                        new Vector3(1/3f, 0, -1/3f), (int)state.Grass, i);
                    
                    if (!Check2StepsClockwise(i*2, state.Solid))
                    {
                        builder.CreateTriangle(new Vector3(1/6f, -0.2f, -1/6f), new Vector3(1/3f, 0, -1/6f),
                            new Vector3(1/3f, 0, -1/3f), (int)currentState, i,
                            new Vector2(1/6f, -1/6f), 
                            new Vector2(1/6f + adjustForStretchInSolid, -1/6f), new Vector2(1/6f + adjustForStretchInSolid, -1/3f));
                        
                        
                        builder.CreateTriangle(new Vector3(1/6f, -0.2f, -1/6f), new Vector3(1/3f, 0, -1/3f),
                            new Vector3(1/6f,0, -1/3f), (int)currentState, i,
                            new Vector2(1/6f, -1/6f),
                            new Vector2(1/3f, -1/6f - adjustForStretchInSolid), new Vector2(1/6f, -1/6f - adjustForStretchInSolid));
                    }
                }
                
            }
        }
        
        else if (currentState == state.Grass)
        {
            
            builder.CreateQuad(new Vector3(-0.5f, 0, -0.5f),new Vector3(-0.5f, 0, 0.5f), new Vector3(0.5f, 0, 0.5f), 
                new Vector3(0.5f, 0, -0.5f), (int)currentState, 0);

            
        }
        
        else if (currentState == state.Water)
        {
            builder.CreateQuad(new Vector3(-0.5f, -0.1f, -0.5f),new Vector3(-0.5f, -0.1f, 0.5f), new Vector3(0.5f, -0.1f, 0.5f), 
                new Vector3(0.5f, -0.1f, -0.5f), (int)currentState, 0);
            
            for (int i = 0; i < 4; i++)
            {
                builder.VertexMatrix = Matrix4x4.Rotate(Quaternion.AngleAxis(-90 * i, new Vector3(0, 1, 0)));

                if (CheckNeighborCurrentState(i*2) == state.Water)
                {
                    if (Check2StepsClockwise(i*2, state.Water) && CheckNeighborCurrentState(i*2 - 1) != state.Water)
                    {
                        builder.CreateTriangle(new Vector3(0.5f, -0.1f, -0.4f), new Vector3(0.5f, 0, -0.5f),
                            new Vector3(0.4f, -0.1f, -0.5f), (int)state.Solid, i);
                        
                    }
                }
                
                else if (CheckNeighborCurrentState(i * 2) == state.Bridge)
                {
                    builder.CreateQuad(new Vector3(0.4f, -0.1f, 0.5f),new Vector3(0.5f, 0f, 0.5f), new Vector3(0.5f, 0, 1/6f), 
                        new Vector3(0.4f, -0.1f, 1/6f), (int)state.Solid, i);
                    
                    builder.CreateTriangle(new Vector3(0.4f, -0.1f, 1/6f), new Vector3(0.5f, 0, 1/6f),
                        new Vector3(0.5f, -0.1f, 1/6f), (int)state.Solid, i,
                        new Vector2(0.4f, -0.1f), new Vector2(0.5f, 0), new Vector2(0.5f, -0.1f));
                    
                    builder.CreateQuad(new Vector3(0.4f, -0.1f, -1/6f),new Vector3(0.5f, 0f, -1/6f), new Vector3(0.5f, 0, -0.5f), 
                        new Vector3(0.4f, -0.1f, -0.5f), (int)state.Solid, i);
                    
                    builder.CreateTriangle(new Vector3(0.5f, -0.1f, -1/6f), new Vector3(0.5f, 0, -1/6f),
                        new Vector3(0.4f, -0.1f, -1/6f), (int)state.Solid, i,
                        new Vector2(0.5f, -0.1f), new Vector2(0.5f, 0), new Vector2(0.4f, -0.1f));
                }

                else
                {
                    builder.CreateQuad(new Vector3(0.4f, -0.1f, 0.5f),new Vector3(0.5f, 0f, 0.5f), new Vector3(0.5f, 0, -0.5f), 
                                            new Vector3(0.4f, -0.1f, -0.5f), (int)state.Solid, i);
                }
                
            }
        }
        
        else if (currentState == state.Bridge)
        {
            builder.CreateQuad(new Vector3(-1/6f, -0.1f, -1/6f),new Vector3(-1/6f, -0.1f, 1/6f), new Vector3(1/6f, -0.1f, 1/6f), 
                new Vector3(1/6f, -0.1f, -1/6f), (int)state.Water, 0);
            
            builder.CreateQuad(new Vector3(-1/6f, 0.3f, -1/6f),new Vector3(-1/6f, 0.3f, 1/6f), new Vector3(1/6f, 0.3f, 1/6f), 
                new Vector3(1/6f, 0.3f, -1/6f), (int)state.Bridge, 0);
            
            builder.CreateQuad(new Vector3(1/6f, 0.3f, -1/6f),new Vector3(1/6f, 0.3f, 1/6f), new Vector3(-1/6f, 0.3f, 1/6f), 
                new Vector3(-1/6f, 0.3f, -1/6f), (int)state.Bridge, 0);
            
            for(int j = 0; j < 4; j++)
            {
                builder.VertexMatrix = Matrix4x4.Translate(new Vector3(0f, 0 ,0)) *
                                       Matrix4x4.Rotate(Quaternion.AngleAxis(-90 * j, new Vector3(0, 1, 0)));
                for (int i = 0; i < 4; i++)
                {
                    builder.VertexMatrix *= Matrix4x4.Translate(new Vector3(1 / 6f - 0.025f, 0, 1 / 6f - 0.025f)) *
                                           Matrix4x4.Rotate(Quaternion.AngleAxis(-90 * i, new Vector3(0, 1, 0))) *
                                           Matrix4x4.Translate(new Vector3(-(1 / 6f - 0.025f), 0, -(1 / 6f - 0.025f)));

                    builder.CreateQuad(new Vector3(1 / 6f - 0.05f, -0.1f, 1 / 6f),
                        new Vector3(1 / 6f - 0.05f, 0.3f, 1 / 6f), new Vector3(1 / 6f - 0.05f, 0.3f, 1 / 6f - 0.05f),
                        new Vector3(1 / 6f - 0.05f, -0.1f, 1 / 6f - 0.05f), (int)state.Bridge, 0,
                        new Vector2((1 / 6f), -0.1f), new Vector2((1 / 6f), 0.3f),
                        new Vector2((1 / 6f - 0.05f), 0.3f), new Vector2((1 / 6f - 0.05f), -0.1f));
                }
            }
            

            for (int i = 0; i < 4; i++)
            {
                builder.VertexMatrix = Matrix4x4.Rotate(Quaternion.AngleAxis(-90 * i, new Vector3(0, 1, 0)));
                
                builder.CreateQuad(new Vector3(1/6f, 0, 0.5f), new Vector3(0.5f, 0, 0.5f),
                    new Vector3(0.5f, 0, 1/6f), new Vector3(1/6f, 0, 1/6f), (int)state.Grass, i);

                if (CheckNeighborCurrentState(i * 2) == state.Bridge)
                {
                    builder.CreateQuad(new Vector3(1 / 6f, 0.3f, 1 / 6f), new Vector3(0.5f, 0.3f, 1 / 6f),
                        new Vector3(0.5f, 0.3f, -1 / 6f),
                        new Vector3(1 / 6f, 0.3f, -1 / 6f), (int)state.Bridge, i);
                    
                    builder.CreateQuad(new Vector3(1 / 6f, 0.3f, -1 / 6f), new Vector3(0.5f, 0.3f, -1 / 6f),
                        new Vector3(0.5f, 0.3f, 1 / 6f), new Vector3(1 / 6f, 0.3f, 1 / 6f), 
                        (int)state.Bridge, i );
                }

                if (CheckNeighborCurrentState(i * 2) == state.Water || CheckNeighborCurrentState(i*2) == state.Bridge)
                {
                    builder.CreateQuad(new Vector3(1 / 6f, -0.1f, 1 / 6f), new Vector3(0.5f, -0.1f, 1 / 6f),
                        new Vector3(0.5f, -0.1f, -1 / 6f),
                        new Vector3(1 / 6f, -0.1f, -1 / 6f), (int)state.Water, i);
                    
                    builder.CreateQuad(new Vector3(1/6f, -0.1f, 1/6f),new Vector3(1/6f, 0f, 1/6f), new Vector3(0.5f, 0f, 1/6f), 
                        new Vector3(0.5f, -0.1f, 1/6f), (int)state.Solid, i,
                        new Vector2(1/6f, -0.1f), new Vector2(1/6f, 0f),
                        new Vector2(0.5f, 0f), new Vector2(0.5f, -0.1f));
                    
                    builder.CreateQuad(new Vector3(0.5f, -0.1f, -1/6f),new Vector3(0.5f, 0f, -1/6f), new Vector3(1/6f, 0f, -1/6f), 
                        new Vector3(1/6f, -0.1f, -1/6f), (int)state.Solid, i,
                        new Vector2(0.5f, -0.1f), new Vector2(0.5f, 0f),
                        new Vector2(1/6f, 0f), new Vector2(1/6f, -0.1f));
                }
                else
                {
                    builder.CreateQuad(new Vector3(1/6f, 0f, 1/6f),new Vector3(0.5f, 0f, 1/6f), new Vector3(0.5f, 0f, -1/6f), 
                        new Vector3(1/6f, 0f, -1/6f), (int)state.Grass, i);
                    
                    builder.CreateQuad(new Vector3(1/6f, -0.1f, 1/6f),new Vector3(1/6f, 0f, 1/6f), new Vector3(1/6f, 0f, -1/6f), 
                        new Vector3(1/6f, -0.1f, -1/6f), (int)state.Solid, i,
                        new Vector2(1/6f, -0.1f), new Vector2(1/6f, 0f),
                        new Vector2(-1/6f, 0f), new Vector2(-1/6f, -0.1f));
                    
                    //Stairs onto the bridge go here
                    builder.CreateQuad(new Vector3(1/6f - 0.05f, 0.2f, 1/6f),new Vector3(1/3f - 0.05f, 0.2f, 1/6f), new Vector3(1/3f - 0.05f, 0.2f, -1/6f), 
                        new Vector3(1/6f - 0.05f, 0.2f, -1/6f), (int)state.Bridge, i);
                    builder.CreateQuad(new Vector3(1/6f - 0.05f, 0.2f, -1/6f),new Vector3(1/3f - 0.05f, 0.2f, -1/6f), new Vector3(1/3f - 0.05f, 0.2f, 1/6f), 
                        new Vector3(1/6f - 0.05f, 0.2f, 1/6f), (int)state.Bridge, i);
                    
                    builder.CreateQuad(new Vector3(1/3f - 0.05f, 0.1f, 1/6f),new Vector3(0.5f - 0.05f, 0.1f, 1/6f), new Vector3(0.5f - 0.05f, 0.1f, -1/6f), 
                        new Vector3(1/3f - 0.05f, 0.1f, -1/6f), (int)state.Bridge, i);
                    builder.CreateQuad(new Vector3(1/3f - 0.05f, 0.1f, -1/6f),new Vector3(0.5f - 0.05f, 0.1f, -1/6f), new Vector3(0.5f - 0.05f, 0.1f, 1/6f), 
                        new Vector3(1/3f - 0.05f, 0.1f, 1/6f), (int)state.Bridge, i);
                    
                    //Stair walls here
                    builder.CreateQuad(new Vector3(1/6f , 0.2f, -1/6f), new Vector3(1/6f, 0.3f, -1/6f),
                        new Vector3(0.5f, 0, -1/6f), new Vector3(0.4f, 0, -1/6f), (int)currentState, i,
                        new Vector2(1/6f, 0.2f), new Vector2(1/6f, 0.3f),
                        new Vector2(0.5f, 0), new Vector2(0.4f, 0));
                    builder.CreateQuad(new Vector3(0.4f, 0, -1/6f), new Vector3(0.5f, 0, -1/6f),
                        new Vector3(1/6f, 0.3f, -1/6f), new Vector3(1/6f , 0.2f, -1/6f), (int)currentState, i,
                        new Vector2(0.4f, 0), new Vector2(0.5f, 0),
                        new Vector2(1/6f, 0.3f), new Vector2(1/6f , 0.2f));
                    
                    
                    builder.CreateQuad(new Vector3(0.5f , 0, 1/6f), new Vector3(1/6f, 0.3f, 1/6f),
                        new Vector3(1/6f, 0.2f, 1/6f), new Vector3(0.4f, 0, 1/6f), (int)currentState, i,
                        new Vector2(0.5f , 0), new Vector2(1/6f, 0.3f),
                        new Vector2(1/6f, 0.2f), new Vector2(0.4f, 0));
                    builder.CreateQuad(new Vector3(0.4f, 0, 1/6f), new Vector3(1/6f, 0.2f, 1/6f),
                        new Vector3(1/6f, 0.3f, 1/6f), new Vector3(0.5f , 0, 1/6f), (int)currentState, i,
                        new Vector2(0.4f, 0), new Vector2(1/6f, 0.2f),
                        new Vector2(1/6f, 0.3f), new Vector2(0.5f , 0));
                }
            }
        }
    }
    
}
