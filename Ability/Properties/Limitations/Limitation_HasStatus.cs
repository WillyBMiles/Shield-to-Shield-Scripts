using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Limitation_HasStatus : Limitation
{
    public List<Status.Type> statusTypes = new List<Status.Type>();
    [HideIf("@statusTypes.Count < 2")]
    [Tooltip("If set to false, target must have all statusTypes")]
    public bool Any = true;

    [ValueDropdown(nameof(_targets))]
    public string TargetID = "SELF";

    static List<Character> tempCs = new List<Character>();
    public override bool CanCast(CallInfo callInfo)
    {
        if (tempCs == null)
            tempCs = new List<Character>();
        callInfo.caster.characterAbilities.GetTargets(callInfo, tempCs, TargetID);
        if (tempCs.Count == 0)
            return false;
        foreach (Character c in tempCs)
        {
            bool gottenAny = false;
            foreach (Status.Type t in statusTypes)
            {
                if (!c.HasStatus(t))
                {
                    if (!Any)
                        return false;
                }
                else
                {
                    if (Any)
                        gottenAny = true;
                }
            }
            if (gottenAny == false)
                return false;
        }
        return true;
    }
}
