using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Win : Effect
{
    public enum WinType
    {
        AllPlayers,
        JustMe,
        Groups
    }

    public WinType winType = WinType.AllPlayers;
    public bool ClearEnemies = false;
    public override void ServerEffect(CallInfo callInfo)
    {
        base.ServerEffect(callInfo);
        CombatRoomController.WinCurrentBattle(ClearEnemies); 
    }

    protected override bool AbstractHasLocalEffect()
    {
        return false;
    }

    public override bool HasLocalOnlyEffect()
    {
        return false;
    }

    public override bool HasServerEffect()
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
