using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Limitation_OncePerFloor : Limitation
{
    public string StoredFloatID = "OncePerFloor";
    public bool StoreFloatCharacterWide = false;
    [ShowIf(nameof(StoreFloatCharacterWide))]
    public bool SaveFloat = false;


    static bool hasSubscribed = false;
    public override void Initialize(Activation activation)
    {
        base.Initialize(activation);
        if (!hasSubscribed)
        {
            Floor.Explore += NewFloor;
            hasSubscribed = true;
        }
    }
    public override void Activate(CallInfo callInfo)
    {
        base.Activate(callInfo);
        float newNumber = 0f;
        callInfo.caster.StoreFloat(callInfo.ability, callInfo.itemID, StoredFloatID, newNumber, StoreFloatCharacterWide, SaveFloat);
    }
    public override bool CanCast(CallInfo callInfo)
    {
        return callInfo.caster.CheckStoredFloat(callInfo.ability, callInfo.itemID, StoredFloatID, 0, StoreFloatCharacterWide, SaveFloat) == 1;

    }
    void LocalNewFloor(Character character, Ability ability, string itemID)
    {

        float newNumber = 1f;
        character.StoreFloat(ability, itemID, StoredFloatID, newNumber, StoreFloatCharacterWide, SaveFloat);
    }

    static void NewFloor()
    {
        foreach (CharacterAbilities ca in Static.I.characterAbilities)
        {
            foreach (var ae in ca.abilityEntries)
            {
                foreach (Activation act in Ability.GetAbility(ae.ability).activations)
                {
                    foreach (Limitation lim in act.limitations)
                    {
                        if (lim is Limitation_OncePerFloor opf)
                        {
                            opf.LocalNewFloor(ca.character, ae.actualAbility, ae.item);
                        }
                    }
                }
            }
        }
    }
}
