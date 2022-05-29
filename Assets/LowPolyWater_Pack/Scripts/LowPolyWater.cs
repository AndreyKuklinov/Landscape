using UnityEngine;

namespace Water.Scripts
{
    public class LowPolyWater : MonoBehaviour
    {
        [SerializeField] private float waveHeight = 0.5f;
        [SerializeField] private float waveFrequency = 0.5f;
        [SerializeField] private float waveLength = 0.75f;

        public Vector3 waveOriginPosition = new Vector3(0.0f, 0.0f, 0.0f);

        private MeshFilter meshFilter;
        private Mesh mesh;
        private Vector3[] vertices;

        private void Awake() =>
            meshFilter = GetComponent<MeshFilter>();

        private void Start() =>
            CreateMeshLowPoly(meshFilter);

        private void CreateMeshLowPoly(MeshFilter mf)
        {
            mesh = mf.sharedMesh;

            var originalVertices = mesh.vertices;

            var triangles = mesh.triangles;

            var meshVertices = new Vector3[triangles.Length];

            for (var i = 0; i < triangles.Length; i++)
            {
                meshVertices[i] = originalVertices[triangles[i]];
                triangles[i] = i;
            }

            mesh.vertices = meshVertices;
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            vertices = mesh.vertices;
        }

        private void Update() =>
            GenerateWaves();

        private void GenerateWaves()
        {
            for (var i = 0; i < vertices.Length; i++)
            {
                var v = vertices[i];

                v.y = 0.0f;

                var distance = Vector3.Distance(v, waveOriginPosition);

                distance = (distance % waveLength) / waveLength;

                v.y = waveHeight * Mathf.Sin(Time.time * Mathf.PI * 2.0f * waveFrequency
                                             + (Mathf.PI * 2.0f * distance));

                vertices[i] = v;
            }

            mesh.vertices = vertices;
            mesh.RecalculateNormals();
            mesh.MarkDynamic();
            meshFilter.mesh = mesh;
        }
    }
}