using UnityEngine;

namespace Backend.Object.Character.Player
{
    public class RaycastExcluder
    {
        #region CONSTANT FIELD API

        private const string IgnoreRaycastLayerName = "Ignore Raycast";

        #endregion
        
        private readonly Collider[] _colliders;

        private int[] _origins;
        
        public RaycastExcluder(Collider[] colliders)
        {
            _colliders = colliders;
            
            _origins = new int[colliders.Length];
        }

        /// <summary>
        /// Temporarily applies the layers of game objects excluded from ray cast to the ‘Ignore Raycast’ layer.
        /// </summary>
        public void Apply()
        {
            Patch();

            var length = _colliders.Length;
            for (var i = 0; i < length; i++)
            {
                if (_colliders[i] == null)
                {
                    continue;
                }
                
                _origins[i] = _colliders[i].gameObject.layer;
                _colliders[i].gameObject.layer = LayerMask.NameToLayer(IgnoreRaycastLayerName);
            }
        }

        /// <summary>
        /// Restores the layers of game objects excluded from the ray cast to their original layers.
        /// </summary>
        public void Restore()
        {
            var length = _colliders.Length;
            for (var i = 0; i < length; i++)
            {
                if (_colliders[i] == null)
                {
                    continue;
                }
                
                _colliders[i].gameObject.layer = _origins[i];
            }
        }

        /// <summary>
        /// If the size of the collider list to exclude from ray cast changes, update the array.
        /// </summary>
        private void Patch()
        {
            if (_origins.Length == _colliders.Length)
            {
                return;
            }
                
            _origins = new int[_colliders.Length];
        }

        public static int ExcludeLayer => LayerMask.NameToLayer(IgnoreRaycastLayerName);
    }
}