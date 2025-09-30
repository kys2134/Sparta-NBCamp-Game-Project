using UnityEngine;

namespace Backend.Object.Character.Enemy.Node
{
    public class SetNineTailHumanAnimation : ActionNode
    {
        public float TransitionDuration = 0.1f;

        public bool PlayIdle = false;
        public bool PlayStrafe = false;
        public bool PlayDie = false;

        public bool SetRun = false;
        public bool SetWalk = false;

        public bool IsRun = false;
        public bool IsWalk = false;

        private readonly int _animationBoolRun = Animator.StringToHash("IsRun");
        private readonly int _animationBoolWalk = Animator.StringToHash("IsWalk");

        private readonly int _animStrafeLeft = Animator.StringToHash("StrafeLeft");
        private readonly int _animStrafeRight = Animator.StringToHash("StrafeRight");
        private readonly int _animationIdle = Animator.StringToHash("Idle");
        private readonly int _animationDie = Animator.StringToHash("Die");

        protected override void Start()
        {
            if (SetRun)
            {
                agent.AnimationController.SetAnimationBoolean(_animationBoolRun, IsRun);
            }
            if (SetWalk)
            {
                agent.AnimationController.SetAnimationBoolean(_animationBoolWalk, IsWalk);
            }
            if (PlayStrafe)
            {
                Vector3 targetRelativePoint = agent.MovementController.transform.InverseTransformPoint(agent.MovementController.Target.transform.position);
                if (targetRelativePoint.x <= 0)
                {
                    agent.AnimationController.SetCrossFadeInFixedTime(_animStrafeLeft, TransitionDuration);
                    blackboard.playerOnLeft = true;
                }
                else
                {
                    agent.AnimationController.SetCrossFadeInFixedTime(_animStrafeRight, TransitionDuration);
                    blackboard.playerOnLeft = false;
                }
            }
            if (PlayIdle)
            {
                agent.AnimationController.SetCrossFadeInFixedTime(_animationIdle, TransitionDuration);
            }
            if (PlayDie)
            {
                agent.AnimationController.SetCrossFadeInFixedTime(_animationDie, TransitionDuration);
            }
        }

        protected override State OnUpdate()
        {
            return State.Success;
        }

        protected override void Stop()
        {

        }
    }
}
