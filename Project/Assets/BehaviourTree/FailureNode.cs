using UnityEngine;

public class FailureNode : DecoratorNode
{
    protected override void Start()
    {
    }

    protected override void Stop()
    {
    }

    protected override State OnUpdate()
    {
        if (Child == null)
        {
            return State.Failure;
        }

        var state = Child.Update();
        if (state == State.Success)
        {
            return State.Failure;
        }
        return state;
    }
}
