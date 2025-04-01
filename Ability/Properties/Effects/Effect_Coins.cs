using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Coins : Effect
{
    public Effect_Coins()
    {
        TargetID = "SELF";
    }

    public Value changeAmount = new Value();

    public override void LocalEffect(CallInfo callInfo)
    {
        base.LocalEffect(callInfo);
        if (callInfo.target == null)
            return;
        if (!callInfo.target.IsAuthoritative() || callInfo.target.playerItems == null)
            return;

        callInfo.target.playerItems.CmdChangeCoins((long)changeAmount.GetValue(callInfo));
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
