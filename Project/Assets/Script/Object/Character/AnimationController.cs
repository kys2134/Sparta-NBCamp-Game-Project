using UnityEngine;

namespace Backend.Object.Character
{
    //[RequireComponent(typeof(Animator))]
    public class AnimationController : MonoBehaviour
    {
        protected Animator Animator;

        protected virtual void Awake()
        {
            Animator = GetComponent<Animator>();
        }

        public void SetAnimationTrigger(string value)
        {
            Animator.SetTrigger(value);
        }

        public void SetAnimationBoolean(int name, bool value)
        {
            Animator.SetBool(name, value);
        }

        public void SetAnimationBoolean(string id, bool value)
        {
            Animator.SetBool(id, value);
        }

        public void SetCrossFadeInFixedTime(int name, float value)
        {
            Animator.CrossFadeInFixedTime(name, value);
        }

        public void SetAnimationInteger(string name, int value)
        {
            Animator.SetInteger(name, value);
        }

        public void SetAnimationFloat(string name, float value)
        {
            Animator.SetFloat(name, value);
        }

        public string GetCurrentAnimationName(int layer)
        {
            var information = Animator.GetCurrentAnimatorStateInfo(layer);
            var clips = Animator.runtimeAnimatorController.animationClips;

            var name = string.Empty;
            var length = clips.Length;
            for (var i = 0; i < length; i++)
            {
                if (information.IsName(clips[i].name))
                {
                    name = clips[i].name;
                }
            }

            return name;
        }

        public float GetCurrentAnimationNormalizedTime(int layer)
        {
            var information = Animator.GetCurrentAnimatorStateInfo(layer);
            var time = information.normalizedTime;

            return time;
        }

        public float GetCurrentAnimationLength(int layer)
        {
            var information = Animator.GetCurrentAnimatorStateInfo(layer);
            var length = information.length;

            return length;
        }
    }
}
