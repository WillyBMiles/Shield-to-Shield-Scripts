using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Limitation_RoomNumber : Limitation
{
    [Tooltip("How many new rooms need to be explored before you can use this ability again?")]
    public int NumberOfRooms = 1;

    [Tooltip("How should this be stored in the character?")]
    public string StoredFloatID = "RoomsLeftToExplore";

    [Tooltip("Should it be stored character wide?")]
    public bool StoreFloatCharacterWide = false;

    [ShowIf(nameof(StoreFloatCharacterWide))]
    public bool SaveFloat = false;

    [Tooltip("Can you cast at start?")]
    public bool CanCastAtStart = true;
    public override bool CanCast(CallInfo callInfo)
    {
        float numberOfRooms;
        float defaultFloat = CanCastAtStart ? 0f : NumberOfRooms;
        numberOfRooms = callInfo.caster.CheckStoredFloat(callInfo.ability, callInfo.itemID, StoredFloatID, defaultFloat, StoreFloatCharacterWide, SaveFloat);
            
        return numberOfRooms <= 0f;
    }

    public override void Activate(CallInfo callInfo)
    {
        base.Activate(callInfo);
        callInfo.caster.StoreFloat(callInfo.ability, callInfo.itemID, StoredFloatID, NumberOfRooms, StoreFloatCharacterWide, SaveFloat);

    }

    static bool hasSubscribed = false;
    public override void Initialize(Activation activation)
    {
        base.Initialize(activation);
        if (!hasSubscribed)
        {
            Room.ExploreAnyRoom += Explored;
            hasSubscribed = true;
        }
    }
    void LocalExplore(Character character, Activation activation, Ability ability, string itemID)
    {
        float numberOfRooms;
        float defaultFloat = CanCastAtStart ? 0f : NumberOfRooms;
        numberOfRooms = character.CheckStoredFloat(ability, itemID, StoredFloatID, defaultFloat, StoreFloatCharacterWide, SaveFloat);
            

        float newNumber = Mathf.Max(0f, numberOfRooms - 1f);
        character.StoreFloat(ability, itemID, StoredFloatID, newNumber, StoreFloatCharacterWide, SaveFloat);

            
    }

    static void Explored()
    {
        foreach (Character c in Static.I.allCharacters.Values)
        {
            foreach (CharacterAbilities.AbilityEntry ae in c.characterAbilities.abilityEntries)
            {
                foreach (Activation act in ae.actualAbility.activations)
                {
                    foreach (Limitation lim in act.limitations)
                    {
                        if (lim is Limitation_RoomNumber lrm){
                            lrm.LocalExplore(c, act, ae.actualAbility, ae.item);
                        }
                    }
                }
            }
        }
    }

    
}
