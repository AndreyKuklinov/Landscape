using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

using Directory = System.IO.Directory;

namespace AdvancedCullingSystem.ChunksMaster
{
    public class ChunksMasterEditorWindow : EditorWindow
    {
        public Vector2 size
        {
            get
            {
                return position.size;
            }
        }

        private ChunksMaster _chunksMaster;

        private string _dataSaveFolder;
        private int _maxTriangles = 10000;
        private bool _showChunks = true;
        private Vector2 _scrollPosition;
        private List<GameObject> _selectedObjects;


        [MenuItem("Tools/NGSTools/Advanced Culling System/Chunks Master")]
        private static void CreateWindow()
        {
            var window = GetWindow<ChunksMasterEditorWindow>(false, "Chunks Master", true);

            window.minSize = new Vector2(490, 205);

            window.Show();
        }

        private void OnEnable()
        {
            SceneView.onSceneGUIDelegate += OnSceneView;

            _selectedObjects = new List<GameObject>();
            _chunksMaster = new ChunksMaster();

            string scenePath = SceneManager.GetActiveScene().path;

            _dataSaveFolder = (scenePath == null || scenePath == "") ? "ACSData/" : 
                scenePath.Replace("Assets/", "").Replace(".unity", "_ACSData/");
        }


        private void OnGUI()
        {
            CheckData();

            GUILayout.BeginArea(new Rect(10, 10, size.x / 2 - 20, size.y - 70));

                GUILayout.BeginHorizontal();

                    GUILayout.Label("Data Save Folder :", GUILayout.Width(110));

                    _dataSaveFolder = GUILayout.TextField(_dataSaveFolder);

                GUILayout.EndHorizontal();

                _maxTriangles = Mathf.Max(EditorGUILayout.IntField("Max Triangles Per Chunk : ", _maxTriangles), 1000);

                GUILayout.Space(10);

                GUILayout.Label(_selectedObjects.Count > 0 ? "Selected Objects :" : "No objects selected");

                _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

                    for (int i = 0; i < _selectedObjects.Count; i++)
                        EditorGUILayout.ObjectField(_selectedObjects[i], typeof(GameObject), false);

                GUILayout.EndScrollView();

            GUILayout.EndArea();



            GUILayout.BeginArea(new Rect(size.x / 2 + 10, 10, size.x / 2 - 20, size.y - 70));

                if (GUILayout.Button("Add Static")) AddStatic();
                if (GUILayout.Button("Add Selected")) AddSelected();
                if (GUILayout.Button("Remove Selected")) RemoveSelected();
                if (GUILayout.Button("Remove All")) RemoveAll();

                _showChunks = EditorGUILayout.ToggleLeft("Show Created Chunks", _showChunks);

            GUILayout.EndArea();



            GUILayout.BeginArea(new Rect(10, size.y - 50, size.x - 20, 40));

                GUILayout.BeginHorizontal();

                    if (IsSceneContainsChunks())
                        if (GUILayout.Button("Delete Chunks", GUILayout.Width(100), GUILayout.Height(40)))
                            DeleteChunks();

                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Split", GUILayout.Width(70), GUILayout.Height(40))) Split();
                    if (GUILayout.Button("Apply", GUILayout.Width(70), GUILayout.Height(40))) Apply();

                GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }


        private void AddStatic()
        {
            int saveObjectsCount = _selectedObjects.Count;


            GameObject[] objects = FindObjectsOfType<Transform>()
                .Select(t => t.gameObject)
                .Where(go => go.isStatic)
                .ToArray();

            for (int i = 0; i < objects.Length; i++)
            {
                EditorUtility.DisplayProgressBar("Adding static objects", i + " of " + objects.Length, (float)i / objects.Length);

                if (IsValidGameObject(objects[i]))
                    AddGameObject(objects[i]);
            }

            EditorUtility.ClearProgressBar();


            if ((_selectedObjects.Count - saveObjectsCount) == 0)
                Debug.Log("No new static objects added");

            else
                Debug.Log("Added " + (_selectedObjects.Count - saveObjectsCount) + " objects");
        }

