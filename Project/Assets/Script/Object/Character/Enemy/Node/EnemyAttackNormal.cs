using Backend.Util.Data.ActionDatas;
using UnityEngine;
using Backend.Object.Character.Enemy;

namespace Backend.Object.Character.Enemy.Node
{
    public class EnemyAttackNormal : ActionNode
    {
        public int AttackNum;
        public string HashTag;
        private int _hashTag;
        private EnemyCombatController combat;
        private ActionBossData attack;

        protected override void Start()
        {
            if (combat == null)
            {
                combat = agent.CombatController;
            }

            if (_hashTag == 0)
            {
                _hashTag = Animator.StringToHash(HashTag);
            }
        }
        protected override void Stop()
        {
        }
        protected override State OnUpdate()
        {
            attack = combat.ActionDatas[AttackNum];
            combat.ActionData = attack;

            blackboard.currentAnimationHash = _hashTag;

            int SkillName = Animator.StringToHash(attack.ID);
            agent.AnimationController.SetCrossFadeInFixedTime(SkillName, 0.1f);

            return State.Success;
        }
    }
}
