using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EffectSetOutlineTimer : Effect
{

    public Value timer = new Value();

    [Tooltip("If set to true, only updates the outline timer when you can cast it")]
    public bool OnlyUpdateIfCanCast = false;

    [Tooltip("If you dont want to use the outline as a timer")]
    public bool DontTick = false;
    [Tooltip("Keep whatever the last max was")]
    public bool DontSetMax = false;

    protected override void UpdateThis(CallInfo callInfo)
    {
        base.UpdateThis(callInfo);
        if (DontTick)
            return;
        if (callInfo.caster.IsAuthoritative())
        {
            if (!OnlyUpdateIfCanCast || callInfo.activation.CanCast(callInfo, true))
            {
                float old = callInfo.caster.CheckStoredFloat(callInfo.ability, callInfo.itemID, Ability.OUTLINETIMER, 0f, false,false);
                callInfo.caster.StoreFloat(callInfo.ability, callInfo.itemID, Ability.OUTLINETIMER, old - Time.deltaTime, false, false);
            }
            
        }
            
    }

    public override void LocalOnlyEffect(CallInfo callInfo)
    {
        base.LocalOnlyEffect(callInfo);
        float time = timer.GetValue(callInfo);
        callInfo.caster.StoreFloat(callInfo.ability, callInfo.itemID, Ability.OUTLINETIMER, time, false, false);
        if (!DontSetMax)
            callInfo.caster.StoreFloat(callInfo.ability, callInfo.itemID, Ability.MAXOUTLINETIMER, time == 0 ? 1f : time, false, false);
           
    }

    public override bool HasLocalOnlyEffect()
    {
        return true;
    }

    public override bool HasServerEffect()
    {
        return false;
    }

    protected override bool AbstractHasLocalEffect()
    {
        return false;
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
