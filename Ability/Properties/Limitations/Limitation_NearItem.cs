using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Limitation_NearItem : Limitation
{
    public Value distance = new Value(); 
    public override bool CanCast(CallInfo callInfo)
    {
        ItemGroundBehaviour igb = ItemGroundBehaviour.GetClosest(callInfo.caster.transform.position);
        if (igb == null)
            return false;
        return Vector3.Distance(igb.transform.position, callInfo.caster.transform.position) < distance.GetValue(callInfo);
    }
}
