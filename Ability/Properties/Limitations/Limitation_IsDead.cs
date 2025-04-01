using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Limitation_IsDead : Limitation
{
    [ValueDropdown(nameof(_targets))]
    public string TargetID = "SELF";
    static List<Character> tempTargets = new List<Character>();

    public override bool CanCast(CallInfo callInfo)
    {
        if (tempTargets == null)
            tempTargets = new List<Character>();
        tempTargets.Clear();
        callInfo.caster.characterAbilities.GetTargets(callInfo, tempTargets, TargetID);
        foreach (Character c in tempTargets)
        {
            if (c.Dead)
                return true;
        }
        return false;
    }
}
