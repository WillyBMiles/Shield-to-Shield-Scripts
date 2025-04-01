using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_CleanseStatus : Effect
{
    public string StatusID;

    public List<string> StatusIDs = new List<string>();

    public override void LocalEffect(CallInfo callInfo)
    {
        base.LocalEffect(callInfo);
        if (callInfo.target == null)
            return;
        callInfo.target.RemoveStatus(StatusID);
        if (StatusIDs == null)
            return;
        foreach (string status in StatusIDs)
        {
            callInfo.target.RemoveStatus(status);
        }
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
