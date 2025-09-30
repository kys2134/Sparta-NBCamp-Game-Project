using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Backend.Object.Character.Enemy
{
    public class EnemyAnimationController : AnimationController
    {
        protected override void Awake()
        {
            base.Awake();
        }

        public bool IsCurrentState(string stateName, int layer)
        {
            return Animator.GetCurrentAnimatorStateInfo(layer).IsName(stateName);
        }

        public bool IsTransition(int layer)
        {
            return Animator.IsInTransition(layer);
        }

        public float GetNextAnimationLength(int layer)
        {
            var information = Animator.GetNextAnimatorStateInfo(layer);
            return information.length;
        }

        public int GetCurrentNameHash(int layer)
        {
            var information = Animator.GetCurrentAnimatorStateInfo(layer);
            return information.shortNameHash;
        }

        /// <summary>
        ///  Animation Transition -> Get Next Animation Length, Animation Playing -> Get Now Animation Length
        /// </summary>
        public float GetAnimationClipLength(int layer) //태그
        {
            if (Animator.IsInTransition(layer))
            {
                AnimatorStateInfo nextStateInfo = Animator.GetNextAnimatorStateInfo(layer);
                return nextStateInfo.length;
            }
            else
            {
                AnimatorStateInfo currentStateInfo = Animator.GetCurrentAnimatorStateInfo(layer);
                return currentStateInfo.length;
            }
        }

        /// <summary>
        ///  Animation Transition -> Get Next Animation NormalizedTime, Animation Playing -> Get Now Animation NormalizedTime
        /// </summary>
        public float GetAnimationNormalize(int layer)
        {
            if (Animator.IsInTransition(layer))
            {
                AnimatorStateInfo nextStateInfo = Animator.GetNextAnimatorStateInfo(layer);
                return nextStateInfo.normalizedTime;
            }
            else
            {
                AnimatorStateInfo currentStateInfo = Animator.GetCurrentAnimatorStateInfo(layer);
                return currentStateInfo.normalizedTime;
            }
        }

        public float GetAnimationNormalByTag(int tagHash, int layer)
        {
            var currentStateInfo = Animator.GetCurrentAnimatorStateInfo(layer);

            if (Animator.IsInTransition(layer))
            {
                var nextStateInfo = Animator.GetNextAnimatorStateInfo(layer);
                if (nextStateInfo.tagHash == tagHash)
                {
                    return nextStateInfo.normalizedTime;
                }
            }

            if (currentStateInfo.tagHash == tagHash)
            {
                return currentStateInfo.normalizedTime;
            }

            return 0f;
        }

        public bool IsStatePlayingByTag(int tagHash, int layer)
        {
            var currentStateInfo = Animator.GetCurrentAnimatorStateInfo(layer);
            if(currentStateInfo.tagHash == tagHash)
            {
                return true;
            }

            if (Animator.IsInTransition(layer))
            {
                var nextStateInfo = Animator.GetNextAnimatorStateInfo(layer);
                if(nextStateInfo.tagHash == tagHash)
                {
                    return true;
                }
            }

            return false;
        }

        public void SetAnimationFloat(int hash, float value, float dampTime, float time)
        {
            Animator.SetFloat(hash, value, dampTime, time);
        }

        public void SetAnimatorSpeed(float speed)
        {
            Animator.speed = speed;
        }
    }
}
