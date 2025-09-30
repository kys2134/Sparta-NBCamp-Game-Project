using UnityEngine;

namespace Backend.Util.Extension
{
    public static class LayerMaskExtension
    {
        /// <summary>
        /// Remove a specific layer from the layer mask.
        /// </summary>
        /// <param name="layerMask">Layer mask to apply.</param>
        /// <param name="layer">Number of layer to remove.</param>
        /// <returns>Applied layer mask.</returns>
        public static LayerMask Remove(this LayerMask layerMask, int layer)
        {
            if ((layerMask & (1 << layer)) != 0)
            {
                layerMask &= ~(1 << layer);
            }

            return layerMask;
        }
    }
}
