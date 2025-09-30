using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Backend.Object.Character.Enemy.Node
{
    public class ActionDodgeSide : ActionNode
    {
        [Tooltip("회피 시의 이동 속도")]
        public float DodgeSpeed = 15.0f;

        [Tooltip("회피할 거리")]
        public float DodgeDistance = 5.0f;

        // 계산된 위치가 NavMesh 위에 있는지 확인하고, 있다면 그곳으로 이동
        private NavMeshHit _hit;
        protected override void Start()
        {
            agent.NavMeshAgent.speed = DodgeSpeed;
            agent.NavMeshAgent.isStopped = false;

            // -1 또는 1을 랜덤하게 선택하여 회피 방향 결정 (왼쪽/오른쪽)
            int direction = (Random.value > 0.5f) ? 1 : -1;

            // 현재 위치에서 측면 방향으로 dodgeDistance만큼 떨어진 위치 계산
            Vector3 dodgeDirection = agent.MovementController.transform.right * direction;
            Vector3 targetPosition = agent.MovementController.transform.position + dodgeDirection * DodgeDistance;


            if (NavMesh.SamplePosition(targetPosition, out _hit, 1.0f, NavMesh.AllAreas))
            {
                agent.NavMeshAgent.SetDestination(_hit.position);
            }
            else
            {
                // 만약 목표 위치가 NavMesh 위가 아니라면, 그냥 현재 위치의 측면으로 이동 시도
                agent.NavMeshAgent.SetDestination(targetPosition);
            }

            // TODO: "DodgeSide" 애니메이션 재생
            // context.animator.SetTrigger("Dodge");
        }

        protected override State OnUpdate()
        {
            // 목적지에 도달했는지 확인, pathPending은 경로 계산 중인지 여부
            if (!agent.NavMeshAgent.pathPending)
            {
                // 남은 거리가 stoppingDistance보다 작거나 같으면 도착한 것으로 간주
                if (agent.NavMeshAgent.remainingDistance <= agent.NavMeshAgent.stoppingDistance)
                {
                    // 도착했으나 경로가 아직 남아있거나, 속도가 남아있는 경우를 대비해 확실히 멈춤
                    if (!agent.NavMeshAgent.hasPath || agent.NavMeshAgent.velocity.sqrMagnitude == 0f)
                    {
                        return State.Success;
                    }
                }
            }

            // 아직 이동 중이면 Running 상태 반환
            return State.Running;
        }

        protected override void Stop()
        {
            agent.NavMeshAgent.ResetPath();
        }
    }
}
