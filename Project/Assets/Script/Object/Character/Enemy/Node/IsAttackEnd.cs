using UnityEngine;

namespace Backend.Object.Character.Enemy.Node
{
    public class IsAttackEnd : ActionNode
    {
        protected override void Start()
        {
        }
        protected override void Stop()
        {
        }
        protected override State OnUpdate()
        {
            blackboard.attackTimeCounter += Time.deltaTime;
            if (blackboard.attackTimeCounter >= agent.AnimationController.GetAnimationClipLength(0))
            {
                blackboard.attackTimeCounter = 0;
                return State.Success;
            }
            return State.Running;
        }
    }
}
