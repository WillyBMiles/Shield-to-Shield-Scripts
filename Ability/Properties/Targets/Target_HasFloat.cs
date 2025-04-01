using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Target_HasFloat : Target
{
    [Header("Point origins, character targets.")]
    [Tooltip("if unchecked, filter out from a specific search")]
    public bool TargetAllCharacters = true;

    [ValueDropdown(nameof(_targets))]
    [HideIf(nameof(TargetAllCharacters))]
    [Tooltip("Which targetID")]
    public string PreviousTargetID = "SELF";

    [Tooltip("Max number")]
    public Value Number = new Value();

    public TargetMask targetMask;

    public string StoredFloatID = "";
    public Value equals = new Value();

    public bool CharacterWide = true;
    [ShowIf(nameof(CharacterWide))]
    public bool SavedFloat = false;


    static List<Character> targets;
    static List<Character> overrideTargets;
    static List<Vector3> origins;
    static List<Character> finalTargets;
    protected override void TargetActivate(CallInfo callInfo)
    {
        if (origins == null)
            origins = new List<Vector3>();
        if (targets == null)
            targets = new List<Character>();
        if (overrideTargets == null)
            overrideTargets = new List<Character>();
        if (finalTargets == null)
            finalTargets = new List<Character>();

        callInfo.caster.characterAbilities.GetTargets(callInfo, origins, OriginTargetID);
        if (TargetAllCharacters)
        {
            targets.Clear();
            targets.AddRange(Static.I.allCharacters.Values);
        }else
        {
            callInfo.caster.characterAbilities.GetTargets(callInfo, targets, PreviousTargetID);
        }
        if (origins.Count == 0)
            return;

        float maxDistance = MaxDistance.GetValue(callInfo);
        callInfo.caster.GetAllInRange(origins[0], overrideTargets,
            maxDistance, targetMask, CanTargetUntargettable, CanTargetTrulyUntargettable);
        finalTargets.Clear();
        float minDistance = MinDistance.GetValue(callInfo);
        float maxNumber = Number.GetValue(callInfo);
        float test = equals.GetValue(callInfo);
        float hasTest = test - 1;
        foreach (Character c in overrideTargets)
        {
            if (finalTargets.Count >= maxNumber)
                break;

            if (!targets.Contains(c))
                continue;
            if (Vector3.Distance(c.transform.position, callInfo.caster.transform.position) < minDistance)
                continue;
            float getValue = c.CheckStoredFloat(callInfo.ability, callInfo.itemID, StoredFloatID, hasTest, CharacterWide, SavedFloat);
            if (getValue != test)
                continue;
            finalTargets.Add(c);
        }

        AddTargets(callInfo, ClearTargets, finalTargets.ToArray());

    }

    public override bool IsDeterministic()
    {
        return true;
    }
}
