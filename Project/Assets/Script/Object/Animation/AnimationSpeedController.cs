using UnityEngine;

namespace Backend.Object.Animation
{
    public class AnimationSpeedController : StateMachineBehaviour
    {
        public AnimationCurve speed = AnimationCurve.Linear(0f, 1f, 1f, 1f);

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var normalized = stateInfo.normalizedTime % 1f;

            animator.speed = speed.Evaluate(normalized);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.speed = 1f;
        }
    }
}
