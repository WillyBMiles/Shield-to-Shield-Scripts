using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitationBlock : Limitation
{
    public Type type;

    public enum Type
    {
        IsBlocking,
        CanBlock
    }

    public override bool CanCast(CallInfo callInfo)
    {
        if (callInfo.caster.characterBlocking == null)
            return false;
        switch (type)
        {
            case Type.IsBlocking:
                return callInfo.caster.characterBlocking.IsBlocking;
            case Type.CanBlock:
                return callInfo.caster.characterStamina.Stamina >= 0f;
        }
        return true;
    }
}
