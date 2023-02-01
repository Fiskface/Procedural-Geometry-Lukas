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

        public void CreateTriangle(Vector3 corner1, Vector3 corner2, Vector3 corner3, int state, int rotation)
        {
            TranslateForTexture(state, rotation);
            
            Vector3 normal = GetNormal(corner1, corner2, corner3);
            
            //triangles.Add(AddVertex(hörn1, normal, new Vector2(1, 1)));
            //triangles.Add(AddVertex(hörn2, normal, new Vector2(1, 0)));
            //triangles.Add(AddVertex(hörn3, normal, new Vector2(0, 0)));
            
            triangles.Add(AddVertex(corner1, normal, new Vector2(corner1.x + 0.5f, corner1.z + 0.5f)));
            triangles.Add(AddVertex(corner2, normal, new Vector2(corner2.x + 0.5f, corner2.z + 0.5f)));
            triangles.Add(AddVertex(corner3, normal, new Vector2(corner3.x + 0.5f, corner3.z + 0.5f)));
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
        public void CreateQuad(Vector3 botLeft, Vector3 topLeft, Vector3 topRight, Vector3 botRight, int state, int rotation)
        {
            TranslateForTexture(state, rotation);
            
            Vector3 normal = GetNormal(botLeft, topLeft, topRight);

            int a = AddVertex(
                botLeft,
                normal,
                new Vector2(botLeft.x + 0.5f, botLeft.z + 0.5f));
            int b = AddVertex(
                topLeft,
                normal,
                new Vector2(topLeft.x + 0.5f, topLeft.z + 0.5f));
            int c = AddVertex(
                topRight,
                normal,
                new Vector2(topRight.x + 0.5f, topRight.z + 0.5f));
            int d = AddVertex(
                botRight,
                normal,
                new Vector2(botRight.x + 0.5f, botRight.z + 0.5f));
            
            AddQuad(a, b, c, d);
        }
        
        public void CreateQuad(Vector3 botLeft, Vector3 topLeft, Vector3 topRight, Vector3 botRight, int state, int rotation,
            Vector2 bLUV, Vector2 tLUV, Vector2 tRUV, Vector2 bRUV)
        {
            TranslateForTexture(state, rotation);
            
            Vector3 normal = GetNormal(botLeft, topLeft, topRight);

            int a = AddVertex(
                botLeft,
                normal,
                new Vector2(bLUV.x + 0.5f, bLUV.y + 0.5f));
            int b = AddVertex(
                topLeft,
                normal,
                new Vector2(tLUV.x + 0.5f, tLUV.y + 0.5f));
            int c = AddVertex(
                topRight,
                normal,
                new Vector2(tRUV.x + 0.5f, tRUV.y + 0.5f));
            int d = AddVertex(
                botRight,
                normal,
                new Vector2(bRUV.x + 0.5f, bRUV.y + 0.5f));
            
            AddQuad(a, b, c, d);
        }
        
        private Vector3 GetNormal(Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 side1 = b - a;
            Vector3 side2 = c - a;

            return Vector3.Cross(side1, side2).normalized;
        }

        private void TranslateForTexture(int stateInt, int rotation)
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
            
            TextureMatrix *= Matrix4x4.Translate(multWith) *
                             Matrix4x4.Translate(new Vector3(-0.5f, -0.5f)) *
                             Matrix4x4.Rotate(Quaternion.AngleAxis(90 + 90 * rotation, Vector3.forward)) * 
                             Matrix4x4.Translate(new Vector3(0.5f, 0.5f));
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