using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Effect_LootNearest : Effect
{
    public Effect_LootNearest()
    {
        TargetID = "SELF";
    }
    [Header("Origin character picks up from target point")]
    public bool overrideDistance = false;

    [ShowIf(nameof(overrideDistance))]
    public Value newDistance = new Value();

    public override void LocalOnlyEffect(CallInfo callInfo)
    {
        base.LocalOnlyEffect(callInfo);

        if (!callInfo.targetPoint.HasValue)
            return;

        GroundCoinItem gci;
        if (overrideDistance)
        {
            float dist = newDistance.GetValue(callInfo);
            gci = GroundCoinItem.GetClosestGCI(callInfo.targetPoint.Value, dist);
        }else
        {
            gci = GroundCoinItem.GetClosestGCI(callInfo.targetPoint.Value);
        }
        
        if (gci != null)
        {
            gci.PickUp(callInfo.origin);
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
