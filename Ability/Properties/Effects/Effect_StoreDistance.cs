using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_StoreDistance : Effect
{
    [Header("Stores the distance between origin and target. Is always ability-wide.")]
    public string StoredFloatID = "Distance";

    public override void LocalEffect(CallInfo callInfo)
    {
        base.LocalEffect(callInfo);

        Vector3 v1 = callInfo.targetPoint ?? callInfo.target.transform.position;
        Vector3 v2 = callInfo.originPoint ?? callInfo.origin.transform.position;

        float distance = Vector3.Distance(v1, v2);

        if (!float.IsInfinity(distance))
            callInfo.caster.StoreFloat(callInfo.ability, callInfo.itemID, StoredFloatID, distance, false, false);
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
