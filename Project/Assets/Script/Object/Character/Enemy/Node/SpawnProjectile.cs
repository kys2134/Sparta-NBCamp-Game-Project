using System.Collections;
using System.Collections.Generic;
using Backend.Object.Management;
using Backend.Object.Projectile;
using Backend.Util.Debug;
using UnityEngine;

namespace Backend.Object.Character.Enemy.Node
{
    public class SpawnProjectile : ActionNode
    {
        public GameObject Projectile;
        public Vector3[] ProjectileSpawnPoint;

        public float Delay = 0.2f;
        public float Speed = 1f;
        public float Duration = 1f;
        public float ChasingTime = 0f;
        public float SpawnDelay = 0f;

        private float _timer;
        private int _spawnCount;
        private float _damage;
        protected override void Start()
        {
            _timer = 0f;
            _spawnCount = 0;
            _damage = agent.Status.BossStatus.Damage * agent.CombatController.ActionData.Damage;

            if (Duration < SpawnDelay || Duration < ChasingTime)
            {
                Debugger.LogError($"Need Set Duration Duration > SpawnDelay + ChasingTime, Now : {Duration} > {SpawnDelay} + {ChasingTime}");
            }
        }
        protected override void Stop() { }
        protected override State OnUpdate()
        {
            _timer += Time.deltaTime;

            if (_timer >= Delay && _spawnCount < ProjectileSpawnPoint.Length)
            {
                var spawnPoint = agent.MovementController.transform.TransformPoint(ProjectileSpawnPoint[_spawnCount]);
                GameObject spawnObj = ObjectPoolManager.SpawnPoolObject(Projectile, spawnPoint, Quaternion.identity, null);

                if (spawnObj.TryGetComponent<IProjectile>(out var projectileComponent))
                {
                    Transform target = agent.MovementController.Target.transform;
                    projectileComponent.Initialized(target, _damage, Speed, SpawnDelay, ChasingTime, Duration);
                }
                else
                {
                    Debugger.LogError($"{Projectile} is Not Have IPorjectile");
                }

                _spawnCount++;
                _timer = 0f;
            }

            if (_spawnCount >= ProjectileSpawnPoint.Length)
            {
                return State.Success;
            }

            return State.Running;
        }
    }
}
