using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target_FightVariationGroup : Target
{
    static List<Character> tempCharacters;
    public enum MatchType
    {
        SameGroupAsMe,
        NotSameGroupAsMe
    }

    public MatchType match = MatchType.SameGroupAsMe;

    protected override void TargetActivate(CallInfo callInfo)
    {
        if (tempCharacters == null)
        {
            tempCharacters = new List<Character>();
        }
        if (callInfo.caster.Group == -1)
            return;
        tempCharacters.Clear();
        foreach (Character c in Static.I.allCharacters.Values)
        {
            if (c.IsPlayerCharacter)
            {
                switch (match)
                {
                    case MatchType.SameGroupAsMe:
                        if (callInfo.caster.Group == c.Group)
                            tempCharacters.Add(c);
                        break;
                    case MatchType.NotSameGroupAsMe:
                        if (callInfo.caster.Group != c.Group)
                            tempCharacters.Add(c);
                        break;
                }
            }
        }
        AddTargets(callInfo, ClearTargets, tempCharacters.ToArray());
    }

    public override bool IsDeterministic()
    {
        return true;
    }
}
