using System.Collections;
using System.Collections.Generic;
using Backend.Object.Management;
using UnityEngine;

namespace Backend.Object.Projectile
{
    public class S0201Projectile : BaseProjectile
    {
        [SerializeField] private float durationTime = 0.5f;
        [SerializeField] private float moveDistance = 2.5f;

        private void OnEnable()
        {
            StartCoroutine(MoveToPosition(transform.position + (transform.forward * moveDistance), durationTime));
        }

        // 특정 위치로 살짝 이동하는 코루틴
        // duration은 이동에 걸리는 시간
        // targetPosition은 이동할 목표 위치
        private IEnumerator MoveToPosition(Vector3 targetPosition, float duration)
        {
            Vector3 startPosition = transform.position;
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                // 빠르게 이동하다가 거의 도착할 때 천천히 이동 효과
                float t = elapsedTime / duration;
                t = t * t * (3f - (2f * t)); // 스무딩 함수
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                elapsedTime += Time.deltaTime;

                yield return null;
            }
            transform.position = targetPosition;

            //yield return new WaitForSeconds(0.1f);
            ObjectPoolManager.Release(gameObject);
        }
    }
}

