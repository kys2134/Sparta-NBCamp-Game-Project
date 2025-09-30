using UnityEngine;

public class DebugLogNode : ActionNode
{
    public string Message;
    protected override void Start()
    {
        Debug.Log($"OnStart{Message}");
    }

    protected override void Stop()
    {
        Debug.Log($"OnStop{Message}");
    }

    protected override State OnUpdate()
    {
        Debug.Log($"OnUpdate{Message}");
        return State.Success;
    }
}
