using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Stamina : Effect
{
    public Effect_Stamina()
    {
        TargetID = "SELF";
    }

    public Value changeAmount = new Value();
    public override void LocalEffect(CallInfo callInfo)
    {
        base.LocalEffect(callInfo);
        if (callInfo.target != null && callInfo.target.characterStamina != null)
        {
            callInfo.target.characterStamina.UseStamina(-changeAmount.GetValue(callInfo), true);
        }
    }





    public override bool CanHitCharacters()
    {
        return true;
    }

    public override bool CanHitPoints()
    {
        return false;
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
