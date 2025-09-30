using UnityEngine;

namespace Backend.Object.Character.Player.Animation
{
    public class RollingStateMachine : StateMachineBehaviour
    {
        public AnimationCurve speed;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var controller = animator.GetComponentInParent<AdvancedActionController>();

            controller?.OnRollingStateEntered();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var controller = animator.GetComponentInParent<AdvancedActionController>();
            var normalized = stateInfo.normalizedTime % 1f;

            controller.rollingSpeed = speed.Evaluate(normalized);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var controller = animator.GetComponentInParent<AdvancedActionController>();

            controller.rollingSpeed = 1f;
            controller.OnRollingStateExited();
        }
    }
}
