using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Limitation_Armor : Limitation
{
    public enum Type
    {
        ArmorFull,
    }
    public Type type = Type.ArmorFull;

    public override bool CanCast(CallInfo callInfo)
    {
        if (callInfo.caster.characterArmor == null)
            return false;
        switch (type)
        {
            case Type.ArmorFull:
                return callInfo.caster.characterArmor.AreArmorsFull();
        }

        return true;
    }
}
