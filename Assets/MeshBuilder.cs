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
            new Vector2(0, 1f), new Vector2(0.5f, 0.5f), //Gräs
            new Vector2(0.5f, 0.5f), new Vector2(1, 0), //Sten
            new Vector2(0.5f, 1), new Vector2(1f, 0.5f), //Vatten
            new Vector2(0f, 0.5f), new Vector2(0.5f, 0f) //Sand
        };

        public int AddVertex(Vector3 position, Vector3 normal, Vector2 uv)
        {
            int index = vertices.Count;
            vertices.Add(VertexMatrix.MultiplyPoint(position));
            normals.Add(VertexMatrix.MultiplyVector(normal));
            this.uv.Add(TextureMatrix.MultiplyPoint(uv));
            return index;
        }

        public void CreateTriangle(Vector3 hörn1, Vector3 hörn2, Vector3 hörn3, int state)
        {
            TranslateForTexture(state);
            
            Vector3 normal = GetNormal(hörn1, hörn2, hörn3);
            
            triangles.Add(AddVertex(hörn1, normal, new Vector2(1, 1)));
            triangles.Add(AddVertex(hörn2, normal, new Vector2(1, 0)));
            triangles.Add(AddVertex(hörn3, normal, new Vector2(0, 0)));
            
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
        public void CreateQuad(Vector3 botLeft, Vector3 topLeft, Vector3 topRight, Vector3 botRight, int state)
        {
            TranslateForTexture(state);
            
            Vector3 normal = GetNormal(botLeft, topLeft, topRight);

            int a = AddVertex(
                botLeft,
                normal,
            new Vector2(0, 0));
            //new Vector2(min.x, min.y));
            int b = AddVertex(
                topLeft,
                normal,
                new Vector2(0, 1));
            //new Vector2(min.x, max.y));
            int c = AddVertex(
                topRight,
                normal, 
                new Vector2(1, 1));
                //new Vector2(max.x, max.y));
            int d = AddVertex(
                botRight,
                normal,
                new Vector2(1, 0));
                //new Vector2(max.x, min.y));
            
            AddQuad(a, b, c, d);
        }

        //Tagen från https://docs.unity3d.com/ScriptReference/Vector3.Cross.html
        private Vector3 GetNormal(Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 side1 = b - a;
            Vector3 side2 = c - a;

            return Vector3.Cross(side1, side2).normalized;
        }

        private void TranslateForTexture(int stateInt)
        {
            TextureMatrix = Matrix4x4.Scale(new Vector3(0.5f, 0.5f, 1));
            Vector2 multWith;
            
            switch (stateInt)
            {
                case 0:
                    multWith = new Vector2(0, 1f);
                    break;
                case 1:
                    multWith = new Vector2(1f, 0);
                    break;
                case 2:
                    multWith = new Vector2(1f, 1f);
                    break;
                case 3:
                    multWith = new Vector2(0, 0);
                    break;
                default:
                    multWith = new Vector2(0, 1f);
                    break;
            }
            
            TextureMatrix *= Matrix4x4.Translate(multWith);
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