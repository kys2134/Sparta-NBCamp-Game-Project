using Backend.Object.Character.Enemy.Boss;
using UnityEngine.AI;

namespace Backend.Object.Character.Enemy
{
    public class EnemyComponent
    {
        public EnemyStatus Status { get; set; }
        public EnemyAnimationController AnimationController { get; set; }
        public EnemyMovementController MovementController { get; set; }
        public EnemyCombatController CombatController { get; set; }
        public NavMeshAgent NavMeshAgent { get; set; }
    }
}
