using UnityEngine;
using UnityEngine.AI;

namespace Backend.Object.Character.Enemy.Node
{
    public class MoveToPlayer1 : ActionNode
    {
        public float Speed = 5f;
        public float StopDistance = 2.0f;
        public float Offset = 2.0f;
        public bool EnableRotation = true;

        private Transform playerTransform;

        protected override void Start()
        {
            agent.NavMeshAgent.speed = Speed;
            //agent.NavMeshAgent.stoppingDistance = StopDistance;

            if (agent.MovementController.Target != null)
            {
                playerTransform = agent.MovementController.Target.transform;
            }
        }

        protected override void Stop()
        {
        }

        protected override State OnUpdate()
        {
            if (playerTransform == null)
            {
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
