using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_PickupItem : Effect
{
    public Effect_PickupItem()
    {
        TargetID = "SELF";
    }
    public Value pickupDistance = new Value();

    public override void LocalOnlyEffect(CallInfo callInfo)
    {
        base.LocalOnlyEffect(callInfo);
        if (callInfo.caster == null || !callInfo.caster.IsAuthoritative() || callInfo.caster.playerItems == null || !callInfo.targetPoint.HasValue)
            return;
        ItemGroundBehaviour igb = ItemGroundBehaviour.GetClosest(callInfo.targetPoint.Value);
        if (igb != null && Vector3.Distance(igb.transform.position, callInfo.targetPoint.Value) < pickupDistance.GetValue(callInfo))
        {
            igb.pickedUp = true;
            callInfo.caster.playerItems.CmdPickUpItem(igb);
        }

        
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
        return false;
    }

    public override bool CanHitPoints()
    {
        return true;
    }
}
