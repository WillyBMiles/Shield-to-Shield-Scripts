using UnityEngine;
using Sirenix.OdinInspector;

public class Effect_Armor : Effect
{
    public Effect_Armor()
    {
        TargetID = "SELF";
    }

    public Value armorChange = new Value();

    public bool allowOverArmor = false;

    [Tooltip("Only change armor given.")]
    public bool targetSpecificArmor = false;

    public bool TargetCaster = false;

    [ShowIf(nameof(targetSpecificArmor))]
    public Trait armorType = Trait.HeavyArmor;

    public override void LocalOnlyEffect(CallInfo callInfo)
    {
        base.LocalEffect(callInfo);
        Character tar = TargetCaster ? callInfo.caster : callInfo.target;
        if (tar.characterArmor == null)
            return;
        tar.characterArmor.ChangeArmor(armorChange.GetValue(callInfo), targetSpecificArmor, allowOverArmor, armorType);
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
        return true;
    }

    public override bool HasServerEffect()
    {
        return false;
    }

    protected override bool AbstractHasLocalEffect()
    {
        return false;
    }
}
