using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_FreezeFrame : Effect
{

    public Value time = new Value();

    public override void LocalEffect(CallInfo callInfo)
    {
        base.LocalEffect(callInfo);
        if (callInfo.target == null)
            return;
        callInfo.target.FreezeFrame(time.GetValue(callInfo));
    }


    public override bool CanHitCharacters()
    {
        return true;
    }

    public override bool CanHitPoints()
    {
        return false;
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

}
