using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Backend.Object.Projectile
{
    public interface IProjectile
    {
        public void Initialized(Transform target, float damage, float speed, float spawnDelay, float chasingTime, float duration);
    }
}
