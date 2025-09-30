using UnityEngine;

public class SucceedNode : DecoratorNode
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
        if (state == State.Failure)
        {
            return State.Success;
        }
        return state;
    }
}
