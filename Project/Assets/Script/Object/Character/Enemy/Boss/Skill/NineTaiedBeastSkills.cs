using System.Collections.Generic;
using Backend.Object.Management;
using Backend.Util.Data.ActionDatas;
using UnityEngine;
using UnityEngine.AI;

namespace Backend.Object.Character.Enemy.Boss.Skill
{
    // 스킬 실행에 필요한 데이터
    [System.Serializable]
    public struct SkillParameter
    {
        public ActionBossData SkillData; // 스킬 ID
        public GameObject ProjectilePrefab; // 발사체 프리팹
        public Transform[] ProjectileTransform; // 발사체 생성 위치 및 방향
        public Transform MovePosition; // 이동 목표 위치
        public float MoveDuration; // 움직이는 시간
        public bool IsPlayerTarget; // 플레이어를 타겟으로 하는지 여부
    }

    public class NineTaiedBeastSkills : MonoBehaviour
    {
        public SkillParameter[] SkillParameter;

        private Dictionary<string, SkillParameter> _skillParametersDict;

        private NavMeshAgent _navMeshAgent;
        private EnemyStatus _enemyStatus;
        private EnemyMovementController _movementController;

        private GameObject _projectilePoolObject;

        private void Awake()
        {
            // SkillParameter 배열을 딕셔너리로 변환하여 빠른 접근 가능
            _skillParametersDict = new Dictionary<string, SkillParameter>();
            foreach (var skillParam in SkillParameter)
            {
                _skillParametersDict[skillParam.SkillData.ID] = skillParam;
            }
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _enemyStatus = GetComponent<EnemyStatus>();
            _movementController = GetComponent<EnemyMovementController>();
            _projectilePoolObject = new GameObject("ProjectilePool");
        }

        // 발사체 생성 메서드
        public void SpawnProjectile(string skillID)
        {
            if (_skillParametersDict.TryGetValue(skillID, out var skillParam))
            {
                foreach (var projectileTransform in skillParam.ProjectileTransform)
                {
                    var projectile = ObjectPoolManager.SpawnPoolObject(skillParam.ProjectilePrefab, projectileTransform.position, projectileTransform.rotation, _projectilePoolObject.transform);

                    if (projectile != null)
                    {
                        var baseProjectile = projectile.GetComponent<BaseProjectile>();
                        if (baseProjectile != null)
                        {
                            baseProjectile.Init(_enemyStatus.BossStatus.Damage * skillParam.SkillData.Damage);
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning($"Skill ID {skillID} not found!");
            }
        }

        // 이동 메서드
        public void MoveToPosition(string skillID)
        {
            if (_skillParametersDict.TryGetValue(skillID, out var skillParam))
            {
                if (skillParam.MoveDuration <= 0f)
                {
                    Debug.LogWarning("duration가 0보다 커야함");
                    return;
                }

                // 목표 위치 계산 (현재 위치 + 이동 벡터)
                Vector3 targetPosition;

                // 플레이어를 타겟으로 하는 경우, 플레이어 위치로 설정
                if (skillParam.IsPlayerTarget)
                {

                    targetPosition = _movementController.Target.transform.position - (_movementController.GetDirection() * -5f);

                }
                else
                {
                    targetPosition = skillParam.MovePosition.position;
                }

                // 이동 거리 계산
                float distance = (targetPosition - transform.position).magnitude;


                // 필요한 속도 계산 (속도 = 거리 / 시간)
                float requiredSpeed = distance / skillParam.MoveDuration;

                // NavMeshAgent 속성 설정
                _navMeshAgent.speed = requiredSpeed;
                _navMeshAgent.SetDestination(targetPosition);
            }
            else
            {
                Debug.LogWarning($"Skill ID {skillID} not found!");
            }
        }

        // 플레이어와 충돌 꺼짐
        public void DisablePlayerCollision()
        {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Player"), true);
        }

        // 플레이어와 충돌 켜짐
        public void EnablePlayerCollision()
        {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Player"), false);
        }
    }
}
