using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugFailNode : ActionNode
{
    public string Message;
    public int FailTargetNum;
    private int _failNum = 0;
    protected override void Start()
    {

    }

    protected override void Stop()
    {
    }

    protected override State OnUpdate()
    {
        Debug.Log($"OnUpdate{Message}");
        if(_failNum < FailTargetNum)
        {
            _failNum++;
            return State.Failure;
        }
        else
        {
            return State.Success;
        }
    }
}
