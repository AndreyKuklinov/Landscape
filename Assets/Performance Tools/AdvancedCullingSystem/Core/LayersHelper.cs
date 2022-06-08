using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace AdvancedCullingSystem
{
    public static class LayersHelper
    {
#if UNITY_EDITOR
        public static int CreateLayer(string layerName)
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layers = tagManager.FindProperty("layers");

            int emptyLayerIdx = -1;

            SerializedProperty layerSP;
            for (int i = 0; i < layers.arraySize; i++)
            {
                layerSP = layers.GetArrayElementAtIndex(i);

                if (layerSP.stringValue == layerName)
                    return i;

                if ((layerSP.stringValue == null || layerSP.stringValue == "") && (emptyLayerIdx == -1 && i >= 8))
                    emptyLayerIdx = i;
            }

            if (emptyLayerIdx < 0)
            {
                Debug.Log("Advanced Culling System require needs to take one layer. " +
                    "Please open 'Project Settins/Tags and Layers' and clear one layer field." +
                    "Then open this tool again");

                return -1;
            }

            layerSP = layers.GetArrayElementAtIndex(emptyLayerIdx);
            layerSP.stringValue = layerName;

            tagManager.ApplyModifiedProperties();

            for (int i = 0; i < layers.arraySize; i++)
            {
                layerSP = layers.GetArrayElementAtIndex(i);

                if (layerSP.stringValue == null || layerSP.stringValue == "")
                    continue;

                Physics.IgnoreLayerCollision(LayerMask.NameToLayer(layerName), LayerMask.NameToLayer(layerSP.stringValue));
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(layerName), LayerMask.NameToLayer(layerSP.stringValue));
            }

            tagManager.ApplyModifiedProperties();

            return emptyLayerIdx;
        }

#else
        public static int CreateLayer(string layerName)
        {
            return -1;
        }
#endif
    }
}
