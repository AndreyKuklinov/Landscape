using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;
using UnityEngine;
using UnityEditor;

using Random = UnityEngine.Random;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace AdvancedCullingSystem.StaticCullingCore
{
    public class StaticCullingMaster
    {
        public const int StandardInitJobsCount = 2000;
        public const int FastInitJobsCount = 6000;
        public const int RaysBatch = 100000;

        private Camera[] _cameras;
        private MeshRenderer[] _renderers;
        private Collider[] _occluders;
        private MeshFilter[] _filters;
        private Bounds[] _areas;

        private bool _fastBake;
        private int _maxRaysPerObject;
        private float _cellSize;
        private int _layer;

        private List<Caster> _casters;
        private List<Caster[,,]> _castersUnit;
        private CasterDataContainer _dataContainer;

        private MeshCollider[] _renderersColliders;
        private List<Collider> _disabledColliders;

        private BinaryTree[] _trees;


        public static Bounds CalculateBoundingBox(IEnumerable<MeshRenderer> renderers)
        {
            Vector3 min = Vector3.one * float.MaxValue;
            Vector3 max = -min;

            foreach(var renderer in renderers)
            {
                Bounds rendererBounds = renderer.bounds;

                min.x = Mathf.Min(rendererBounds.min.x, min.x);
                min.y = Mathf.Min(rendererBounds.min.y, min.y);
                min.z = Mathf.Min(rendererBounds.min.z, min.z);

                max.x = Mathf.Max(rendererBounds.max.x, max.x);
                max.y = Mathf.Max(rendererBounds.max.y, max.y);
                max.z = Mathf.Max(rendererBounds.max.z, max.z);
            }

            return new Bounds((min + max) / 2, max - min);
        }

        public static List<Bounds> CalculateCellsBounds(Bounds boundingBox, float cellSize)
        {
            Vector3 min = boundingBox.min;

            List<Bounds> bounds = new List<Bounds>();

            int countX = Mathf.CeilToInt(boundingBox.size.x / cellSize);
            int countY = Mathf.CeilToInt(boundingBox.size.y / cellSize);
            int countZ = Mathf.CeilToInt(boundingBox.size.z / cellSize);

            for (int i = 0; i < countX; i++)
            {
                for (int c = 0; c < countY; c++)
                {
                    for (int j = 0; j < countZ; j++)
                    {
                        Vector3 center = min;

                        center.x += (i * cellSize) + cellSize / 2;
                        center.y += (c * cellSize) + cellSize / 2;
                        center.z += (j * cellSize) + cellSize / 2;

                        bounds.Add(new Bounds(center, Vector3.one * cellSize));
                    }
                }
            }

            return bounds;
        }

        public static int CalculateCastersCount(List<Bounds> areas, float cellSize)
        {
            return CalculateCastersCount(areas, cellSize, out int x, out int y, out int z);
        }

        public static int CalculateCastersCount(List<Bounds> areas, float cellSize, 
            out int countX, out int countY, out int countZ)
        {
            countX = 0;
            countY = 0;
            countZ = 0;

            for (int i = 0; i < areas.Count; i++)
            {
                countX += Mathf.CeilToInt((areas[i].max.x - areas[i].min.x) / cellSize) + 1;
                countY += Mathf.CeilToInt((areas[i].max.y - areas[i].min.y) / cellSize) + 1;
                countZ += Mathf.CeilToInt((areas[i].max.z - areas[i].min.z) / cellSize) + 1;
            }

            return countX * countY * countZ;
        }


        public StaticCullingMaster(Camera[] cameras, MeshRenderer[] renderers, Collider[] occluders, Bounds[] areas,
            bool fastBake, int maxJobs, float cellSize, int layer)
        {
            _renderers = renderers;
            _occluders = occluders;
            _filters = _renderers.Select(r => r.GetComponent<MeshFilter>()).ToArray();
            _areas = areas;

            _fastBake = fastBake;
            _maxRaysPerObject = maxJobs;
            _cellSize = cellSize;
            _layer = layer;

            _cameras = cameras;
        }

        public float CalculateComputingTime()
        {
            float totalSeconds = 0;

            try
            {
                int totalCasters = CalculateCastersCount(_areas.ToList(), _cellSize);
                int castersCount = Mathf.Min(500, totalCasters);
                float ratio = (float)totalCasters / castersCount;

                InstantiateColliders();

                Stopwatch mainSW = new Stopwatch();
                mainSW.Start();

                InstantiateCastersRandom(castersCount);

                if (InitializeCasters())
                {
                    mainSW.Stop();

                    if (_fastBake)
                    {
                        totalSeconds = mainSW.Elapsed.Seconds * ratio;
                    }
                    else
                    {
                        Stopwatch sw = new Stopwatch();
                        sw.Start();

                        if (ComputeVisibilityWithoutCasting())
                        {
                            sw.Stop();

                            mainSW.Start();
                            if (ComputeVisibility())
                            {
                                mainSW.Stop();

                                totalSeconds = (mainSW.Elapsed.Seconds - sw.Elapsed.Seconds) * ratio + sw.Elapsed.Seconds;
                            }
                        }

                        sw.Stop();
                    }
                }

                mainSW.Stop();
            }
            catch(System.Exception ex)
            {
                EditorUtility.ClearProgressBar();

                Debug.Log("Can't calculate baking time");
                Debug.Log("Cause : " + ex.Message + " " + ex.StackTrace);
                Debug.Log("Please write about it on e-mail(andre-orsk@yandex.ru) and I will help You");
                Debug.Log("-----------------------------------");

                totalSeconds = 0;
            }

            Clear();

            return (float)System.Math.Round(totalSeconds / 60, 2);
        }

        public void Compute()
        {
            try
            {
                EditorUtility.DisplayProgressBar("Start...", "0 of 1", 0);

                InstantiateCasters();
                InstantiateColliders();

                EditorUtility.ClearProgressBar();

                if (InitializeCasters())
                {
                    if(_fastBake || ComputeVisibility())
                    {
                        CreateBinaryTrees();

                        CreateStaticCullingObject();
                    }
                }
            }
            catch (System.Exception ex)
            {
                EditorUtility.ClearProgressBar();

                Debug.Log("Can't compute visibility");
                Debug.Log("Cause : " + ex.Message + " " + ex.StackTrace);
                Debug.Log("Please write about it on e-mail(andre-orsk@yandex.ru) and I will help You");
                Debug.Log("-----------------------------------");
            }
            finally
            {
                Clear();
            }
        }


        private void InstantiateCastersRandom(int count)
        {
            _casters = new List<Caster>();

            int created = 0;
            while (created < count)
            {
                int idx = Random.Range(0, _areas.Length);

                float posX = Random.Range(_areas[idx].min.x, _areas[idx].max.x);
                float posY = Random.Range(_areas[idx].min.y, _areas[idx].max.y);
                float posZ = Random.Range(_areas[idx].min.z, _areas[idx].max.z);

                Caster caster = new GameObject("Caster").AddComponent<Caster>();

                caster.transform.position = new Vector3(posX, posY, posZ);

                _casters.Add(caster);

                created++;
            }
        }

        private bool ComputeVisibilityWithoutCasting()
        {
            NativeList<int> targets = new NativeList<int>(Allocator.TempJob);
            NativeList<int2> pointers = new NativeList<int2>(Allocator.TempJob);
            NativeList<float3> points = new NativeList<float3>(Allocator.TempJob);

            bool canceled = false;
            for (int i = 0; i < _renderers.Length; i++)
            {
                if (canceled)
                    break;

                float progress = (float)System.Math.Round((float)i / _renderers.Length, 2);
                float percents = progress * 100;

                if (EditorUtility.DisplayCancelableProgressBar("Calculating Visibility...", percents + "% of 100%", progress))
                {
                    targets.Dispose();
                    pointers.Dispose();
                    points.Dispose();

                    EditorUtility.ClearProgressBar();

                    return false;
                }

                targets.Add(_dataContainer.iDByRenderer[_renderers[i]]);

                int startIndex = points.Length;

                Mesh mesh = _filters[i].sharedMesh;

                Vector3[] vertices = mesh.vertices.Select(v => _filters[i].transform.TransformPoint(v)).ToArray();
                List<int> triangles = new List<int>();

                for (int c = 0; c < mesh.subMeshCount; c++)
                    triangles.AddRange(mesh.GetTriangles(c));

                List<Vector3> rayPoints = new List<Vector3>(vertices.Distinct());

                for (int c = 0; c < triangles.Count; c += 3)
                {
                    Vector3 vec1 = vertices[triangles[c]];
                    Vector3 vec2 = vertices[triangles[c + 1]];
                    Vector3 vec3 = vertices[triangles[c + 2]];

                    rayPoints.Add((vec1 + vec2 + vec3) / 3);
                }

                int step = _maxRaysPerObject >= rayPoints.Count ? 1 : Mathf.RoundToInt((float)rayPoints.Count / _maxRaysPerObject);

                for (int c = 0; c < rayPoints.Count; c += step)
                {
                    points.Add(rayPoints[c]);

                    if (points.Length >= RaysBatch)
                    {
                        pointers.Add(new int2(startIndex, points.Length));

                        targets.Clear();
                        points.Clear();
                        pointers.Clear();

                        targets.Add(_dataContainer.iDByRenderer[_renderers[i]]);

                        startIndex = 0;
                    }
                }

                pointers.Add(new int2(startIndex, points.Length));
            }

            targets.Dispose();
            points.Dispose();
            pointers.Dispose();

            EditorUtility.ClearProgressBar();

            return !canceled;
        }


        private void InstantiateColliders()
        {
            _disabledColliders = new List<Collider>();

            _disabledColliders = Object.FindObjectsOfType<Collider>()
                .Where(col => col.gameObject.layer == _layer)
                .ToList();

            _disabledColliders.ForEach(col => col.enabled = false);


            _renderersColliders = new MeshCollider[_renderers.Length];
            for(int i = 0; i < _renderersColliders.Length; i++)
            {
                GameObject go = new GameObject("Mesh Collider");

                go.transform.parent = _renderers[i].transform;

                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;

                go.layer = _layer;

                MeshCollider col = go.AddComponent<MeshCollider>();
                col.sharedMesh = _renderers[i].GetComponent<MeshFilter>().sharedMesh;

                _renderersColliders[i] = col;
            }

            for (int i = 0; i < _occluders.Length; i++)
            {
                _occluders[i].gameObject.layer = _layer;
                _occluders[i].enabled = true;
            }
        }

        private void InstantiateCasters()
        {
            _casters = new List<Caster>();
            _castersUnit = new List<Caster[,,]>();

            for (int i = 0; i < _areas.Length; i++)
            {
                Bounds bounds = _areas[i];

                int countX = Mathf.CeilToInt((bounds.max.x - bounds.min.x) / _cellSize);
                int countY = Mathf.CeilToInt((bounds.max.y - bounds.min.y) / _cellSize);
                int countZ = Mathf.CeilToInt((bounds.max.z - bounds.min.z) / _cellSize);

                _castersUnit.Add(new Caster[countX + 1, countY + 1, countZ + 1]);

                for (int x = 0; x <= countX; x++)
                    for (int y = 0; y <= countY; y++)
                        for (int z = 0; z <= countZ; z++)
                        {
                            Caster caster = new GameObject("Caster").AddComponent<Caster>();

                            caster.transform.position = bounds.min + new Vector3(x, y, z) * _cellSize;
                            caster.transform.localScale = Vector3.one * _cellSize;

                            _casters.Add(caster);
                            _castersUnit[i][x, y, z] = caster;
                        }
            }
        }

        private bool InitializeCasters()
        {
            _dataContainer = new CasterDataContainer(_renderers, _renderersColliders, _occluders, LayerMask.LayerToName(_layer));

            NativeList<float3> sphereUniformDirections = CasterUtility.SphereUniform(_fastBake ? FastInitJobsCount : StandardInitJobsCount);

            CasterUtility.InitializeCasters(_casters, _dataContainer);

            bool isCompleted = CasterUtility.ProcessCasters(_casters, sphereUniformDirections, "Initializing...");

            sphereUniformDirections.Dispose();

            return isCompleted;
        }

        private bool ComputeVisibility()
        {
            NativeList<int> targets = new NativeList<int>(Allocator.TempJob);
            NativeList<int2> pointers = new NativeList<int2>(Allocator.TempJob);
            NativeList<float3> points = new NativeList<float3>(Allocator.TempJob);

            bool canceled = false;
            for (int i = 0; i < _renderers.Length; i++)
            {
                if (canceled)
                    break;

                float progress = (float)System.Math.Round((float) i / _renderers.Length, 2);
                float percents = progress * 100;

                if (EditorUtility.DisplayCancelableProgressBar("Calculating Visibility...", percents + "% of 100%", progress))
                {
                    targets.Dispose();
                    pointers.Dispose();
                    points.Dispose();

                    EditorUtility.ClearProgressBar();

                    return false;
                }

                targets.Add(_dataContainer.iDByRenderer[_renderers[i]]);

                int startIndex = points.Length;

                Mesh mesh = _filters[i].sharedMesh;

                Vector3[] vertices = mesh.vertices.Select(v => _filters[i].transform.TransformPoint(v)).ToArray();
                List<int> triangles = new List<int>();

                for (int c = 0; c < mesh.subMeshCount; c++)
                    triangles.AddRange(mesh.GetTriangles(c));

                List<Vector3> rayPoints = new List<Vector3>(vertices.Distinct());

                for (int c = 0; c < triangles.Count; c += 3)
                {
                    Vector3 vec1 = vertices[triangles[c]];
                    Vector3 vec2 = vertices[triangles[c + 1]];
                    Vector3 vec3 = vertices[triangles[c + 2]];

                    rayPoints.Add((vec1 + vec2 + vec3) / 3);
                }

                int step = _maxRaysPerObject >= rayPoints.Count ? 1 : Mathf.RoundToInt((float)rayPoints.Count / _maxRaysPerObject);

                for (int c = 0; c < rayPoints.Count; c += step)
                {
                    points.Add(rayPoints[c]);

                    if (points.Length >= RaysBatch)
                    {
                        pointers.Add(new int2(startIndex, points.Length));

                        if (!CasterUtility.ProcessCasters(_casters, targets, pointers, points, "Step..."))
                        {
                            canceled = true;
                            break;
                        }

                        targets.Clear();
                        points.Clear();
                        pointers.Clear();

                        targets.Add(_dataContainer.iDByRenderer[_renderers[i]]);

                        startIndex = 0;
                    }
                }

                pointers.Add(new int2(startIndex, points.Length));
            }

            if(!canceled)
                canceled = !CasterUtility.ProcessCasters(_casters, targets, pointers, points, "Step...");

            CasterUtility.Dispose(_casters, _dataContainer);

            targets.Dispose();
            points.Dispose();
            pointers.Dispose();

            EditorUtility.ClearProgressBar();

            return !canceled;
        }

        private void CreateBinaryTrees()
        {
            _trees = new BinaryTree[_areas.Length];

            for (int i = 0; i < _areas.Length; i++)
            {
                EditorUtility.DisplayProgressBar("Creating Binary Trees...", i + " of " + _areas.Length,
                    (float)i / _areas.Length);

                _trees[i] = new GameObject("Binary Tree").AddComponent<BinaryTree>();
                _trees[i].CreateTree(_castersUnit[i]);

                BinaryTreeUtil.OptimizeTree(_trees[i]);
            }

            EditorUtility.ClearProgressBar();
        }

        private void CreateStaticCullingObject()
        {
            StaticCulling staticCulling = StaticCulling.Create(_renderers, _trees, _cameras);
          
            for (int i = 0; i < _trees.Length; i++)
                _trees[i].transform.parent = staticCulling.transform;
        }

        private void Clear()
        {
            if (_disabledColliders != null)
                _disabledColliders.ForEach(col => col.enabled = true);

            if (_renderersColliders != null)
            {
                for (int i = 0; i < _renderersColliders.Length; i++)
                    if (_renderersColliders[i] != null)
                        Object.DestroyImmediate(_renderersColliders[i].gameObject);
            }

            if (_casters != null)
            {
                for (int i = 0; i < _casters.Count; i++)
                    if (_casters[i] != null)
                    {
                        _casters[i].Dispose();

                        Object.DestroyImmediate(_casters[i].gameObject);
                    }
            }

            _dataContainer?.Dispose();
        }
    }

    public static class CasterUtility
    {
        public const int CastersBatch = 10;


        public static NativeList<float3> SphereUniform(int count)
        {
            NativeList<float3> directions = new NativeList<float3>(Allocator.TempJob);

            float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
            float angleIncrement = Mathf.PI * 2 * goldenRatio;

            for (int i = 0; i < count; i++)
            {
                float t = (float)i / count;
                float inclination = Mathf.Acos(1 - 2 * t);
                float azimuth = angleIncrement * i;

                float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
                float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
                float z = Mathf.Cos(inclination);

                directions.Add(new Vector3(x, y, z));
            }

            return directions;
        }

        public static void InitializeCasters(List<Caster> casters, CasterDataContainer container)
        {
            for (int i = 0; i < casters.Count; i++)
                casters[i].Initialize(container);
        }

        public static bool ProcessCasters(List<Caster> casters, NativeList<float3> directions, string barTitle = null)
        {
            NativeList<JobHandle> handles = new NativeList<JobHandle>(Allocator.TempJob);

            int processed = 0;
            while (processed < casters.Count)
            {
                float progress = (float)System.Math.Round((float)processed / casters.Count, 2);
                float percents = progress * 100;

                if (barTitle != null && EditorUtility.DisplayCancelableProgressBar(barTitle, percents + "% of 100%", progress))
                {
                    handles.Dispose();

                    EditorUtility.ClearProgressBar();

                    return false;
                }

                int count = Mathf.Min(CastersBatch, casters.Count - processed);

                for (int i = processed; i < processed + count; i++)
                    casters[i].CreateRayCommands(directions);

                handles.Clear();
                for (int i = processed; i < processed + count; i++)
                    handles.Add(casters[i].CastRays());

                JobHandle.CompleteAll(handles);

                handles.Clear();
                for (int i = processed; i < processed + count; i++)
                    handles.Add(casters[i].ComputeHitsResult());

                JobHandle.CompleteAll(handles);

                for (int i = processed; i < processed + count; i++)
                    casters[i].PushData();

                processed += count;
            }

            handles.Dispose();

            if(barTitle != null)
                EditorUtility.ClearProgressBar();

            return true;
        }

        public static bool ProcessCasters(List<Caster> casters, NativeList<int> targets, 
            NativeList<int2> pointers, NativeList<float3> points, string barTitle = null)
        {
            NativeList<JobHandle> handles = new NativeList<JobHandle>(Allocator.TempJob);

            int processed = 0;
            while (processed < casters.Count)
            {
                float progress = (float)System.Math.Round((float)processed / casters.Count, 2);
                float percents = progress * 100;

                if (barTitle != null && EditorUtility.DisplayCancelableProgressBar(barTitle, percents + "% of 100%", progress))
                {
                    handles.Dispose();

                    EditorUtility.ClearProgressBar();

                    return false;
                }

                int count = Mathf.Min(CastersBatch, casters.Count - processed);

                handles.Clear();
                for (int i = processed; i < processed + count; i++)
                    handles.Add(casters[i].CreateRayCommands(targets, points, pointers));

                JobHandle.CompleteAll(handles);

                handles.Clear();
                for (int i = processed; i < processed + count; i++)
                    handles.Add(casters[i].CastRays());

                JobHandle.CompleteAll(handles);

                handles.Clear();
                for (int i = processed; i < processed + count; i++)
                    handles.Add(casters[i].ComputeHitsResult());

                JobHandle.CompleteAll(handles);

                for (int i = processed; i < processed + count; i++)
                    casters[i].PushData();

                processed += count;
            }

            handles.Dispose();

            if(barTitle != null)
                EditorUtility.ClearProgressBar();

            return true;
        }

        public static void Dispose(List<Caster> casters, CasterDataContainer dataContainer = null)
        {
            for (int i = 0; i < casters.Count; i++)
                casters[i].Dispose();

            if (dataContainer != null)
                dataContainer.Dispose();
        }
    }
}