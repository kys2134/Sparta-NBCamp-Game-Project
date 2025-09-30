using UnityEngine;

public class RandomSelectorNode : CompositeNode
{
    protected int current;

    protected override void Start()
    {
        current = Random.Range(0, Children.Count);
    }

    protected override void Stop()
    {
    }

    protected override State OnUpdate()
    {
        Node child = Children[current];
        return child.Update();
    }
}
