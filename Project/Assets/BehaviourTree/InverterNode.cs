public class InverterNode : DecoratorNode
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

        switch (Child.Update())
        {
            case State.Running:
                return State.Running;
            case State.Failure:
                return State.Success;
            case State.Success:
                return State.Failure;
        }
        return State.Failure;
    }
}
