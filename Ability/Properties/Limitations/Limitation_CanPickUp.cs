using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Limitation_CanPickUp : Limitation
{
    public override bool CanCast(CallInfo callInfo)
    {
        return GroundCoinItem.CanPickupAny();
    }
}
