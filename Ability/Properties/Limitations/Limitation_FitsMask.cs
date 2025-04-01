using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Limitation_FitsMask : Limitation
{
    [ValueDropdown(nameof(_targets))]
    public string TargetID = "SELF";

    public TargetMask mask = TargetMask.MyEnemies;


    static List<Character> tempCs = new List<Character>();
    public override bool CanCast(CallInfo callInfo)
    {
        if (tempCs == null)
        {
            tempCs = new List<Character>();
        }
        tempCs.Clear();
        callInfo.caster.characterAbilities.GetTargets(callInfo, tempCs, TargetID);

        foreach (Character c in tempCs)
        {
            if (!callInfo.caster.FitsMask(c, mask))
                return false;
        }
        return true;
    }
}