        private void AddSelected()
        {
            int saveObjectsCount = _selectedObjects.Count;



            GameObject[] selected = Selection.gameObjects;

            if (selected == null || selected.Length == 0)
            {
                Debug.Log("No objects selected");
                return;
            }

            for (int i = 0; i < selected.Length; i++)
            {
                EditorUtility.DisplayProgressBar("Adding selected objects", i + " of " + selected.Length, (float)i / selected.Length);

                foreach (var go in selected[i].GetComponentsInChildren<Transform>().Select(t => t.gameObject))
                    if (IsValidGameObject(go))
                        AddGameObject(go);
            }

            EditorUtility.ClearProgressBar();


            if ((_selectedObjects.Count - saveObjectsCount) == 0)
                Debug.Log("No new objects added");

            else
                Debug.Log("Added " + (_selectedObjects.Count - saveObjectsCount) + " objects");
        }

        private void RemoveSelected()
        {
            int saveObjectsCount = _selectedObjects.Count;


            GameObject[] selected = Selection.gameObjects;

            if (selected == null || selected.Length == 0)
            {
                Debug.Log("No objects selected");
                return;
            }

            for (int i = 0; i < selected.Length; i++)
            {
                EditorUtility.DisplayProgressBar("Removing selected objects", i + " of " + selected.Length, (float)i / selected.Length);

                foreach (var go in selected[i].GetComponentsInChildren<Transform>().Select(t => t.gameObject))
                    if (_selectedObjects.Contains(go))
                        _selectedObjects.Remove(go);
            }

            EditorUtility.ClearProgressBar();


            if ((saveObjectsCount - _selectedObjects.Count) == 0)
                Debug.Log("No objects removed");

            else
                Debug.Log("Removed " + (saveObjectsCount - _selectedObjects.Count) + " objects");
        }

        private void RemoveAll()
        {
            _selectedObjects.Clear();
            
            Debug.Log("Removed all objects");
        }

        private void Apply()
        {
            Mesh[] meshes = _selectedObjects.Select(go => go.GetComponent<MeshFilter>().sharedMesh).ToArray();

            for (int i = 0; i < meshes.Length; i++)
            {
                try
                {
                    EditorUtility.DisplayProgressBar("Applying...", i + " of " + meshes.Length, (float)i / meshes.Length);

                    _chunksMaster.CreateChunksBounds(meshes[i], _maxTriangles);
                }
                catch (System.Exception ex)
                {
                    Debug.Log("Can't compute mesh " + meshes[i].name);
                    Debug.Log("Cause : " + ex.Message + " " + ex.StackTrace);
                    Debug.Log("Please write about it on e-mail(andre-orsk@yandex.ru) and I will help You");
                    Debug.Log("-----------------------------------");
                }
            }

            EditorUtility.ClearProgressBar();
        }

        private void Split()
        {
            int count = _selectedObjects.Count;

            for (int i = 0; i < count; i++)
            {
                try
                {
                    EditorUtility.DisplayProgressBar("Splitting...", i + " of " + count, (float)i / count);

                    MeshFilter filter = _selectedObjects[0].GetComponent<MeshFilter>();

                    _chunksMaster.CreateChunks(filter.sharedMesh, _maxTriangles, 
                        "Assets/" + _dataSaveFolder + filter.gameObject.name + "/");

                    _chunksMaster.SetupChunks(filter);
                }
                catch (System.Exception ex)
                {
                    Debug.Log("Can't split mesh " + _selectedObjects[0].name);
                    Debug.Log("Cause : " + ex.Message + " " + ex.StackTrace);
                    Debug.Log("Please write about it on e-mail(andre-orsk@yandex.ru) and I will help You");
                    Debug.Log("-----------------------------------");
                }

                _selectedObjects.RemoveAt(0);
            }

            EditorUtility.ClearProgressBar();
        }

