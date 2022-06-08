using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdvancedCullingSystem.StaticCullingCore
{
    public static class BinaryTreeUtil
    {
        public static void OptimizeTree(BinaryTree binaryTree)
        {
            int depth = 0;

            GetMaxDepth(binaryTree.rootNode, ref depth);

            for (int i = 0; i <= (depth + 1); i++)
                LinkNodesIfEquals(binaryTree, binaryTree.rootNode);
        }

        public static void LinkNodesIfEquals(BinaryTree binaryTree, BinaryTreeNode node)
        {
            if ((node.left != null && node.left.isLeaf) && (node.right != null && node.right.isLeaf))
            {
                if (node.left.visibleRenderers.Count != node.right.visibleRenderers.Count)
                    return;

                for (int i = 0; i < node.left.visibleRenderers.Count; i++)
                    if (!node.right.visibleRenderers.Contains(node.left.visibleRenderers[i]))
                        return;


                node.SetVisibleRenderers(node.left.visibleRenderers);

                Object.DestroyImmediate(node.left.gameObject);
                Object.DestroyImmediate(node.right.gameObject);

                node.SetChilds(null, null);

                return;
            }

            if (node.left != null)
                if (node.left.left != null || node.left.right != null)
                    LinkNodesIfEquals(binaryTree, node.left);

            if (node.right != null)
                if (node.right.left != null || node.right.right != null)
                    LinkNodesIfEquals(binaryTree, node.right);
        }


        public static void GetMaxDepth(BinaryTreeNode node, ref int depth, int currentDepth = 0)
        {
            if (node.isLeaf)
            {
                if (currentDepth > depth)
                    depth = currentDepth;

                return;
            }

            if (node.left != null)
                GetMaxDepth(node.left, ref depth, currentDepth + 1);

            if (node.right != null)
                GetMaxDepth(node.right, ref depth, currentDepth + 1);
        }


        public static void DrawGizmos(BinaryTree binaryTree, Color color)
        {
            if (binaryTree.rootNode == null)
                return;

            DrawGizmosRecursively(binaryTree.rootNode, color);
        }

        public static void DrawGizmosRecursively(BinaryTreeNode parent, Color color)
        {
            if (parent.isLeaf)
            {
                Gizmos.color = color;
                Gizmos.DrawWireCube(parent.center, parent.size);

                return;
            }

            if (parent.left != null)
                DrawGizmosRecursively(parent.left, color);

            if (parent.right != null)
                DrawGizmosRecursively(parent.right, color);
        }
    }
}
