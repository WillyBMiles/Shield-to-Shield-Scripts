using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target_MoveDirection : Target
{
    public Value distance = new Value();

    public override bool IsDeterministic()
    {
        return true;
    }

    protected override void TargetActivate(CallInfo callInfo)
    {
        if (callInfo.caster.lastPosition == callInfo.caster.transform.position)
            AddTargets(callInfo, true, callInfo.caster.transform.position);
        else
        {
            AddTargets(callInfo, true, callInfo.caster.transform.position + 
                (callInfo.caster.transform.position - callInfo.caster.lastPosition).normalized * distance.GetValue(callInfo));
        }
    }
}
