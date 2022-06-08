using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace AdvancedCullingSystem.StaticCullingCore
{
    public class StaticCulling : MonoBehaviour
    {
        public static StaticCulling Instance { get; private set; }

        [SerializeField]
        [HideInInspector]
        private MeshRenderer[] _renderers;

        [SerializeField]
        [HideInInspector]
        private BinaryTree[] _trees;

        [SerializeField]
        private List<Camera> _cameras;

        [Header("Editor only")]

        [SerializeField]
        private bool _enableFrustum;

        [SerializeField]
        private bool _drawCullingArea;

        private List<MeshRenderer> _activeRenderers = new List<MeshRenderer>();


        public static StaticCulling Create(MeshRenderer[] renderers, BinaryTree[] trees, Camera[] cameras = null)
        {
            StaticCulling staticCulling = new GameObject("Static Culling").AddComponent<StaticCulling>();

            staticCulling._renderers = renderers;
            staticCulling._trees = trees;

            if (cameras != null)
            {
                staticCulling._cameras = new List<Camera>();

                for (int i = 0; i < cameras.Length; i++)
                    staticCulling.AddCamera(cameras[i]);
            }

            return staticCulling;
        }


        private void Awake()
        {
            Instance = this;

            _activeRenderers.AddRange(_renderers);
        }

        private void Start()
        {
            CheckCameras();
        }

        private void Update()
        {
            try
            {
                DisableRenderers();

                int i = 0;
                while (i < _cameras.Count)
                {
                    if (_cameras[i] == null)
                    {
                        Debug.Log("StaticCulling::looks like camera was destroyed");

                        _cameras.RemoveAt(i);

                        if (!CheckCameras())
                            return;

                        continue;
                    }

                    bool insideArea = false;
                    Vector3 pos = _cameras[i].transform.position;

                    for (int c = 0; c < _trees.Length; c++)
                    {
                        if (new Bounds(_trees[c].rootNode.center, _trees[c].rootNode.size).Contains(pos))
                        {
                            EnableRenderers(_cameras[i], _trees[c].GetNode(pos).visibleRenderers);
                            insideArea = true;
                            break;
                        }
                    }

                    if (!insideArea)
                    {
                        EnableRenderers();
                        return;
                    }

                    i++;
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log("StaticCulling::get exception");
                Debug.Log("Cause : " + ex.Message + " " + ex.StackTrace);
                Debug.Log("Please write about it on e-mail(andre-orsk@yandex.ru) and I will help You");
                Debug.Log("-----------------------------------");

                Disable();
            }

        }

        private void OnDrawGizmos()
        {
            if (!_drawCullingArea || _trees == null || _trees.Length == 0)
                return;

            foreach (var tree in _trees)
                BinaryTreeUtil.DrawGizmos(tree, Color.blue);
        }

        private void OnDrawGizmosSelected()
        {
            if (_drawCullingArea || _trees == null || _trees.Length == 0)
                return;

            Gizmos.color = Color.blue;
            foreach (var tree in _trees)
                Gizmos.DrawWireCube(tree.rootNode.center, tree.rootNode.size);
        }


        private bool CheckCameras()
        {
            if (_cameras == null || _cameras.Count == 0)
            {
                Debug.Log("StaticCulling::no cameras assigned");

                Disable();

                return false;
            }

            return true;
        }

        private void DisableRenderers()
        {
            for (int i = 0; i < _activeRenderers.Count; i++)
                _activeRenderers[i].shadowCastingMode = ShadowCastingMode.ShadowsOnly;

            _activeRenderers.Clear();
        }

        private void EnableRenderers()
        {
            for (int i = 0; i < _renderers.Length; i++)
                _renderers[i].shadowCastingMode = ShadowCastingMode.On;

            _activeRenderers.AddRange(_renderers);
        }

        private void EnableRenderers(Camera camera, List<MeshRenderer> renderers)
        {
#if UNITY_EDITOR

            Plane[] planes = null;

            if (_enableFrustum)
                planes = GeometryUtility.CalculateFrustumPlanes(camera);

            for (int i = 0; i < renderers.Count; i++)
            {
                if (!_enableFrustum || GeometryUtility.TestPlanesAABB(planes, renderers[i].bounds))
                {
                    renderers[i].shadowCastingMode = ShadowCastingMode.On;

                    _activeRenderers.Add(renderers[i]);
                }
            }

#else

            for (int i = 0; i < renderers.Count; i++)
                renderers[i].shadowCastingMode = ShadowCastingMode.On;

            _activeRenderers.AddRange(renderers);

#endif
        }


        public void AddCamera(Camera camera)
        {
            if (!_cameras.Contains(camera))
                _cameras.Add(camera);

            if (!enabled)
                Enable();
        }

        public void RemoveCamera(Camera camera)
        {
            if (_cameras.Contains(camera))
                _cameras.Remove(camera);

            CheckCameras();
        }

        public void Enable()
        {
            enabled = true;

            CheckCameras();
        }

        public void Disable()
        {
            EnableRenderers();

            enabled = false;
        }
    }
}