using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_DirectionalDamageReduction : Effect
{
    [Header("Applies directional damage reduction to target. The direction is defined by the orign point. (origin-target)")]
    [Tooltip("How long does it last")]
    public Value Duration = new Value();
    [Tooltip("How much does it multiply damage from that angle ")]
    public Value Multiplier = new Value();
    [Tooltip("Width of the angle")]
    public Value Angle = new Value();
    [Tooltip("Stacking")]
    public bool Stack = false;

    public string StatusID = "DirectionalShield";

    public override bool HasLocalOnlyEffect()
    {
        return false;
    }

    public override bool HasServerEffect()
    {
        return false;
    }

    public override void LocalEffect(CallInfo callInfo)
    {
        base.LocalEffect(callInfo);
        if (callInfo.target == null || !callInfo.originPoint.HasValue)
            return;
        Status status = new Status()
        {
            type = Status.Type.DirectionalDamageReduction,
            StatusID = StatusID,
            duration = Duration,
            amount = Multiplier,
            AngleWidth = Angle,
            Direction = callInfo.originPoint.Value - callInfo.target.transform.position
        };
        status.AddToCharacter(callInfo, Stack);
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
        return false;
    }
}
