using Backend.Util.Data.ActionDatas;
using UnityEngine;
using Backend.Object.Character.Enemy;

namespace Backend.Object.Character.Enemy.Node
{
    public class EnemyAttack : ActionNode
    {
        [Tooltip("For attack array in enemy combat controller")]
        public int AttackNum;
        public bool IsRandomAttack;
        public int MinRandomIndex = 0;
        public int RandomLength;
        EnemyCombatController combat;

        protected override void Start()
        {
            if (combat == null)
            {
                combat = agent.CombatController;
            }
            blackboard.attackTimeCounter = 0f;
        }
        protected override void Stop()
        {
        }
        protected override State OnUpdate()
        {
            ActionBossData attack;
            if (IsRandomAttack)
            {
                attack = combat.ActionDatas[Random.Range(MinRandomIndex, RandomLength)]; // 랜덤 공격 선택

            }
            else
            {
                attack = combat.ActionDatas[AttackNum]; // 지정된 공격 선택
            }
            combat.ActionData = attack;
            int SkillName = Animator.StringToHash(attack.ID);
            agent.AnimationController.SetCrossFadeInFixedTime(SkillName, 0.1f); // 공격 애니메이션 재생

            Debug.Log($"Enemy Attack: {attack.ID}");
            
            //넉백 처리
            //if (attack.force > 1)
            //{
            //    agent.forceReceiver.AddForce();
            //}

            blackboard.attackTimeCounter += Time.deltaTime;
            return State.Success;
        }
    }
}
