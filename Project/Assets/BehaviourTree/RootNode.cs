using UnityEngine;

public class RootNode : Node
{
    public Node Child;
    protected override void Start()
    {
    }

    protected override void Stop()
    {
    }

    protected override State OnUpdate()
    {
        return Child.Update();
    }

    public override Node Clone()
    {
        RootNode node = Instantiate(this);
        node.Child = Child.Clone();
        return node;
    }
}
