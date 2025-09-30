using Backend.Util.Data.ActionDatas;

namespace Backend.Object.Character.Enemy.Node
{
    public class IsPlayerInRange : ActionNode
    {
        public float minDistance = 0f;
        public float maxDistance = 5f;

        // RangeCheck 열거형을 사용할지 여부에 대한 변수
        public bool useRangeCheck = false;
        public RangeCheck rangeCheck = RangeCheck.None;

        protected override void Start()
        {
        }
        protected override void Stop()
        {
        }
        protected override State OnUpdate()
        {
            float distance = agent.MovementController.Distance; // 플레이어와의 거리 계산

            if (useRangeCheck)
            {

                int index = (int)rangeCheck;

                float maxDis = agent.Status.BossStatus.AttackRange[index];
                float minDis = index == 0 ? 0f : agent.Status.BossStatus.AttackRange[index - 1];

                bool isInRange = distance > minDis && distance <= maxDis;

                if (isInRange)
                {
                    blackboard.RangeCheck = rangeCheck;
                    return State.Success;
                }
                else
                {
                    return State.Failure;
                }
            }
            else
            {
                if (distance >= minDistance && distance < maxDistance)
                {
                    return State.Success;
                }
                else
                {
                    return State.Failure;
                }
            }
        }
    }
}
