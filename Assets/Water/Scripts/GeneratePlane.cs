    using System.IO;
    using UnityEditor;
    using UnityEngine;

    namespace Water.Scripts
    {
        public class GeneratePlane : ScriptableWizard
        {
            [SerializeField] private string objectName = "Ocean";

            [SerializeField] private int widthSegments = 254;
            [SerializeField] private int heightSegments = 254;
            [SerializeField] private float planeWidth = 1.0f;
            [SerializeField] private float planeHeight = 1.0f;

            [SerializeField] private bool addCollider;
            [SerializeField] private Material material;
            
            private const string AssetSaveLocation = "Assets/Water/Plane Meshes/";

            [MenuItem("GameObject/LowPoly Water/Generate Water Plane...")]
            private static void CreateWizard()
            {
                if (!Directory.Exists(AssetSaveLocation))
                {
                    Directory.CreateDirectory(AssetSaveLocation);
                }

                DisplayWizard("Generate Water Plane", typeof(GeneratePlane));
            }

            private void OnWizardUpdate()
            {
                widthSegments = Mathf.Clamp(widthSegments, 1, 254);
                heightSegments = Mathf.Clamp(heightSegments, 1, 254);
            }

            private void OnWizardCreate()
            {
                var plane = new GameObject
                {
                    name = string.IsNullOrEmpty(objectName) ? "Plane" : objectName
                };

                var meshFilter = plane.AddComponent(typeof(MeshFilter)) as MeshFilter;
                var meshRenderer = plane.AddComponent((typeof(MeshRenderer))) as MeshRenderer;
                meshRenderer.sharedMaterial = material;

                var planeMeshAssetName = plane.name + widthSegments + "x" + heightSegments
                                         + "W" + planeWidth + "H" + planeHeight + ".asset";

                var m = (Mesh) AssetDatabase.LoadAssetAtPath(AssetSaveLocation + planeMeshAssetName, typeof(Mesh));

                if (m == null)
                {
                    m = new Mesh
                    {
                        name = plane.name
                    };

                    var hCount2 = widthSegments + 1;
                    var vCount2 = heightSegments + 1;
                    var numTriangles = widthSegments * heightSegments * 6;
                    var numVertices = hCount2 * vCount2;

                    var vertices = new Vector3[numVertices];
                    var uvs = new Vector2[numVertices];
                    var triangles = new int[numTriangles];
                    var tangents = new Vector4[numVertices];
                    var tangent = new Vector4(1f, 0f, 0f, -1f);
                    var anchorOffset = Vector2.zero;

                    var index = 0;
                    var uvFactorX = 1.0f / widthSegments;
                    var uvFactorY = 1.0f / heightSegments;
                    var scaleX = planeWidth / widthSegments;
                    var scaleY = planeHeight / heightSegments;

                    for (var y = 0.0f; y < vCount2; y++)
                    {
                        for (var x = 0.0f; x < hCount2; x++)
                        {
                            vertices[index] = new Vector3(x * scaleX - planeWidth / 2f - anchorOffset.x, 0.0f,
                                y * scaleY - planeHeight / 2f - anchorOffset.y);

                            tangents[index] = tangent;
                            uvs[index++] = new Vector2(x * uvFactorX, y * uvFactorY);
                        }
                    }

                    index = 0;
                    for (var y = 0; y < heightSegments; y++)
                    {
                        for (var x = 0; x < widthSegments; x++)
                        {
                            triangles[index] = (y * hCount2) + x;
                            triangles[index + 1] = ((y + 1) * hCount2) + x;
                            triangles[index + 2] = (y * hCount2) + x + 1;

                            triangles[index + 3] = ((y + 1) * hCount2) + x;
                            triangles[index + 4] = ((y + 1) * hCount2) + x + 1;
                            triangles[index + 5] = (y * hCount2) + x + 1;
                            index += 6;
                        }
                    }

                    m.vertices = vertices;
                    m.uv = uvs;
                    m.triangles = triangles;
                    m.tangents = tangents;
                    m.RecalculateNormals();

                    AssetDatabase.CreateAsset(m, AssetSaveLocation + planeMeshAssetName);
                    AssetDatabase.SaveAssets();
                }

                //Update mesh
                meshFilter.sharedMesh = m;
                m.RecalculateBounds();

                if (addCollider)
                    plane.AddComponent(typeof(BoxCollider));

                plane.AddComponent<Water.Scripts.LowPolyWater>();

                Selection.activeObject = plane;
            }
        }
    }