using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target_Animation : Target
{
    public Target_Animation()
    {
        timing = Timing.WindDown;
    }

    public string CallbackID = "Callback";
    public TargetMask mask = TargetMask.MyEnemies;

    public override bool IsDeterministic()
    {
        return true;
    }

    protected override void TargetActivate(CallInfo callInfo)
    {
        if (callInfo.caster.animationHitManager == null)
            return;
        foreach (AnimationHitManager.Hit hit in callInfo.caster.animationHitManager.hits)
        {
            Character target = Character.GetCharacter(hit.gameObject);
            if (target == null || !callInfo.caster.FitsMask(target, mask))
                continue;
            if (target.HasStatus(Status.Type.Untargettability) && !CanTargetUntargettable)
                continue;
            if (target.Untargettable && !CanTargetTrulyUntargettable)
                continue;

            CallInfo newInfo = callInfo.Duplicate();
            newInfo.CallbackID = CallbackID;
            newInfo.target = target;
            newInfo.targetPoint = hit.position;
            newInfo.originPoint = hit.position - hit.direction;

            callInfo.activation.OnCallback?.Invoke(newInfo);
        }
    }

}
