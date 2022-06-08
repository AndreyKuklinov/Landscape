using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdvancedCullingSystem.StaticCullingCore
{
    public class BinaryTree : MonoBehaviour
    {
        [SerializeField]
        private BinaryTreeNode _rootNode;
        public BinaryTreeNode rootNode
        {
            get
            {
                return _rootNode;
            }
        }


        public void CreateTree(Caster[,,] casters)
        {
            int countX = casters.GetLength(0);
            int countY = casters.GetLength(1);
            int countZ = casters.GetLength(2);

            Vector3 min = casters[0, 0, 0].transform.position;
            Vector3 max = casters[countX - 1, countY - 1, countZ - 1].transform.position;

            _rootNode = BinaryTreeNode.Create((min + max) / 2, max - min);
            _rootNode.transform.parent = transform;

            CreateChilds(_rootNode, casters);
        }

        private void CreateChilds(BinaryTreeNode parent, Caster[,,] casters, int depth = 1)
        {
            int countX = casters.GetLength(0);
            int countY = casters.GetLength(1);
            int countZ = casters.GetLength(2);

            if (countX <= 2 && countY <= 2 && countZ <= 2)
            {
                for (int i = 0; i < countX; i++)
                    for (int c = 0; c < countY; c++)
                        for (int j = 0; j < countZ; j++)
                            if (casters[i, c, j].visibleRenderers != null)
                                parent.AddVisibleRenderers(casters[i, c, j]);

                parent.DistinctVisibleRenderersList();

                return;
            }

            ComputeChildsData(casters, out BinaryTreeNode leftNode, out BinaryTreeNode rightNode,
                out Caster[,,] leftNodeCasters, out Caster[,,] rightNodeCasters);

            parent.SetChilds(leftNode, rightNode);

            CreateChilds(leftNode, leftNodeCasters, depth + 1);
            CreateChilds(rightNode, rightNodeCasters, depth + 1);
        }

        private void ComputeChildsData(Caster[,,] casters, out BinaryTreeNode leftNode, out BinaryTreeNode rightNode,
            out Caster[,,] leftNodeCasters, out Caster[,,] rightNodeCasters)
        {
            int lastX = casters.GetLength(0) - 1;
            int lastY = casters.GetLength(1) - 1;
            int lastZ = casters.GetLength(2) - 1;

            Vector3 lMin = Vector3.zero, lMax = Vector3.zero, rMin = Vector3.zero, rMax = Vector3.zero;

            if (lastX >= lastY && lastX >= lastZ && lastX > 1)
            {
                int middle = lastX / 2;

                leftNodeCasters = new Caster[middle + 1, lastY + 1, lastZ + 1];
                rightNodeCasters = new Caster[lastX - middle + 1, lastY + 1, lastZ + 1];

                lMin = casters[0, 0, 0].transform.position;
                lMax = casters[middle, lastY, lastZ].transform.position;

                rMin = casters[middle, 0, 0].transform.position;
                rMax = casters[lastX, lastY, lastZ].transform.position;

                for (int i = 0; i <= lastX; i++)
                    for (int c = 0; c <= lastY; c++)
                        for (int j = 0; j <= lastZ; j++)
                        {
                            if (i <= middle)
                                leftNodeCasters[i, c, j] = casters[i, c, j];

                            if (i >= middle)
                                rightNodeCasters[i - middle, c, j] = casters[i, c, j];
                        }
            }
            else if (lastY >= lastX && lastY >= lastZ && lastY > 1)
            {
                int middle = lastY / 2;

                leftNodeCasters = new Caster[lastX + 1, middle + 1, lastZ + 1];
                rightNodeCasters = new Caster[lastX + 1, lastY - middle + 1, lastZ + 1];

                lMin = casters[0, 0, 0].transform.position;
                lMax = casters[lastX, middle, lastZ].transform.position;

                rMin = casters[0, middle, 0].transform.position;
                rMax = casters[lastX, lastY, lastZ].transform.position;

                for (int i = 0; i <= lastX; i++)
                    for (int c = 0; c <= lastY; c++)
                        for (int j = 0; j <= lastZ; j++)
                        {
                            if (c <= middle)
                                leftNodeCasters[i, c, j] = casters[i, c, j];

                            if (c >= middle)
                                rightNodeCasters[i, c - middle, j] = casters[i, c, j];
                        }
            }
            else if (lastZ >= lastX && lastZ >= lastY && lastZ > 1)
            {
                int middle = lastZ / 2;

                leftNodeCasters = new Caster[lastX + 1, lastY + 1, middle + 1];
                rightNodeCasters = new Caster[lastX + 1, lastY + 1, lastZ - middle + 1];

                lMin = casters[0, 0, 0].transform.position;
                lMax = casters[lastX, lastY, middle].transform.position;

                rMin = casters[0, 0, middle].transform.position;
                rMax = casters[lastX, lastY, lastZ].transform.position;

                for (int i = 0; i <= lastX; i++)
                    for (int c = 0; c <= lastY; c++)
                        for (int j = 0; j <= lastZ; j++)
                        {
                            if (j <= middle)
                                leftNodeCasters[i, c, j] = casters[i, c, j];

                            if (j >= middle)
                                rightNodeCasters[i, c, j - middle] = casters[i, c, j];
                        }
            }
            else
                throw new System.Exception("Ex");

            leftNode = BinaryTreeNode.Create((lMin + lMax) / 2, (lMax - lMin));
            rightNode = BinaryTreeNode.Create((rMin + rMax) / 2, (rMax - rMin));
        }


        public BinaryTreeNode GetNode(Vector3 point)
        {
            return GetNode(_rootNode, point);
        }

        public BinaryTreeNode GetNode(BinaryTreeNode parent, Vector3 point)
        {
            if (parent.isLeaf)
                return parent;

            if (parent.left != null)
            {
                if (new Bounds(parent.left.center, parent.left.size).Contains(point))
                    return GetNode(parent.left, point);
            }

            return GetNode(parent.right, point);
        }
    }
}
