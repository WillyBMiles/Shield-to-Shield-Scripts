using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class Effect_Convert : Effect
{
    public Value Time = new Value();
    
    public override void LocalEffect(CallInfo callInfo)
    {
        base.LocalEffect(callInfo);

        if (callInfo.target == null)
            return;
        callInfo.target.Convert(callInfo.caster, Time.GetValue(callInfo));
    }

    public override bool HasLocalOnlyEffect()
    {
        return false;
    }

    public override bool HasServerEffect()
    {
        return false;
    }

    protected override bool AbstractHasLocalEffect()
    {
        return true;
    }

    public override bool CanHitCharacters()
    {
        return true;
    }

    public override bool CanHitPoints()
    {
        return false;
    }
}
