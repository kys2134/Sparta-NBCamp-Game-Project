using System.Collections;
using System.Collections.Generic;
using Backend.Object.Character;
using Backend.Object.Management;
using Backend.Util.Debug;
using UnityEngine;

namespace Backend.Object.Projectile
{
    public class FoxFireProjectile : MonoBehaviour, IProjectile
    {
        private Transform _target;
        private float _speed;
        private float _damage;
        private float _remainingTime;
        private float _chasingTime;
        private float _spawnDelay = 0f;
        private float _time = 0f;
        public LayerMask HitLayer;

        public void Initialized(Transform target, float damage, float speed, float spawnDelay, float chasingTime, float duration)
        {
            _target = target;
            _speed = speed;
            _damage = damage;
            _remainingTime = duration;
            _chasingTime = chasingTime;
            _spawnDelay = spawnDelay;
        }

        private void OnEnable()
        {
            _time = 0f;
        }

        private void Update()
        {
            _time += Time.deltaTime;

            if(_spawnDelay >= _time)
            {
                return;
            }

            if(_target != null && _chasingTime >= _time)
            {
                Vector3 direction = (_target.position - transform.position).normalized;

                transform.position += _speed * Time.deltaTime * direction;
                transform.rotation = Quaternion.LookRotation(direction);
            }
            else
            {
                transform.position += _speed * Time.deltaTime * transform.forward;
            }

            if(_remainingTime < _time)
            {
                ObjectPoolManager.Release(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent<IDamagable>(out var target) && other.gameObject.layer == (int)Mathf.Log(HitLayer.value, 2))
            {
                target.TakeDamage(_damage);
                Debugger.LogProgress($"{_damage}만큼 데미지를 가하였습니다.");
                ObjectPoolManager.Release(gameObject);
            }
        }
    }
}
