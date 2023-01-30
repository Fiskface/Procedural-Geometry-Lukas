using System.Collections.Generic;
using UnityEngine;

namespace Scenes
{
    public class MeshBuilder
    {
        private readonly List<Vector3> vertices = new List<Vector3>();
        private readonly List<Vector3> normals = new List<Vector3>();
        private readonly List<Vector2> uv = new List<Vector2>();
        private readonly List<int> triangles = new List<int>();
        
        public Matrix4x4 VertexMatrix = Matrix4x4.identity;
        public Matrix4x4 TextureMatrix = Matrix4x4.identity;
        
        private Vector2[] stateTextureUV =
        {
            new Vector2(0, 1), new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), new Vector2(1, 0),
            new Vector2(0.5f, 1), new Vector2(1, 0.5f),
            new Vector2(0, 0.5f), new Vector2(0.5f, 0)
        };

        public int AddVertex(Vector3 position, Vector3 normal, Vector2 uv)
        {
            int index = vertices.Count;
            vertices.Add(VertexMatrix.MultiplyPoint(position));
            normals.Add(VertexMatrix.MultiplyVector(normal));
            this.uv.Add(TextureMatrix.MultiplyPoint(uv));
            return index;
        }

        public void AddQuad(int bottomLeft, int topLeft, int topRight, int bottomRight)
        {
            //First triangle
            triangles.Add(bottomLeft);
            triangles.Add(topLeft);
            triangles.Add(topRight);
            
            //Second triangle
            triangles.Add(bottomLeft);
            triangles.Add(topRight);
            triangles.Add(bottomRight);
        }
        
        //add 2 vectors for minimum and maximum uv
        public void CreateQuad(Vector3 botLeft, Vector3 topLeft, Vector3 topRight, Vector3 botRight, Vector3 normal, int state)
        {
            Vector2 min = stateTextureUV[state * 2];
            Vector2 max = stateTextureUV[state * 2 + 1];


                int a = AddVertex(
                botLeft, 
                normal, 
                new Vector2(min.x, min.y));
            int b = AddVertex(
                topLeft, 
                normal, 
                new Vector2(min.x, max.y));
            int c = AddVertex(
                topRight, 
                normal, 
                new Vector2(max.x, max.y));
            int d = AddVertex(
                botRight, 
                normal, 
                new Vector2(max.x, min.y));
            
            AddQuad(a, b, c, d);
        }

        public void Build(Mesh mesh)
        {
            mesh.Clear();
            mesh.SetVertices(vertices);
            mesh.SetNormals(normals);
            mesh.SetUVs(0, uv);
            mesh.SetIndices(triangles, MeshTopology.Triangles, 0);
            mesh.MarkModified();
        }
    }
}