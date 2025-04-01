using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target_AITarget : Target
{
    //Puts both target and point
    protected override void TargetActivate(CallInfo callInfo)
    {
        if (callInfo.caster.enemyAI == null || callInfo.caster.enemyAI.target == null)
            return;
        AddTargets(callInfo, ClearTargets, callInfo.caster.enemyAI.target);
        AddTargets(callInfo, ClearTargets, callInfo.caster.enemyAI.targetPoint) ;
    }

    public override bool IsDeterministic()
    {
        return false;
    }
}
