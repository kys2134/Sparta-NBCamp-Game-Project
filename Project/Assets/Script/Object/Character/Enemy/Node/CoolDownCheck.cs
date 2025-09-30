namespace Backend.Object.Character.Enemy.Node
{
    public class CoolDownCheck : ActionNode
    {
        public string ActionID;

        // 쿨다운이 아닐때 쿨다운 미리 시작할지 여부
        public bool IsPreStartCooldown = false;

        protected override State OnUpdate()
        {
            if (agent.CombatController.ActionData != null)
            {
                if (agent.CombatController.ActionCoolTimer[ActionID].IsCoolingDown)
                {
                    return State.Failure;
                }
                else
                {
                    if (IsPreStartCooldown)
                    {
                        agent.CombatController.ActionCoolTimer[ActionID].Start();
                    }
                    return State.Success;
                }
            }

            return State.Failure;
        }
        protected override void Start()
        {

        }
        protected override void Stop()
        {

        }
    }
}
