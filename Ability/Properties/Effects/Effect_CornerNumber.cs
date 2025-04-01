using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class Effect_CornerNumber : Effect
{
    [HideIf(nameof(Hide))]
    public Value showValue = new Value();

    public bool Hide = false;


    public override void LocalOnlyEffect(CallInfo callInfo)
    {
        base.LocalOnlyEffect(callInfo);

        callInfo.caster.StoreFloat(callInfo.ability, callInfo.itemID, Ability.SHOWCORNERNUMBER, Hide ? 0f : 1f, false, false);
        if (!Hide)
            callInfo.caster.StoreFloat(callInfo.ability, callInfo.itemID, Ability.CORNERNUMBER, (int) showValue.GetValue(callInfo), false, false);
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
