using Backend.Object.Character;
using Backend.Object.Management;
using UnityEngine;

// 리기드바디 생성
[RequireComponent(typeof(Rigidbody))]
public class BaseProjectile : MonoBehaviour
{
    private Rigidbody _rigidbody;

    // 파괴 여부
    [SerializeField] private bool isDestroyed = false;

    private float _damage;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false; // 중력 비활성화
    }

    public void Init(float damage)
    {
        _damage = damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<IDamagable>()?.TakeDamage(_damage);
            Debug.Log($"Player Hit! Damage: {_damage}");
            if (isDestroyed)
            {
                ObjectPoolManager.Release(gameObject);
            }
        }
    }
}