        private void DeleteChunks()
        {
            ChunkMark[] chunks = FindObjectsOfType<ChunkMark>();
            for(int i = 0; i < chunks.Length; i++)
            {
                try
                {
                    EditorUtility.DisplayProgressBar("Deleting...", i + " of " + chunks.Length, (float)i / chunks.Length);

                    Mesh mesh = chunks[i].GetComponent<MeshFilter>().sharedMesh;

                    if (mesh != null)
                    {
                        string path = AssetDatabase.GetAssetPath(mesh);
                        string parentFolder = path.Remove(path.LastIndexOf("/"), path.Length - path.LastIndexOf("/"));

                        if (path != null && path != "")
                            AssetDatabase.DeleteAsset(path);

                        if (Directory.GetFiles(parentFolder).Length == 0)
                            Directory.Delete(parentFolder, false);
                    }

                    chunks[i].transform.parent.GetComponent<MeshRenderer>().enabled = true;

                    DestroyImmediate(chunks[i].gameObject);
                }
                catch (System.Exception ex)
                {
                    Debug.Log("Can't remove chunk " + chunks[i].name);
                    Debug.Log("Cause : " + ex.Message + " " + ex.StackTrace);
                    Debug.Log("Please write about it on e-mail(andre-orsk@yandex.ru) and I will help You");
                    Debug.Log("-----------------------------------");
                }
            }

            AssetDatabase.Refresh();

            EditorUtility.ClearProgressBar();
        }


        private void CheckData()
        {
            if (!_dataSaveFolder.EndsWith("/"))
                _dataSaveFolder += "/";

            _selectedObjects = _selectedObjects.Where(obj => obj != null && IsValidGameObject(obj)).ToList();
        }

        private bool IsSceneContainsChunks()
        {
            return FindObjectsOfType<ChunkMark>().Length > 0;
        }

        private bool IsValidGameObject(GameObject go)
        {
            try
            {
                if (go.GetComponent<MeshRenderer>() == null)
                    return false;

                MeshFilter filter = go.GetComponent<MeshFilter>();

                if (filter == null || filter.sharedMesh == null)
                    return false;

                int subMeshCount = filter.sharedMesh.subMeshCount;

                for (int i = 0; i < subMeshCount; i++)
                    if ((filter.sharedMesh.GetTriangles(i).Length / 3) >= _maxTriangles)
                        return true;

                return false;
            }
            catch (System.Exception ex)
            {
                Debug.Log("Can't process " + go.name);
                Debug.Log("Cause : " + ex.Message + " " + ex.StackTrace);
                Debug.Log("Please write about it on e-mail(andre-orsk@yandex.ru) and I will help You");
                Debug.Log("-----------------------------------");

                return false;
            }
        }

        private void AddGameObject(GameObject go)
        {
            try
            {
                if (!_selectedObjects.Contains(go))
                {
                    Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;

                    _chunksMaster.CreateChunksBounds(mesh, _maxTriangles);

                    _selectedObjects.Add(go);
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log("Can't add " + go.name);
                Debug.Log("Cause : " + ex.Message + " " + ex.StackTrace);
                Debug.Log("Please write about it on e-mail(andre-orsk@yandex.ru) and I will help You");
                Debug.Log("-----------------------------------");
            }
        }


        private void OnSceneView(SceneView sceneView)
        {
            if (!_showChunks)
                return;

            CheckData();

            Handles.color = Color.blue;

            foreach (var go in _selectedObjects)
            {
                Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;

                int index = _chunksMaster.meshes.IndexOf(mesh);

                Handles.matrix = Matrix4x4.TRS(go.transform.position, go.transform.rotation, go.transform.lossyScale);

                foreach (var bounds in _chunksMaster.bounds[index])
                    Handles.DrawWireCube(bounds.center, bounds.size);
            }

            Handles.matrix = Matrix4x4.identity;
        }

        private void OnDestroy()
        {
            SceneView.onSceneGUIDelegate -= OnSceneView;
        }
    }
}


