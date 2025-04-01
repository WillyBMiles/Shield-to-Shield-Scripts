using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_ExtraItemModel : Effect
{
    public Effect_ExtraItemModel()
    {
        TargetID = "SELF";
    }

    [Header("Will replace same body part, same item if possible")]
    public GameObject ExtraItemModel;
    public EquipSlot equipSlot;



    public override void LocalEffect(CallInfo callInfo)
    {
        base.LocalEffect(callInfo);
        if (callInfo.caster == null || callInfo.item == null)
            return;
        LocalShowEquipped lse = callInfo.caster.localShowEquipped;
        if (lse == null)
            return;
        lse.ShowExtraModel(callInfo.item.uniqueIdentifier, ExtraItemModel, equipSlot);
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
