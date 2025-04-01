using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Limitation_InCombat : Limitation
{
    public override bool CanCast(CallInfo callInfo)
    {
        return CombatRoomController.InCombat;
    }
}
