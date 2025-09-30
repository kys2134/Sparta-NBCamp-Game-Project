using UnityEngine;

namespace Backend.Object.Character.Enemy.Node
{
    public class ActionStrafe : ActionNode
    {
        // 최소 시간, 최대 시간
        public float MinStrafeDuration = 1.0f;
        public float MaxStrafeDuration = 3.0f;

        public float MoveSpeed = 15.0f; // 이동 속도
        public float RotationSpeed = 270f; // 회전 속도 

        private float _strafeDuration;
        private float _desiredDistance; // 플레이어와의 원하는 거리
        private float _timer;
        private int _direction; // -1 for left, 1 for right
        private Transform _playerTransform; // 실제로는 플레이어 정보를 가져와야 함
        protected override void Start()
        {
            _timer = 0f;
            _direction = (Random.value > 0.5f) ? 1 : -1; // 이동 방향을 랜덤으로 결정

            _playerTransform = agent.MovementController.Target.transform; // 플레이어의 Transform을 가져옴

            agent.NavMeshAgent.speed = MoveSpeed;
            agent.NavMeshAgent.angularSpeed = RotationSpeed;
            _strafeDuration = Random.Range(MinStrafeDuration, MaxStrafeDuration);

            agent.NavMeshAgent.isStopped = false;
        }
        protected override void Stop()
        {
            // context.agent.ResetPath(); // 이동을 멈추기 위해 경로 초기화
            agent.NavMeshAgent.ResetPath();
        }
        protected override State OnUpdate()
        {
            _timer += Time.deltaTime;
            if (_timer > _strafeDuration)
            {
                return State.Success; // 정해진 시간이 지나면 행동 성공
            }

            // 보스가 항상 플레이어를 바라보게 만드는 부분
            // context.transform.LookAt(playerTransform);
            _desiredDistance = agent.MovementController.Distance; // 현재 플레이어와의 거리
            // 다음 이동 지점을 계산
            Vector3 nextPoint = CalculateStrafePoint(_playerTransform, agent.MovementController.transform, _direction, _desiredDistance);

            // 해당 지점으로 이동 명령
            agent.NavMeshAgent.SetDestination(nextPoint);

            return State.Running; // 이동이 완료될 때까지 Running 상태 유지
        }

        private Vector3 CalculateStrafePoint(Transform player, Transform boss, int strafeDirection, float distanceFromPlayer, float stepAngle = 15.0f)
        {
            Vector3 directionFromPlayerToBoss = (boss.position - player.position).normalized; // 플레이어에서 보스로 향하는 방향 벡터
            Quaternion rotation = Quaternion.AngleAxis(stepAngle * strafeDirection, Vector3.up); // Y축을 기준으로 회전
            Vector3 rotatedDirection = rotation * directionFromPlayerToBoss; // 회전된 방향 벡터
            Vector3 nextPosition = player.position + rotatedDirection * distanceFromPlayer; // 플레이어로부터 일정 거리 떨어진 지점 계산
            return nextPosition;
        }
    }
}
