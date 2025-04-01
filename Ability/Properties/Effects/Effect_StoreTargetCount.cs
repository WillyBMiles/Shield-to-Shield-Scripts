using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class Effect_StoreTargetCount : Effect
{
    [Tooltip("What StoredFloatID should we store it as")]
    public string StoredID = "";
    [Tooltip("Should we store it character wide or ability wide?")]
    public bool StoreCharacterWide = false;
    
    [ShowIf(nameof(StoreCharacterWide))]
    [Tooltip("Should we save it to the save-character?")]
    public bool SaveTargetCount = false;

    [Dropdown(nameof(_targets))]
    public string targetID = "SELF";

    [Tooltip("Defaults to counting up just the characters")]
    public bool CountUpPointsInstead = false;

    public override void Initialize(Activation activation)
    {
        base.Initialize(activation);
        overrideCharacters = new List<Character>();
        overridePoints = new List<Vector3>();
    }

    List<Character> overrideCharacters = new List<Character>();
    List<Vector3> overridePoints = new List<Vector3>();
    public override void LocalEffect(CallInfo callInfo)
    {
        base.LocalEffect(callInfo);
        int number;
        if (CountUpPointsInstead)
        {
            callInfo.caster.characterAbilities.GetTargets(callInfo, overridePoints, targetID);
            number = overridePoints.Count;
        }
        else
        {
            callInfo.caster.characterAbilities.GetTargets(callInfo, overrideCharacters, targetID);
            number = overrideCharacters.Count;
        }
        callInfo.caster.StoreFloat(callInfo.ability, callInfo.itemID, StoredID, number, StoreCharacterWide, SaveTargetCount);
        
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

    public override bool CanHitCharacters()
    {
        return true;
    }

    public override bool CanHitPoints()
    {
        return true;
    }
}
