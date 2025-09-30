using System.Collections;
using System.Collections.Generic;
using Backend.Util.Debug;
using UnityEngine;

namespace Backend.Object.Character.Enemy.Node
{
    public class SetAttackAnimSpeed : ActionNode
    {
        public string HashTag;
        public int LayerNum = 0;
        public float ChangeSpeed = 1f;
        public float ReturnSpeed = 1f;

        [Range(0f, 1f)] public float ChangePoint = 0f;
        [Range(0f, 0.9f)] public float ReturnPoint = 0f;

        private int _hashTag;
        private bool _isSpeedChange;
        protected override void Start()
        {
            _hashTag = Animator.StringToHash(HashTag);
            _isSpeedChange = false;
            agent.AnimationController.SetAnimatorSpeed(1.0f);

            if(ReturnPoint <= ChangePoint)
            {
                Debugger.LogError($"ReturnPoint는 ChangePoint보다 수치가 높아야합니다.");
            }
        }
        protected override void Stop()
        {
            agent.AnimationController.SetAnimatorSpeed(1.0f);
        }
        protected override State OnUpdate()
        {
            if (agent.AnimationController.IsStatePlayingByTag(_hashTag, LayerNum))
            {
                float normalizeTime = agent.AnimationController.GetAnimationNormalByTag(_hashTag, LayerNum);
                if (!_isSpeedChange)
                {
                    if (normalizeTime >= ChangePoint)
                    {
                        agent.AnimationController.SetAnimatorSpeed(ChangeSpeed);
                        _isSpeedChange = true;
                    }
                }
                else
                {
                    if (normalizeTime >= ReturnPoint)
                    {
                        agent.AnimationController.SetAnimatorSpeed(ReturnSpeed);
                    }
                }
                return State.Running;
            }
            agent.AnimationController.SetAnimatorSpeed(1.0f);
            return State.Success;
        }
    }
}
