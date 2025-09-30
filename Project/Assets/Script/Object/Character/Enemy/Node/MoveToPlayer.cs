using Backend.Util.Debug;
using UnityEngine;
using UnityEngine.AI;

namespace Backend.Object.Character.Enemy.Node
{
    public class MoveToPlayer : ActionNode
    {
        public float Speed = 5f;
        public float StopDistance = 2.0f;
        public float Offset = 2.0f;
        public bool EnableRotation = true;
        public bool EnableUseAnim = false;

        private Transform playerTransform;

        private readonly int _animRun = Animator.StringToHash("Run");
        private readonly int _animIdle = Animator.StringToHash("Idle");

        protected override void Start()
        {
            agent.NavMeshAgent.speed = Speed;
            agent.NavMeshAgent.stoppingDistance = StopDistance;

            if (agent.MovementController.Target != null)
            {
                if (EnableUseAnim)
                {
                    agent.AnimationController.SetCrossFadeInFixedTime(_animRun, 0.1f);
                }
                playerTransform = agent.MovementController.Target.transform;
            }
        }

        protected override void Stop()
        {
            if (EnableUseAnim)
            {
                agent.AnimationController.SetCrossFadeInFixedTime(_animIdle, 0.1f);
            }
        }

        protected override State OnUpdate()
        {
            if (playerTransform == null)
            {
                Debugger.LogError("PlayerTransform is NULL");
                return State.Failure;
            }

            float distance = agent.MovementController.Distance;

            if (distance <= StopDistance)
            {
                return State.Success;
            }

            Vector3 dir = agent.MovementController.GetDirection();
            Vector3 destination = playerTransform.position - (dir * Offset);
            agent.NavMeshAgent.SetDestination(destination);

            if (EnableRotation)
            {
                agent.MovementController.SetLerpRotation(agent.MovementController.transform.position, destination, Speed);
            }

            return State.Running;
        }
    }
}
