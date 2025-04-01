using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Target_All : Target
{
    public bool All = false;

    [HideIf(nameof(All))]
    public Faction faction;

    public override bool IsDeterministic()
    {
        return true;
    }

    protected override void TargetActivate(CallInfo callInfo)
    {
        if (All)
        {
            foreach (Character c in Static.I.allCharacters.Values)
            {
                AddTargets(callInfo, false, c);
            }
            
        }
        else 
            AddTargets(callInfo, ClearTargets, Static.I.allCharactersByFaction[faction].ToArray());
    }
}
