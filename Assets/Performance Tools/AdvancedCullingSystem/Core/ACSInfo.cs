using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdvancedCullingSystem
{
    public static class ACSInfo
    {
        public const string CullingLayerName = "ACSCulling";
        public static int CullingLayer
        {
            get
            {
                int layer = LayerMask.NameToLayer(CullingLayerName);

                if (layer == -1)
                    return LayersHelper.CreateLayer(CullingLayerName);

                return layer;
            }
        }
        public static int CullingMask
        {
            get
            {
                int mask = LayerMask.GetMask(CullingLayerName);

                if (mask == 0)
                    return LayersHelper.CreateLayer(CullingLayerName);

                return mask;
            }
        }
    }
}
