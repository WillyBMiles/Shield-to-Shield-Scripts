using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Lose : Effect
{
    public Effect_Lose()
    {
        TargetID = "SELF";
    }

    public override void ServerEffect(CallInfo callInfo)
    {
        base.ServerEffect(callInfo);
        CombatRoomController.LoseCurrentBattle();
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
