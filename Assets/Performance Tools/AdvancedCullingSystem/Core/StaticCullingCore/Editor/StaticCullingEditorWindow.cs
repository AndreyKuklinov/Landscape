using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace AdvancedCullingSystem.StaticCullingCore
{
    public enum AreasPlacing { Around_Static, Placing_By_User }

    public class StaticCullingEditorWindow : EditorWindow
    {
        public Vector2 size
        {
            get
            {
                return position.size;
            }
        }

        private int _selectedTool;
        private string[] _toolNames;
        private Action[] _toolGUIFuncs;
        private Vector2[] _scrolls;

        private List<Camera> _cameras = new List<Camera>();

        private List<MeshRenderer> _objects = new List<MeshRenderer>();
        private List<Collider> _occluders = new List<Collider>();
        private bool _showSelectedObjects = true;

        private List<Transform> _areasTransforms = new List<Transform>();
        private List<Bounds> _areasBounds = new List<Bounds>();
        private AreasPlacing _areasPlacing;
        private float _cellSize = 3f;
        private int _jobsPerObject = 20;
        private bool _fastBake;
        private bool _showCells = false;

        private int _castersCount;
        private float _bakingTime;

        private GUIStyle _smallTextStyle;



        [MenuItem("Tools/NGSTools/Advanced Culling System/Static Culling")]
        private static void CreateWindow()
        {
            var window = GetWindow<StaticCullingEditorWindow>(false, "Static Culling", true);

            window.minSize = new Vector2(270, window.minSize.y);             

            window.Show();
        }

        private void OnEnable()
        {
            _selectedTool = 0;

            _toolNames = new string[] { "Cameras", "Objects", "Occluders", "Areas" };
            _toolGUIFuncs = new Action[] { CamerasToolGUI, ObjectsToolGUI, OccludersToolGUI, AreasToolGUI };
            _scrolls = new Vector2[_toolNames.Length];

            _castersCount = 0;
            _bakingTime = 0;

            _smallTextStyle = new GUIStyle()
            {
                fontSize = 9,
                padding = new RectOffset(5, 5, 0, 1),
            };

            SceneView.onSceneGUIDelegate += OnSceneGUI;

            LayersHelper.CreateLayer(ACSInfo.CullingLayerName);
        }


        private void CamerasToolGUI()
        {

            GUILayout.BeginHorizontal();

                GUILayout.Space(10);
                EditorGUILayout.HelpBox("You can assign cameras now or make it later after baking", MessageType.Info);
                GUILayout.Space(10);

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();

                GUILayout.Space(10);
                GUILayout.Label("Cameras :");

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();

                GUILayout.Space(10);

                GUILayout.BeginVertical();

                    if (_cameras.Count > 0)
                    {
                        _scrolls[_selectedTool] = GUILayout.BeginScrollView(_scrolls[_selectedTool]);

                            for (int i = 0; i < _cameras.Count; i++)
                                EditorGUILayout.ObjectField(_cameras[i], typeof(Camera), false);

                        GUILayout.EndScrollView();
                    }
                    else
                        EditorGUILayout.HelpBox("Cameras non assigned", MessageType.Warning);

                GUILayout.EndVertical();

                GUILayout.FlexibleSpace();

                GUILayout.BeginVertical();

                    if (GUILayout.Button("Add All Cameras")) AddAllCameras();
                    if (GUILayout.Button("Add Selected")) AddSelectedCameras();
                    if (GUILayout.Button("Remove Selected")) RemoveSelectedCameras();
                    if (GUILayout.Button("Remove All")) RemoveAllCameras();

                GUILayout.EndVertical();

                GUILayout.Space(10);

            GUILayout.EndHorizontal();
        }

        private void ObjectsToolGUI()
        {
            GUILayout.BeginHorizontal();

                GUILayout.Space(10);
                EditorGUILayout.HelpBox("Assign static objects that are needed to be culled", MessageType.Info);
                GUILayout.Space(10);

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();

                GUILayout.Space(10);
                GUILayout.Label("Objects :");

            GUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();

                GUILayout.Space(10);

                EditorGUILayout.BeginVertical();
         
                    if (_objects.Count > 0)
                    {
                        _scrolls[_selectedTool] = GUILayout.BeginScrollView(_scrolls[_selectedTool]);

                        for (int i = 0; i < _objects.Count; i++)
                            EditorGUILayout.ObjectField(_objects[i], typeof(GameObject), false);

                        GUILayout.EndScrollView();

                        GUILayout.Space(10);
                    }
                    else
                        EditorGUILayout.HelpBox("Objects non assigned", MessageType.Warning);

                EditorGUILayout.EndVertical();


                GUILayout.FlexibleSpace();


                EditorGUILayout.BeginVertical();

                    if (GUILayout.Button("Add All Static")) AddStaticObjects();
                    if (GUILayout.Button("Add Selected")) AddSelectedObjects();
                    if (GUILayout.Button("Remove Selected")) RemoveSelectedObjects();
                    if (GUILayout.Button("Remove All")) RemoveAllObjects();
                    
                    _showSelectedObjects = GUILayout.Toggle(_showSelectedObjects, "Show Selected");

                EditorGUILayout.EndVertical();

                GUILayout.Space(10);

            EditorGUILayout.EndHorizontal();
        }

        private void OccludersToolGUI()
        {
            GUILayout.BeginHorizontal();

                GUILayout.Space(10);
                EditorGUILayout.HelpBox("Here You can add colliders thats only occlude other objects. Unity Terrain for example", MessageType.Info);
                GUILayout.Space(10);

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();

                GUILayout.Space(10);
                GUILayout.Label("Occluders :");

            GUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();

                GUILayout.Space(10);

                EditorGUILayout.BeginVertical();
            
                    if (_occluders.Count > 0)
                    {
                        _scrolls[_selectedTool] = GUILayout.BeginScrollView(_scrolls[_selectedTool]);

                            for (int i = 0; i < _occluders.Count; i++)
                                EditorGUILayout.ObjectField(_occluders[i], typeof(Collider), false);

                        GUILayout.EndScrollView();

                        GUILayout.Space(10);
                    }
                    else
                        EditorGUILayout.HelpBox("Occluders non assigned", MessageType.Warning);

                EditorGUILayout.EndVertical();


                GUILayout.FlexibleSpace();


                EditorGUILayout.BeginVertical();

                    if (GUILayout.Button("Add Selected")) AddSelectedOccluders();
                    if (GUILayout.Button("Remove Selected")) RemoveSelectedOccluders();
                    if (GUILayout.Button("Remove All")) RemoveAllOccluders();

                EditorGUILayout.EndVertical();

                GUILayout.Space(10);

            EditorGUILayout.EndHorizontal();
        }

        private void AreasToolGUI()
        {
            GUILayout.BeginHorizontal();

                GUILayout.Space(10);

                EditorGUILayout.HelpBox("Set up areas where cameras may be located", MessageType.Info);

                GUILayout.Space(10);

            GUILayout.EndHorizontal();



            GUILayout.BeginHorizontal();

                GUILayout.Space(10);

                GUILayout.BeginVertical();

                    _areasPlacing = (AreasPlacing)EditorGUILayout.EnumPopup("Areas Placing :", _areasPlacing);

                    if (_areasPlacing == AreasPlacing.Placing_By_User)
                    {
                        GUILayout.Label("Areas :");

                        _scrolls[_selectedTool] = EditorGUILayout.BeginScrollView(_scrolls[_selectedTool]);

                            for (int i = 0; i < _areasTransforms.Count; i++)
                            {
                                EditorGUILayout.BeginHorizontal();

                                    EditorGUILayout.ObjectField(_areasTransforms[i], typeof(Transform), false);

                                    if (GUILayout.Button("-")) RemoveCullingArea(i);

                                EditorGUILayout.EndHorizontal();
                            }

                        EditorGUILayout.EndScrollView();

                        GUILayout.BeginHorizontal();

                            if (GUILayout.Button("Add")) AddCullingArea();

                            GUILayout.FlexibleSpace();

                            if (_areasTransforms.Count > 0)
                                if (GUILayout.Button("Clear"))
                                    RemoveAllCullingAreas();

                        GUILayout.EndHorizontal();
                        GUILayout.Space(20);
                    }

                    EditorGUILayout.BeginHorizontal();

                            _cellSize = Mathf.Max(EditorGUILayout.DelayedFloatField("Cell Size", _cellSize), 0.5f);
                            GUILayout.FlexibleSpace();

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();

                        _jobsPerObject = Mathf.Max(EditorGUILayout.DelayedIntField("Jobs Count Per Object", _jobsPerObject), 1);
                        GUILayout.FlexibleSpace();

                    EditorGUILayout.EndHorizontal();

                    _fastBake = EditorGUILayout.Toggle("Fast Bake", _fastBake);
                    _showCells = EditorGUILayout.Toggle("Show Cells", _showCells);

                    GUILayout.Space(10);
;
                    GUILayout.Label("Casters count : " + _castersCount, _smallTextStyle);

                    GUILayout.BeginHorizontal();

                            GUILayout.BeginVertical();

                                GUILayout.Label("Baking time : ~" + _bakingTime + " min." + "(Experimental)", _smallTextStyle);
                                if (GUILayout.Button("Calculate time")) CalculateBakingTime();

                            GUILayout.EndVertical();

                            GUILayout.FlexibleSpace();

                    GUILayout.EndHorizontal();

                GUILayout.EndVertical();

                GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();
        }

        private void OnGUI()
        {
            UpdateAssignedData();

            _selectedTool = GUILayout.Toolbar(_selectedTool, _toolNames);
            _toolGUIFuncs[_selectedTool].Invoke();

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();

                GUILayout.Space(10);

                if (IsDataBaked())
                    if (GUILayout.Button("Clear Data", GUILayout.Height(30), GUILayout.Width(80))) Clear();

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Bake", GUILayout.Height(30), GUILayout.Width(80))) Bake();

                GUILayout.Space(10);

            GUILayout.EndHorizontal();

            GUILayout.Space(10);
        }



        private void AddAllCameras()
        {
            Camera[] cameras = FindObjectsOfType<Camera>();

            if (cameras.Length == 0)
            {
                Debug.Log("Cameras not found");
                return;
            }

            foreach (var camera in cameras)
                if (!_cameras.Contains(camera))
                    _cameras.Add(camera);
        }

        private void AddSelectedCameras()
        {
            foreach (var selected in Selection.gameObjects)
            {
                foreach (var camera in selected.GetComponentsInChildren<Camera>())
                    if (!_cameras.Contains(camera))
                        _cameras.Add(camera);
            }
        }

        private void RemoveSelectedCameras()
        {
            foreach (var selected in Selection.gameObjects)
            {
                foreach (var camera in selected.GetComponentsInChildren<Camera>())
                    if (_cameras.Contains(camera))
                        _cameras.Remove(camera);
            }
        }

        private void RemoveAllCameras()
        {
            _cameras.Clear();
        }


        private void AddStaticObjects()
        {
            foreach (var renderer in FindObjectsOfType<MeshRenderer>())
                if (IsValidGameObject(renderer.gameObject))
                    if (!_objects.Contains(renderer))
                        _objects.Add(renderer);
        }

        private void AddSelectedObjects()
        {
            foreach (var selected in Selection.gameObjects)
            {
                foreach (var renderer in selected.GetComponentsInChildren<MeshRenderer>())
                    if (IsValidGameObject(renderer.gameObject))
                        if (!_objects.Contains(renderer))
                            _objects.Add(renderer);
            }
        }

        private void RemoveSelectedObjects()
        {
            foreach (var selected in Selection.gameObjects)
            {
                foreach (var renderer in selected.GetComponentsInChildren<MeshRenderer>())
                    if (_objects.Contains(renderer))
                        _objects.Remove(renderer);
            }
        }

        private void RemoveAllObjects()
        {
            _objects.Clear();
        }


        private void AddSelectedOccluders()
        {
            foreach (var selected in Selection.gameObjects)
            {
                Collider collider = selected.GetComponent<Collider>();

                if (collider != null)
                    _occluders.Add(collider);
            }
        }

        private void RemoveSelectedOccluders()
        {
            foreach (var selected in Selection.gameObjects)
            {
                Collider collider = selected.GetComponent<Collider>();

                if (collider != null && _occluders.Contains(collider))
                    _occluders.Remove(collider);
            }
        }

        private void RemoveAllOccluders()
        {
            _occluders.Clear();
        }


        private void AddCullingArea()
        {
            string name = "Culling Area " + (_areasTransforms.Count + 1);

            _areasTransforms.Add(new GameObject(name).transform);
        }

        private void RemoveCullingArea(int index)
        {
            DestroyImmediate(_areasTransforms[index].gameObject);

            _areasTransforms.RemoveAt(index);
        }

        private void RemoveAllCullingAreas()
        {
            int count = _areasTransforms.Count;

            for (int i = 0; i < count; i++)
                RemoveCullingArea(0);
        }


        private void Bake()
        {
            if (!IsAssignedDataValid())
                return;

            Clear();

            CalculateAreasBounds();

            StaticCullingMaster cullingMaster = new StaticCullingMaster(_cameras.ToArray(), _objects.ToArray(), _occluders.ToArray(), 
                _areasBounds.ToArray(), _fastBake, _jobsPerObject, _cellSize, ACSInfo.CullingLayer);

            cullingMaster.Compute();
        }

        private void Clear()
        {
            foreach (var culling in FindObjectsOfType<StaticCulling>())
                DestroyImmediate(culling.gameObject);

            foreach (var tree in FindObjectsOfType<BinaryTree>())
                DestroyImmediate(tree.gameObject);
        }



        private void UpdateAssignedData()
        {
            _cameras = _cameras.Where(c => c != null).ToList();
            _objects = _objects.Where(obj => obj != null && IsValidGameObject(obj.gameObject)).ToList();
            _occluders = _occluders.Distinct().Where(c => c != null).ToList();
            _areasTransforms = _areasTransforms.Where(area => area != null).ToList();
            _castersCount = CalculateCastersCount();
        }

        private bool IsAssignedDataValid()
        {
            if (_objects.Count == 0)
            {
                Debug.Log("No objects assign");
                return false;
            }

            if (_areasPlacing == AreasPlacing.Placing_By_User && _areasTransforms.Count == 0)
            {
                Debug.Log("No areas assign");
                return false;
            }

            //int castersCount = CalculateCastersCount();
            //if (castersCount > 100000)
            //{
            //    Debug.Log("Casters count more then 100000");
            //    Debug.Log("Please decrease the 'Culling Area' or increase the 'Cell Size'");
            //    Debug.Log("This scene is too big. Computing this scene takes a very lot time");
            //    Debug.Log("I recommend You use Dynamic Culling. Because Dynamic Culling not requre preprocessing");
            //    Debug.Log("Or You can contact me(andre-orsk@yandex.ru) and I will help You to choose the best solution");
            //    return false;
            //}

            return true;
        }

        private void CalculateAreasBounds()
        {
            _areasBounds.Clear();

            if (_areasPlacing == AreasPlacing.Around_Static && _objects.Count > 0)
                _areasBounds.Add(StaticCullingMaster.CalculateBoundingBox(_objects));

            else if (_areasPlacing == AreasPlacing.Placing_By_User && _areasTransforms.Count > 0)
            {
                for (int i = 0; i < _areasTransforms.Count; i++)
                    _areasBounds.Add(new Bounds(_areasTransforms[i].position, _areasTransforms[i].lossyScale));
            }
        }

        private int CalculateCastersCount()
        {
            CalculateAreasBounds();

            return StaticCullingMaster.CalculateCastersCount(_areasBounds, _cellSize);
        }

        private void CalculateBakingTime()
        {
            if (!IsAssignedDataValid())
                return;

            CalculateAreasBounds();

            StaticCullingMaster cullingMaster = new StaticCullingMaster(_cameras.ToArray(), _objects.ToArray(), _occluders.ToArray(),
                _areasBounds.ToArray(), _fastBake, _jobsPerObject, _cellSize, ACSInfo.CullingLayer);

            _bakingTime = cullingMaster.CalculateComputingTime();
        }

        private bool IsDataBaked()
        {
            if (FindObjectOfType<StaticCulling>() != null || FindObjectOfType<BinaryTree>() != null)
                return true;

            return false;
        }

        private bool IsValidGameObject(GameObject go)
        {
            if (!go.isStatic)
                return false;

            MeshFilter filter = go.GetComponent<MeshFilter>();

            if (filter == null || filter.sharedMesh == null)
                return false;

            MeshRenderer renderer = go.GetComponent<MeshRenderer>();

            if (renderer == null || !renderer.enabled)
                return false;

            return true;
        }



        private void OnSceneGUI(SceneView sceneView)
        {
            UpdateAssignedData();

            CalculateAreasBounds();

            if (_showSelectedObjects && _selectedTool == 1)
            {
                Handles.color = Color.blue;
                foreach (var obj in _objects)
                {
                    Bounds bounds = obj.GetComponent<MeshRenderer>().bounds;

                    Handles.DrawWireCube(bounds.center, bounds.size);
                }
            }

            if (_selectedTool == 3)
            {
                for (int i = 0; i < _areasBounds.Count; i++)
                {
                    if (!_showCells)
                    {
                        Handles.color = Color.blue;
                        Handles.DrawWireCube(_areasBounds[i].center, _areasBounds[i].size);
                    }
                    else
                    {
                        Handles.color = Color.yellow;
                        foreach (var bounds in StaticCullingMaster.CalculateCellsBounds(_areasBounds[i], _cellSize))
                            Handles.DrawWireCube(bounds.center, bounds.size);
                    }
                }
            }
        }

        private void OnDestroy()
        {
            SceneView.onSceneGUIDelegate -= OnSceneGUI;

            RemoveAllCullingAreas();
        }
    }
}
