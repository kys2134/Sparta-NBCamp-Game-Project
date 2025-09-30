using Backend.Object.Character;
using Backend.Object.Character.Enemy;
using Backend.Object.Character.Enemy.Boss;
using UnityEngine;
using UnityEngine.AI;

public class BehaviourTreeRunner : MonoBehaviour
{
    public BehaviourTree Tree;
    public bool stop;

    private void Awake()
    {
        var component = new EnemyComponent()
        {
            Status = GetComponent<EnemyStatus>(),
            AnimationController = GetComponent<EnemyAnimationController>(),
            MovementController = GetComponent<EnemyMovementController>(),
            CombatController = GetComponent<EnemyCombatController>(),
            NavMeshAgent = GetComponent<NavMeshAgent>()
        };
        Tree = Tree.Clone();
        Tree.Bind(component);
    }

    private void Update()
    {
        if (stop)
        {
            return;
        }
        Tree.Update();
    }
}

