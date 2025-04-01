using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_ChangeItemModel : Effect
{

    public GameObject newModel;
    public override void LocalEffect(CallInfo callInfo)
    {
        base.LocalEffect(callInfo);
        if (callInfo.caster == null || callInfo.item == null)
            return;
        LocalShowEquipped lse = callInfo.caster.localShowEquipped;
        if (lse == null)
            return;
        lse.ReplaceEquippedItem(callInfo.item.uniqueIdentifier, newModel);
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
