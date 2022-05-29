using System.IO;
using UnityEditor;
using UnityEngine;

namespace LowPolyWater_Pack.Scripts.Editor
{
    public class GeneratePlane : ScriptableWizard
    {
        public string objectName; //Optional name that can given to created plane gameobject

        public int widthSegments = 1; //Number of pieces for dividing plane vertically
        public int heightSegments = 1; //Number of pieces for dividing plane horizontally
        public float planeWidth = 1.0f;
        public float planeHeight = 1.0f;

        public bool addCollider = false; //Add box collider?
        public Material material; //By default, it is assigned to 'LowPolyWaterMaterial' in the editor

        private static Camera Cam;
        private static Camera LastUsedCam;

        //Generated plane meshes are saved and loaded from Plane Meshes folder (you can change it to whatever you want)
        private const string AssetSaveLocation = "Assets/Low Poly Water/Plane Meshes/";

        [MenuItem("GameObject/LowPoly Water/Generate Water Plane...")]
        private static void CreateWizard()
        {
            Cam = Camera.current;
            // Hack because camera.current doesn't return editor camera if scene view doesn't have focus
            if (!Cam)
                Cam = LastUsedCam;
            else
                LastUsedCam = Cam;

            //Check if the asset save location folder exists
            //If the folder doesn't exists, create it
            if (!Directory.Exists(AssetSaveLocation))
            {
                Directory.CreateDirectory(AssetSaveLocation);
            }

            //Open Wizard
            DisplayWizard("Generate Water Plane", typeof(GeneratePlane));
        }

        private void OnWizardUpdate()
        {
            //Max segment number is 254, because a mesh can't have more 
            //than 65000 vertices (254^2 = 64516 max. number of vertices)
            widthSegments = Mathf.Clamp(widthSegments, 1, 254);
            heightSegments = Mathf.Clamp(heightSegments, 1, 254);
        }

        private void OnWizardCreate()
        {
            //Create an empty gamobject
            var plane = new GameObject
            {
                //If user hasn't assigned a name, by default object name is 'Plane'
                name = string.IsNullOrEmpty(objectName) ? "Plane" : objectName
            };

            //Create Mesh Filter and Mesh Renderer components
            var meshFilter = plane.AddComponent(typeof(MeshFilter)) as MeshFilter;
            var meshRenderer = plane.AddComponent((typeof(MeshRenderer))) as MeshRenderer;
            meshRenderer.sharedMaterial = material;

            //Generate a name for the mesh that will be created
            var planeMeshAssetName = plane.name + widthSegments + "x" + heightSegments
                                     + "W" + planeWidth + "H" + planeHeight + ".asset";

            //Load the mesh from the save location
            var m = (Mesh) AssetDatabase.LoadAssetAtPath(AssetSaveLocation + planeMeshAssetName, typeof(Mesh));

            //If there isn't a mesh located under assets, create the mesh
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

                //Generate the vertices
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

                //Reset the index and generate triangles
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

                //Update the mesh properties (vertices, UVs, triangles, normals etc.)
                m.vertices = vertices;
                m.uv = uvs;
                m.triangles = triangles;
                m.tangents = tangents;
                m.RecalculateNormals();

                //Save the newly created mesh under save location to reload later
                AssetDatabase.CreateAsset(m, AssetSaveLocation + planeMeshAssetName);
                AssetDatabase.SaveAssets();
            }

            //Update mesh
            meshFilter.sharedMesh = m;
            m.RecalculateBounds();

            //If add collider is set to true, add a box collider
            if (addCollider)
                plane.AddComponent(typeof(BoxCollider));

            //Add LowPolyWater as component
            plane.AddComponent<Water.Scripts.LowPolyWater>();

            Selection.activeObject = plane;
        }
    }
}