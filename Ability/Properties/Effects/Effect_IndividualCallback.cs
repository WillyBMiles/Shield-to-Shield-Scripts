using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_IndividualCallback : Effect
{
    [Header("Does basically what projectiles do for hits, with any timing.")]
    public string TriggeringCallback = "Callback";
    public string TriggeringTargetID = "Individual";

    public bool Points = false;
    public bool Characters = true;

    public override void LocalEffect(CallInfo callInfo)
    {
        base.LocalEffect(callInfo);
        if (!callInfo.caster.IsAuthoritative())
            return;

        if (Characters)
            callInfo.caster.characterAbilities.AddTarget(callInfo.ability, callInfo.itemID, TriggeringTargetID, true, callInfo.target);
        if (Points && callInfo.targetPoint.HasValue)
            callInfo.caster.characterAbilities.AddTarget(callInfo.ability, callInfo.itemID, TriggeringTargetID, true, callInfo.targetPoint.Value);
        CallInfo newInfo = callInfo.Duplicate();
        newInfo.CallbackID = TriggeringCallback;

        callInfo.activation.OnCallback?.Invoke(newInfo);


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
        return true;
    }
}
