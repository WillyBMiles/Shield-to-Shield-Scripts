using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Effect_Items : Effect
{
    public Effect_Items()
    {
        TargetID = "SELF";
    }

    [Tooltip("Unequip all items")]
    public bool UnequipAll;
    
    [Tooltip("Cant equip items")]
    [HideIf(nameof(CanEquipAgain))]
    public bool CantEquip;
    [Tooltip("Cleanses the 'Cant equip items' effect")]
    [HideIf(nameof(CantEquip))]
    public bool CanEquipAgain;

    [ShowIf("@CanEquipAgain || CantEquip")]
    [Tooltip("ID for can/can't equip. Make sure they match the effect that disables/reenables item equip")]
    public string EquipID;

    static List<string> tempitems = new List<string>();
    public override void ServerEffect(CallInfo callInfo)
    {
        if (callInfo.target == null || callInfo.target.playerItems == null)
            return;

        
        if (UnequipAll)
        {
            if (tempitems == null)
                tempitems = new List<string>();
            tempitems.Clear();
            tempitems.AddRange(callInfo.target.playerItems.equippedItems);
            foreach (string i in tempitems)
            {
                callInfo.target.playerItems.CmdDequipItem(i);
            }
        }
        if (CantEquip)
            callInfo.target.playerItems.AddCantEquipItems(EquipID);
        if (CanEquipAgain)
            callInfo.target.playerItems.RemoveCantEquipItems(EquipID);
    }

    public override bool HasLocalOnlyEffect()
    {
        return false;
    }

    public override bool HasServerEffect()
    {
        return true;
    }

    protected override bool AbstractHasLocalEffect()
    {
        return false;
    }

    public override bool CanHitCharacters()
    {
        return true;
    }

    public override bool CanHitPoints()
    {
        return false;
    }
}
