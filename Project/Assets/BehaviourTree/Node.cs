using Backend.Object.Character.Enemy;
using UnityEngine;

public abstract class Node : ScriptableObject
{
    public enum State
    {
        Running,
        Failure,
        Success
    }

    [HideInInspector] public State mState = State.Running;
    [HideInInspector] public bool Started = false;
    [HideInInspector] public string Guid;
    [HideInInspector] public Vector2 Position;
    [HideInInspector] public Blackboard blackboard;
    [HideInInspector] public BehaviourTree tree;
    [HideInInspector] public EnemyComponent agent;
    [TextArea] public string description;

    public State Update()
    {
        if (!Started)
        {
            Start();
            Started = true;
        }

        mState = OnUpdate();

        if (mState == State.Failure || mState == State.Success)
        {
            Stop();
            Started = false;
        }

        return mState;
    }

    public virtual Node Clone()
    {
        return Instantiate(this);
    }

    public void Abort()
    {
        tree.Traverse(this, (node) =>
        {
            node.Started = false;
            node.mState = State.Running;
            node.Stop();
        });
    }

    protected abstract void Start();

    protected abstract void Stop();

    protected abstract State OnUpdate();
}
