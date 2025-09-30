using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Backend.Object.Character.Enemy.Node
{
    public class SetNavMesh : ActionNode
    {
        public bool IsNavMesh = false;
        protected override State OnUpdate()
        {
            agent.NavMeshAgent.isStopped = IsNavMesh;
            return State.Success;
        }

        protected override void Start()
        {

        }
        protected override void Stop() { }
    }
}
