using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AdvancedCullingSystem.StaticCullingCore
{
    public class BinaryTreeNode : MonoBehaviour
    {
        [SerializeField] private BinaryTreeNode _left;
        [SerializeField] private BinaryTreeNode _right;
        [SerializeField] private List<MeshRenderer> _visibleRenderers = new List<MeshRenderer>();


        public Vector3 center
        {
            get
            {
                return transform.position;
            }
        }
        public Vector3 size
        {
            get
            {
                return transform.lossyScale;
            }
        }

        public BinaryTreeNode left
        {
            get
            {
                return _left;
            }
        }
        public BinaryTreeNode right
        {
            get
            {
                return _right;
            }
        }

        public List<MeshRenderer> visibleRenderers
        {
            get
            {
                return _visibleRenderers;
            }
        }
        public bool isLeaf
        {
            get
            {
                return left == null && right == null;
            }
        }


        public static BinaryTreeNode Create(Vector3 center, Vector3 size, string name = "TreeNode")
        {
            GameObject go = new GameObject(name);

            go.transform.position = center;
            go.transform.localScale = size;

            return go.AddComponent<BinaryTreeNode>();
        }


        public void SetChilds(BinaryTreeNode left, BinaryTreeNode right)
        {
            _left = left;
            _right = right;

            if(left != null)
                _left.transform.parent = transform;

            if(right != null)
                _right.transform.parent = transform;
        }

        public void SetVisibleRenderers(List<MeshRenderer> renderers)
        {
            _visibleRenderers = renderers;
        }


        public void AddVisibleRenderers(Caster caster)
        {
            _visibleRenderers.AddRange(caster.visibleRenderers);
        }

        public void DistinctVisibleRenderersList()
        {
            _visibleRenderers = _visibleRenderers.Distinct().ToList();
        }
    }
}
