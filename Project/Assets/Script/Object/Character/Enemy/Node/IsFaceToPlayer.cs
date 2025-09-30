using UnityEngine;

namespace Backend.Object.Character.Enemy.Node
{
    public class IsFaceToPlayer : ActionNode
    {
        // 오른쪽 왼쪽 애니메이션 회전을 위해 쓰는 노드입니다.
        // 플레이어를 "바라보고 있다"고 판단할 최소 방향 유사도(내적 값)입니다. 1에 가까울수록 정면입니다.
        public float minSimilarity = 0.99f;
        private readonly int _animStrafeLeft = Animator.StringToHash("StrafeLeft");
        private readonly int _animStrafeRight = Animator.StringToHash("StrafeRight");

        protected override void Start()
        {
        }
        // 노드가 중지될 때 호출됩니다.
        protected override void Stop()
        {
        }
        // 노드가 실행되는 동안 매 프레임 호출됩니다.
        protected override State OnUpdate()
        {
            Vector3 dir = agent.MovementController.GetDirection();
            dir.y = 0f;

            // 에이전트의 전방 벡터와 플레이어 방향 벡터의 내적(dot product)을 계산하여 방향 유사도를 구합니다.
            float similarity = Vector3.Dot(dir.normalized, agent.MovementController.transform.forward);
            // 유사도가 설정값보다 높으면 (즉, 플레이어를 거의 정면으로 보고 있으면) 성공(Success) 상태를 반환합니다.
            if (similarity > minSimilarity)
            {
                return State.Success;
            }
            // 플레이어를 바라보고 있지 않다면, 회전을 돕기 위한 스트레이프 애니메이션을 실행합니다.
            // 에이전트의 로컬 좌표계를 기준으로 플레이어의 상대 위치를 계산합니다.
            Vector3 targetRelativePoint = agent.MovementController.transform.InverseTransformPoint(agent.MovementController.Target.transform.position);
            // 이전에 플레이어가 왼쪽에 있었는지 확인합니다. (blackboard는 행동 트리 전체에서 공유되는 데이터 저장소)
            if (blackboard.playerOnLeft)
            {
                // 이전에 왼쪽에 있었는데, 이제 오른쪽에 있다면 (x > 0)
                if (targetRelativePoint.x > 0)
                {
                    // 오른쪽으로 도는 스트레이프 애니메이션을 실행합니다.
                    agent.AnimationController.SetCrossFadeInFixedTime(_animStrafeRight, 0.1f);
                    // 플레이어가 이제 오른쪽에 있다고 기록합니다.
                    blackboard.playerOnLeft = false;
                }
            }
            else // 이전에 플레이어가 오른쪽에 있었다면
            {
                // 이제 왼쪽에 있다면 (x <= 0)
                if (targetRelativePoint.x <= 0)
                {
                    // 왼쪽으로 도는 스트레이프 애니메이션을 실행합니다.
                    agent.AnimationController.SetCrossFadeInFixedTime(_animStrafeLeft, 0.1f);
                    // 플레이어가 이제 왼쪽에 있다고 기록합니다.
                    blackboard.playerOnLeft = true;
                }
            }
            // 아직 플레이어를 정면으로 바라보고 있지 않으므로 실패(Failure) 상태를 반환합니다.
            // 이 노드는 다음 틱에서 다시 평가됩니다.
            return State.Failure;
        }
    }
}
