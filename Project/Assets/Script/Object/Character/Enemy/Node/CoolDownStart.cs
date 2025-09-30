using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Backend.Object.Character.Enemy.Node
{
    public class CoolDownStart : ActionNode
    {
        public string SkillId;
        protected override State OnUpdate()
        {
            agent.CombatController.ActionCoolTimer[SkillId].Start();
            return State.Success;
        }
        protected override void Start()
        {

        }
        protected override void Stop()
        {

        }
    }
}
