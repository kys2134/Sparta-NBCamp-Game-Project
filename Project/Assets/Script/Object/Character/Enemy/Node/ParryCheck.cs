using System.Collections;
using System.Collections.Generic;
using Backend.Util.Debug;
using UnityEngine;

namespace Backend.Object.Character.Enemy.Node
{
    public class ParryCheck : ActionNode
    {
        protected override State OnUpdate()
        {
            //그로기 발생
            
            return State.Success;
        }
        protected override void Start()
        {
            if (blackboard.IsParry)
            {
                Debugger.LogProgress("패링 됨");
            }
            else
            {
                Debugger.LogProgress("패링 실패");
                return;
            }
        }
        protected override void Stop()
        {
            blackboard.IsParry = false;
        }
    }
}

