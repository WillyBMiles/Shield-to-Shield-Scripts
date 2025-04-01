using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlockInfo
{
    public List<Damage.Type> types = new List<Damage.Type>();
    public Damage.SubType subtypes = Damage.SubType.None;
    [Tooltip("Angle of the block")]
    public Value angle = new Value() { baseMult = 150f };
    [Tooltip("Speed multiplier of player while blocking")]
    public Value speedMultiplier = new Value() { baseMult = 1f };
    [Tooltip("Damage reduction while blocking")]
    public Value damageMultiplier = new Value() { baseMult = 0f };
    [Tooltip("Stagger damage reduction while blocking")]
    public Value staggerMultiplier = new Value() { baseMult = 0f };
    [Tooltip("How much stamina does it cost to block 100 damage")]
    public Value staminaCost = new Value() { baseMult = 1f };
}


public class Effect_Block : Effect
{
    public enum Type
    {
        StartBlock,
        EndBlock
    }
    [Header("Only the caster can block.")]
    public Type type = Type.StartBlock;
    public override void LocalEffect(CallInfo callInfo)
    {
        base.LocalEffect(callInfo);
        if (callInfo.caster == null || callInfo.item == null || callInfo.item.myItemGenerator.blockInfo == null || callInfo.caster.characterBlocking == null)
            return;
        switch (type)
        {
            case Type.StartBlock:
                callInfo.caster.characterBlocking.StartBlock(callInfo.item.uniqueIdentifier, callInfo);
                break;
            case Type.EndBlock:
                callInfo.caster.characterBlocking.EndBlock();
                break;
        }
    }

    public override bool CanHitCharacters()
    {
        return true;
    }

    public override bool CanHitPoints()
    {
        return false;
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
}
