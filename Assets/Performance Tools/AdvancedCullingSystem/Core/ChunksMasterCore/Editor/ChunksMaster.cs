using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace AdvancedCullingSystem.ChunksMaster
{
    public class ChunksMaster
    {
        private int _uvChannelsCount = 8;

        private List<Mesh> _meshes;
        private List<List<Bounds>> _bounds;
        private List<List<Mesh>> _chunks;
        private List<int> _maxTriangles;

        public List<Mesh> meshes
        {
            get
            {
                return _meshes;
            }
        }
        public List<List<Bounds>> bounds
        {
            get
            {
                return _bounds;
            }
        }
        public List<int> maxTriangles
        {
            get
            {
                return _maxTriangles;
            }
        }

        private string _saveFolder;


        public ChunksMaster()
        {
            _meshes = new List<Mesh>();
            _bounds = new List<List<Bounds>>();
            _chunks = new List<List<Mesh>>();
            _maxTriangles = new List<int>();
        }

        public void CreateChunksBounds(Mesh mesh, int maxTriangles)
        {
            int index = _meshes.IndexOf(mesh);

            if (index >= 0)
            {
                if (_maxTriangles[index] == maxTriangles && _bounds[index].Count > 0)
                    return;

                _bounds[index].Clear();
                _maxTriangles[index] = maxTriangles;
            }
            else
            {
                _meshes.Add(mesh);
                _bounds.Add(new List<Bounds>());
                _chunks.Add(new List<Mesh>());
                _maxTriangles.Add(maxTriangles);
            }

            ProcessChunk(mesh, GetTriangles(mesh), mesh.bounds, maxTriangles, true, false);
        }

        public void CreateChunks(Mesh mesh, int maxTriangles, string saveFolder)
        {
            _saveFolder = saveFolder;

            int index = _meshes.IndexOf(mesh);

            if (index >= 0)
            {
                if (_maxTriangles[index] == maxTriangles && _chunks[index].Count > 0 && _chunks[0] != null)
                    return;

                _bounds[index].Clear();
                _chunks[index].Clear();

                _maxTriangles[index] = maxTriangles;
            }
            else
            {
                _meshes.Add(mesh);
                _bounds.Add(new List<Bounds>());
                _chunks.Add(new List<Mesh>());
                _maxTriangles.Add(maxTriangles);
            }

            if (!Directory.Exists(saveFolder))
                Directory.CreateDirectory(saveFolder);

            ProcessChunk(mesh, GetTriangles(mesh), mesh.bounds, maxTriangles, true, true);
        }

        public List<GameObject> SetupChunks(MeshFilter filter)
        {
            int index = _meshes.IndexOf(filter.sharedMesh);
            
            if (index < 0)
                return null;

            List<Mesh> chunks = _chunks[index];
            List<GameObject> chunksObjects = new List<GameObject>();

            for (int i = 0; i < chunks.Count; i++)
            {
                GameObject chunkObject = new GameObject(filter.gameObject.name + "_chunk");

                ComponentUtility.CopyComponent(filter);
                ComponentUtility.PasteComponentAsNew(chunkObject);

                ComponentUtility.CopyComponent(filter.GetComponent<MeshRenderer>());
                ComponentUtility.PasteComponentAsNew(chunkObject);

                chunkObject.transform.parent = filter.transform;

                chunkObject.transform.localPosition = Vector3.zero;
                chunkObject.transform.localRotation = Quaternion.identity;
                chunkObject.transform.localScale = Vector3.one;

                chunkObject.GetComponent<MeshFilter>().sharedMesh = chunks[i];
                chunkObject.AddComponent<ChunkMark>();

                chunkObject.isStatic = filter.gameObject.isStatic;

                chunksObjects.Add(chunkObject);
            }

            filter.GetComponent<MeshRenderer>().enabled = false;

            return chunksObjects;
        }

        public void Clear()
        {
            _meshes.Clear();
            _bounds.Clear();
            _chunks.Clear();
            _maxTriangles.Clear();
        }


        private void ProcessChunk(Mesh mesh, List<int>[] triangles, Bounds bounds, int maxTriangles, 
            bool createBounds, bool createChunks)
        {
            int count = 0;

            for (int i = 0; i < triangles.Length; i++)
                count += triangles[i].Count / 3;

            if (count < maxTriangles)
            {
                if(createBounds)
                    _bounds[_meshes.IndexOf(mesh)].Add(bounds);

                if (createChunks)
                    CreateAndSaveMesh(mesh, triangles);

                return;
            }

            Vector3 size = bounds.size;

            if (bounds.size.x >= bounds.size.y && bounds.size.x >= bounds.size.z)
                size.x /= 2;

            else if (bounds.size.y >= bounds.size.x && bounds.size.y >= bounds.size.z)
                size.y /= 2;

            else
                size.z /= 2;

            Bounds leftBounds = new Bounds(bounds.min + size / 2, size);
            Bounds rightBounds = new Bounds(bounds.max - size / 2, size);

            Vector3[] vertices = mesh.vertices;

            List<int>[] leftTriangles = new List<int>[triangles.Length];
            List<int>[] rightTriangles = new List<int>[triangles.Length];

            for (int i = 0; i < triangles.Length; i++)
            {
                leftTriangles[i] = new List<int>();
                rightTriangles[i] = new List<int>();

                for (int c = 0; c < triangles[i].Count; c += 3)
                {
                    Vector3 vec1 = vertices[triangles[i][c]];
                    Vector3 vec2 = vertices[triangles[i][c + 1]];
                    Vector3 vec3 = vertices[triangles[i][c + 2]];

                    if (leftBounds.Contains(vec1) || leftBounds.Contains(vec2) || leftBounds.Contains(vec3))
                    {
                        leftTriangles[i].Add(triangles[i][c]);
                        leftTriangles[i].Add(triangles[i][c + 1]);
                        leftTriangles[i].Add(triangles[i][c + 2]);
                    }
                    else
                    {
                        rightTriangles[i].Add(triangles[i][c]);
                        rightTriangles[i].Add(triangles[i][c + 1]);
                        rightTriangles[i].Add(triangles[i][c + 2]);
                    }
                }
            }

            ProcessChunk(mesh, leftTriangles, leftBounds, maxTriangles, createBounds, createChunks);
            ProcessChunk(mesh, rightTriangles, rightBounds, maxTriangles, createBounds, createChunks);
        }

        private List<int>[] GetTriangles(Mesh mesh)
        {
            int subMeshCount = mesh.subMeshCount;

            List<int>[] triangles = new List<int>[subMeshCount];

            for (int i = 0; i < subMeshCount; i++)
                triangles[i] = new List<int>(mesh.GetTriangles(i));

            return triangles;
        }

        private void GetUVs(Mesh mesh, out List<List<Vector2>> uvs, out List<int> uvChannels)
        {
            uvs = new List<List<Vector2>>();
            uvChannels = new List<int>();

            for (int i = 0; i < _uvChannelsCount; i++)
            {
                List<Vector2> uv = new List<Vector2>();

                mesh.GetUVs(i, uv);

                if (uv != null && uv.Count > 0)
                {
                    uvs.Add(uv);
                    uvChannels.Add(i);
                }
            }
        }

        private void CreateAndSaveMesh(Mesh src, List<int>[] triangles)
        {
            Mesh chunk = CreateChunkMesh(src, triangles);

            string path = _saveFolder + chunk.GetHashCode() + ".asset";

            AssetDatabase.CreateAsset(chunk, path);

            _chunks[_meshes.IndexOf(src)].Add(AssetDatabase.LoadAssetAtPath<Mesh>(path));
        }

        private Mesh CreateChunkMesh(Mesh src, List<int>[] triangles)
        {
            Mesh mesh = new Mesh();

            GetUVs(src, out List<List<Vector2>> srcUvs, out List<int> uvChannels);

            Vector3[] srcVertices = src.vertices;
            Vector4[] srcTangents = src.tangents;
            Vector3[] srcNormals = src.normals;
            Color[] srcColors = src.colors;
            Color32[] srcColors32 = src.colors32;

            List<Vector3> vertices = new List<Vector3>();
            List<Vector4> tangents = new List<Vector4>();
            List<Vector3> normals = new List<Vector3>();
            List<Color> colors = new List<Color>();
            List<Color32> colors32 = new List<Color32>();
            List<Vector2>[] uvs = new List<Vector2>[uvChannels.Count];
            List<int>[] meshTriangles = new List<int>[triangles.Length];

            for (int i = 0; i < uvs.Length; i++)
                uvs[i] = new List<Vector2>();

            Dictionary<int, int> srcToNewTriangles = new Dictionary<int, int>();

            for (int i = 0; i < triangles.Length; i++)
            {
                meshTriangles[i] = new List<int>();

                for (int c = 0; c < triangles[i].Count; c++)
                {
                    int srcTriangle = triangles[i][c];
                    int newTriangle = -1;

                    if (srcToNewTriangles.ContainsKey(srcTriangle))
                    {
                        newTriangle = srcToNewTriangles[srcTriangle];
                    }
                    else
                    {
                        newTriangle = vertices.Count;

                        vertices.Add(srcVertices[srcTriangle]);
                        tangents.Add(srcVertices[srcTriangle]);
                        normals.Add(srcVertices[srcTriangle]);

                        for (int j = 0; j < uvChannels.Count; j++)
                            uvs[j].Add(srcUvs[j][srcTriangle]);

                        if (srcColors.Length > 0)
                            colors.Add(srcColors[srcTriangle]);

                        if (srcColors32.Length > 0)
                            colors32.Add(srcColors32[srcTriangle]);

                        srcToNewTriangles.Add(srcTriangle, newTriangle);
                    }

                    meshTriangles[i].Add(newTriangle);
                }
            }
            
            mesh.vertices = vertices.ToArray();
            mesh.tangents = tangents.ToArray();
            mesh.normals = normals.ToArray();

            if (colors.Count > 0)
                mesh.SetColors(colors);

            if (colors32.Count > 0)
                mesh.SetColors(colors32);

            for (int i = 0; i < uvChannels.Count; i++)
                mesh.SetUVs(uvChannels[i], uvs[i]);

            mesh.subMeshCount = triangles.Length;

            for (int i = 0; i < triangles.Length; i++)
                mesh.SetTriangles(meshTriangles[i], i);

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            return mesh;
        }
    }


    public static class BoundsHelper
    {
        public static bool Intersects(this Bounds bounds, Vector3 t1, Vector3 t2, Vector3 t3)
        {
            float p0, p1, p2, r;

            Vector3 extents = bounds.max - bounds.center;

            Vector3 v0 = t1 - bounds.center,
                v1 = t2 - bounds.center,
                v2 = t3 - bounds.center;

            Vector3 f0 = v1 - v0,
                f1 = v2 - v1,
                f2 = v0 - v2;

            Vector3 a00 = new Vector3(0, -f0.z, f0.y),
                a01 = new Vector3(0, -f1.z, f1.y),
                a02 = new Vector3(0, -f2.z, f2.y),
                a10 = new Vector3(f0.z, 0, -f0.x),
                a11 = new Vector3(f1.z, 0, -f1.x),
                a12 = new Vector3(f2.z, 0, -f2.x),
                a20 = new Vector3(-f0.y, f0.x, 0),
                a21 = new Vector3(-f1.y, f1.x, 0),
                a22 = new Vector3(-f2.y, f2.x, 0);

            // Test axis a00
            p0 = Vector3.Dot(v0, a00);
            p1 = Vector3.Dot(v1, a00);
            p2 = Vector3.Dot(v2, a00);
            r = extents.y * Mathf.Abs(f0.z) + extents.z * Mathf.Abs(f0.y);

            if (Mathf.Max(-Mathf.Max(p0, p1, p2), Mathf.Min(p0, p1, p2)) > r)
                return false;

            // Test axis a01
            p0 = Vector3.Dot(v0, a01);
            p1 = Vector3.Dot(v1, a01);
            p2 = Vector3.Dot(v2, a01);
            r = extents.y * Mathf.Abs(f1.z) + extents.z * Mathf.Abs(f1.y);

            if (Mathf.Max(-Mathf.Max(p0, p1, p2), Mathf.Min(p0, p1, p2)) > r)
                return false;

            // Test axis a02
            p0 = Vector3.Dot(v0, a02);
            p1 = Vector3.Dot(v1, a02);
            p2 = Vector3.Dot(v2, a02);
            r = extents.y * Mathf.Abs(f2.z) + extents.z * Mathf.Abs(f2.y);

            if (Mathf.Max(-Mathf.Max(p0, p1, p2), Mathf.Min(p0, p1, p2)) > r)
                return false;

            // Test axis a10
            p0 = Vector3.Dot(v0, a10);
            p1 = Vector3.Dot(v1, a10);
            p2 = Vector3.Dot(v2, a10);
            r = extents.x * Mathf.Abs(f0.z) + extents.z * Mathf.Abs(f0.x);
            if (Mathf.Max(-Mathf.Max(p0, p1, p2), Mathf.Min(p0, p1, p2)) > r)
                return false;

            // Test axis a11
            p0 = Vector3.Dot(v0, a11);
            p1 = Vector3.Dot(v1, a11);
            p2 = Vector3.Dot(v2, a11);
            r = extents.x * Mathf.Abs(f1.z) + extents.z * Mathf.Abs(f1.x);

            if (Mathf.Max(-Mathf.Max(p0, p1, p2), Mathf.Min(p0, p1, p2)) > r)
                return false;

            // Test axis a12
            p0 = Vector3.Dot(v0, a12);
            p1 = Vector3.Dot(v1, a12);
            p2 = Vector3.Dot(v2, a12);
            r = extents.x * Mathf.Abs(f2.z) + extents.z * Mathf.Abs(f2.x);

            if (Mathf.Max(-Mathf.Max(p0, p1, p2), Mathf.Min(p0, p1, p2)) > r)
                return false;

            // Test axis a20
            p0 = Vector3.Dot(v0, a20);
            p1 = Vector3.Dot(v1, a20);
            p2 = Vector3.Dot(v2, a20);
            r = extents.x * Mathf.Abs(f0.y) + extents.y * Mathf.Abs(f0.x);

            if (Mathf.Max(-Mathf.Max(p0, p1, p2), Mathf.Min(p0, p1, p2)) > r)
                return false;

            // Test axis a21
            p0 = Vector3.Dot(v0, a21);
            p1 = Vector3.Dot(v1, a21);
            p2 = Vector3.Dot(v2, a21);
            r = extents.x * Mathf.Abs(f1.y) + extents.y * Mathf.Abs(f1.x);

            if (Mathf.Max(-Mathf.Max(p0, p1, p2), Mathf.Min(p0, p1, p2)) > r)
                return false;

            // Test axis a22
            p0 = Vector3.Dot(v0, a22);
            p1 = Vector3.Dot(v1, a22);
            p2 = Vector3.Dot(v2, a22);
            r = extents.x * Mathf.Abs(f2.y) + extents.y * Mathf.Abs(f2.x);

            if (Mathf.Max(-Mathf.Max(p0, p1, p2), Mathf.Min(p0, p1, p2)) > r)
                return false;

            if (Mathf.Max(v0.x, v1.x, v2.x) < -extents.x || Mathf.Min(v0.x, v1.x, v2.x) > extents.x)
                return false;

            if (Mathf.Max(v0.y, v1.y, v2.y) < -extents.y || Mathf.Min(v0.y, v1.y, v2.y) > extents.y)
                return false;

            if (Mathf.Max(v0.z, v1.z, v2.z) < -extents.z || Mathf.Min(v0.z, v1.z, v2.z) > extents.z)
                return false;

            var normal = Vector3.Cross(f1, f0).normalized;
            var pl = new Plane(normal, Vector3.Dot(normal, t1));

            return Intersects(bounds, pl);
        }

        public static bool Intersects(this Bounds bounds, Plane plane)
        {
            Vector3 center = bounds.center;
            var extents = bounds.max - center;

            var r = extents.x * Mathf.Abs(plane.normal.x) + extents.y * Mathf.Abs(plane.normal.y) + extents.z * Mathf.Abs(plane.normal.z);
            var s = Vector3.Dot(plane.normal, center) - plane.distance;

            return Mathf.Abs(s) <= r;
        }
    }
}
