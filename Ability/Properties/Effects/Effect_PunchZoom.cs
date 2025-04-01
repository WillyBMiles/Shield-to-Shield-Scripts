using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_PunchZoom : Effect
{

    public Value amount = new Value();
    [Tooltip("How long to delay until unzoom happens.")]
    public Value delay = new Value();
    public override void LocalEffect(CallInfo callInfo)
    {
        base.LocalEffect(callInfo);
        CameraControls.PunchZoom(amount.GetValue(callInfo), delay.GetValue(callInfo));
    }

    public override bool CanHitCharacters()
    {
        return true;
    }

    public override bool CanHitPoints()
    {
        return true;
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
