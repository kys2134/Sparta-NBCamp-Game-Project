using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Backend.Object.Character.Enemy.Node
{
    public class MoveToBackWard : ActionNode
    {
        public float StepSpeed = 15f;
        public float StepDistance = 10f;

        private NavMeshHit _hitPos;
        private bool _isArrive = false;
        private readonly int BackStep = Animator.StringToHash("S0107");
        private readonly int BackStepTag = Animator.StringToHash("BackStep");

        protected override void Start()
        {
            agent.NavMeshAgent.speed = StepSpeed;
            agent.NavMeshAgent.updateRotation = false;

            Vector3 direction = agent.MovementController.transform.forward * -1f;
            Vector3 targetPos = agent.MovementController.transform.position + (direction * StepDistance);

            if(NavMesh.SamplePosition(targetPos, out _hitPos, 1.0f, NavMesh.AllAreas))
            {
                agent.NavMeshAgent.SetDestination(_hitPos.position);
                agent.AnimationController.SetCrossFadeInFixedTime(BackStep, 0.1f);
            }
        }
        protected override void Stop()
        {
        }
        protected override State OnUpdate()
        {
            if (!_isArrive && !agent.NavMeshAgent.pathPending)
            {
                if (agent.NavMeshAgent.remainingDistance <= agent.NavMeshAgent.stoppingDistance)
                {
                    if (!agent.NavMeshAgent.hasPath || agent.NavMeshAgent.velocity.sqrMagnitude == 0f)
                    {
                        _isArrive = true;
                        agent.NavMeshAgent.ResetPath();
                    }
                }
            }

            if (_isArrive)
            {
                if (agent.AnimationController.GetAnimationNormalByTag(BackStepTag, 0) >= 0.8f)
                {
                    return State.Success;
                }
            }

            return State.Running;
        }
    }
}
