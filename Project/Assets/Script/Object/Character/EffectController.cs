using System.Collections;
using System.Collections.Generic;
using Backend.Object.Management;
using Backend.Util.Debug;
using UnityEngine;

namespace Backend.Object.Character
{
    public class EffectController : MonoBehaviour
    {
        public GameObject EffectPrefab;

        private GameObject _effect;

        public void SpawnEffect()
        {
            if (EffectPrefab == null)
            {
                Debugger.LogError("Effect is NULL");
            }

            _effect = ObjectPoolManager.SpawnPoolObject(EffectPrefab, transform.position, Quaternion.identity, null);
        }

        public void ReleaseEffect()
        {
            ObjectPoolManager.Release(_effect);
        }
    }
}
