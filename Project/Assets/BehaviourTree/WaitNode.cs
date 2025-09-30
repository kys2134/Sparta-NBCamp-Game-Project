using UnityEngine;

public class WaitNode : ActionNode
{
    public float WaitTime = 1f;
    private float _startTime;
    protected override void Start()
    {
        _startTime = Time.time;
    }

    protected override void Stop()
    {

    }

    protected override State OnUpdate()
    {
        if(Time.time - _startTime >= WaitTime)
        {
            return State.Success;
        }
        
        return State.Running;
    }
}
