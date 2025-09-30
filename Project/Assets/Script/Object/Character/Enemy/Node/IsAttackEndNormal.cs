using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Backend.Object.Character.Enemy.Node
{
    public class IsAttackEndNormal : ActionNode
    {
        public int LayerNum = 0;
        [Range(0f, 1f)]
        public float ExitTime = 0.95f;

        protected override void Start()
        {
        }
        protected override void Stop()
        {
        }
        protected override State OnUpdate()
        {
            int tagHash = blackboard.currentAnimationHash;

            if (tagHash == 0)
            {
                return State.Success;
            }

            bool isCurrentState = agent.AnimationController.IsStatePlayingByTag(tagHash, LayerNum);

            if (isCurrentState)
            {
                float normalizedTime = agent.AnimationController.GetAnimationNormalByTag(tagHash, LayerNum);

                if(normalizedTime >= ExitTime)
                {
                    blackboard.currentAnimationHash = 0;
                    return State.Success;
                }
            }
            else
            {
                blackboard.currentAnimationHash = 0;
                return State.Success;
            }

            return State.Running;
        }
    }
}
