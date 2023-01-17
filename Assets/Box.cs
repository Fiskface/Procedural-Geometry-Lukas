using System.Collections;
using System.Collections.Generic;
using Scenes;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Box : MonoBehaviour
{
    private Mesh mesh;
    void Start()
    {
        var meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh { name = "Box" };
        meshFilter.sharedMesh = mesh;
    }
    
    
    void Update()
    {
        MeshBuilder builder = new MeshBuilder();
        
        float t = Time.time; // In seconds
        t -= Mathf.Floor(t); // 0.0 to 1.0
        t -= 0.5f;           // -0.5 to 0.5
        t *= 2.0f;           // -1.0 to 1.0
        t = Mathf.Abs(t);    // 0.0 to 1.0
        t = Mathf.SmoothStep(0.0f, 1.0f, t);
        
        //Roof
        builder.VertexMatrix = Matrix4x4.Translate(new Vector3(0, 1, 0)) * Matrix4x4.Rotate(Quaternion.AngleAxis(t*-90, Vector3.right));
        
        builder.TextureMatrix = 
            Matrix4x4.Translate(new Vector3(0.5f, 0.5f, 0.0f)) *
            Matrix4x4.Scale(new Vector3(0.5f, 0.5f, 1.0f));
        
        
        int a = builder.AddVertex(
            new Vector3(0, 0, 0), 
            new Vector3(0, 1, 0), 
            new Vector2(0, 0));
        int b = builder.AddVertex(
            new Vector3(0, 0, 1), 
            new Vector3(0, 1, 0), 
            new Vector2(0, 1));
        int c = builder.AddVertex(
            new Vector3(1, 0, 1), 
            new Vector3(0, 1, 0), 
            new Vector2(1, 1));
        int d = builder.AddVertex(
            new Vector3(1, 0, 0), 
            new Vector3(0, 1, 0), 
            new Vector2(1, 0));
        
        builder.AddQuad(a, b, c, d);

        //Walls
        for (int i = 0; i < 4; i++)
        {
            for (int j = -1; j <= 1; j += 2)
            {
                Matrix4x4 mat =
                    Matrix4x4.Translate(new Vector3(0.5f, 0, 0.5f)) *
                    Matrix4x4.Rotate(Quaternion.AngleAxis(90 * i, Vector3.up)) *
                    Matrix4x4.Translate(new Vector3(-0.5f, 0, j * 0.5f)) *
                    Matrix4x4.Rotate(Quaternion.AngleAxis(-90, Vector3.right));

                builder.VertexMatrix = mat;
                builder.TextureMatrix = 
                    Matrix4x4.Translate(new Vector3(0.0f, 0.5f, 0.0f)) *
                    Matrix4x4.Scale(new Vector3(0.5f, 0.5f, 1.0f));
                

                int w0 = builder.AddVertex(new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector2(0, 0));
                int w1 = builder.AddVertex(new Vector3(0, 0, 1), new Vector3(0, 1, 0), new Vector2(0, 1));
                int w2 = builder.AddVertex(new Vector3(1, 0, 1), new Vector3(0, 1, 0), new Vector2(1, 1));
                int w3 = builder.AddVertex(new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector2(1, 0));
                builder.AddQuad(w0, w1, w2, w3);
            }
        }
        
        builder.Build(mesh);
        
        /*
        mesh.Clear();
        mesh.vertices = new[]
        {
            //Floor
            new Vector3(0,0,0),
            new Vector3(0,0,1),
            new Vector3(1, 0, 1),
            new Vector3(1,0,0),
            
            //Wall 0374
            new Vector3(0,1,0),
            new Vector3(0,1,1),
            new Vector3(1, 1, 1),
            new Vector3(1,1,0)
        };
        
        mesh.normals = new[] {
            //Floor
            new Vector3(0, 1, 0), 
            new Vector3(0, 1, 0), 
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 0), 
            
            //Wall 0374
            new Vector3(1, 0, 0), 
            new Vector3(1, 0, 0),
            new Vector3(1,0,0),
            new Vector3(1,0,0)
            
            
        };
        
        mesh.triangles = new[]
        {
            //Floor
            0, 1, 2,
            0, 2, 3,
            
            //Walls
            0, 4, 5,
            0, 5, 1,
            
            
            
        };

        mesh.uv = new[]
        {
            //Floor
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 0),
            
            //Wall 0374
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(0, 0),
            new Vector2(1, 0),
        };
        mesh.MarkModified();*/
    }
}
