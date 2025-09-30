using UnityEngine;


public class TimeoutNode : DecoratorNode
{
    [Tooltip("Returns failure after this amount of time if the subtree is still running.")]
    public float duration = 1.0f;
    
    private float _time;

    protected override void Start()
    {
        _time = Time.time;
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

        if (Time.time - _time > duration)
        {
            return State.Failure;
        }

        return Child.Update();
    }
}
