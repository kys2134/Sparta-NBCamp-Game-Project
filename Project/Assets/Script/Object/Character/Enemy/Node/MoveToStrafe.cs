using UnityEngine;

namespace Backend.Object.Character.Enemy.Node
{
    public class MoveToStrafe : ActionNode
    {
        public float StrafeSpeed;
        public float ForwardBackwardSpeed = 1.0f;
        public float MaxDistance;
        public float MinDistance;

        private float _duration;
        private float _direction;
        private float _timer;

        private readonly int _animStrafeRight = Animator.StringToHash("StrafeRight");
        private readonly int _animStrafeLeft = Animator.StringToHash("StrafeLeft");

        protected override State OnUpdate()
        {
            if (_timer >= _duration)
            {
                return State.Success;
            }

            agent.MovementController.SetRotation();

            Vector3 movement = _direction * StrafeSpeed * Time.deltaTime * agent.MovementController.transform.right;

            Vector3 forwardBackwardMovement = Vector3.zero;
            if (agent.MovementController.Target != null)
            {
                Transform target = agent.MovementController.Target.transform;
                float distanceToTarget = agent.MovementController.Distance;

                if (distanceToTarget > MaxDistance)
                {
                    forwardBackwardMovement = ForwardBackwardSpeed * Time.deltaTime * agent.MovementController.transform.forward;
                }
                else
                {
                    forwardBackwardMovement = -ForwardBackwardSpeed * Time.deltaTime * agent.MovementController.transform.forward;
                }
            }

            agent.MovementController.transform.position += movement + forwardBackwardMovement;

            _timer += Time.deltaTime;

            return State.Running;
        }
        protected override void Start()
        {
            _duration = Random.Range(1.0f, 2.5f);
            _direction = (Random.value > 0.5f) ? 1.0f : -1.0f;

            if (_direction > 0) // 오른쪽으로 이동
            {
                agent.AnimationController.SetCrossFadeInFixedTime(_animStrafeRight, 0.1f);
            }
            else // 왼쪽으로 이동
            {
                agent.AnimationController.SetCrossFadeInFixedTime(_animStrafeLeft, 0.1f);
            }

            _timer = 0f;
        }
        protected override void Stop()
        {

        }
    }
}
