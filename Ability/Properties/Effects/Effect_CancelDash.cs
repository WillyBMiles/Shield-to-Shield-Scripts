using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class Effect_CancelDash : Effect
{
    public override void LocalEffect(CallInfo callInfo)
    {
        base.LocalEffect(callInfo);
        if (callInfo.target != null)
        {
            callInfo.target._isDashing = false;
        }
            
    }
    public override void ServerEffect(CallInfo callInfo)
    {
        base.ServerEffect(callInfo);
        if (callInfo.target != null)
        {
            callInfo.target.isDashing = false;
            callInfo.target._isDashing = false;
        }
            

    }

    public override bool HasLocalOnlyEffect()
    {
        return false;
    }

    public override bool HasServerEffect()
    {
        return true;
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
