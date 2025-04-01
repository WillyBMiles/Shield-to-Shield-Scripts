using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Knockback : Effect
{
    public Effect_Knockback()
    {
        TargetID = "HIT";
    }
    
    public Value knockbackSpeed = new Value() { baseMult = 40f };
    public bool canKnockThroughCharacters = true;
    public bool canKnockThroughWalls = false;
    [Tooltip("Don't interrupt already in progress dashes.")]
    public bool dontInterrupt;

    [Tooltip("Instead of pushing directly away, push in pointing direction.")]
    public bool overrideWithFacingDirection = false;

    [Tooltip("Use for things like on hit effects so that only the caster can call this.")]
    public bool waitForCommand;

    public Value offsetMultiplier = new Value();
    [Tooltip("+x is to the right of origin, -x is to the left. +z is directly away from origin, -z is directly towards origin." +
        " If origin = target, it's from their local transform")]
    public Vector3 offset = new Vector3();

    public override void LocalEffect(CallInfo callInfo)
    {
        base.LocalEffect(callInfo);
        if (callInfo.target == null || callInfo.target.Unmoveable)
            return;
        bool nonauth = callInfo.caster.IsAuthoritative() && !callInfo.target.IsAuthoritative();
        if (!callInfo.caster.IsAuthoritative() && waitForCommand)
            return;

        if (dontInterrupt && (callInfo.target.isDashing || callInfo.target._isDashing))
            return;

        Vector3 zDirection = callInfo.target == callInfo.caster || overrideWithFacingDirection ? callInfo.caster.transform.forward : (callInfo.target.transform.position - callInfo.originPoint.Value).normalized;
        Vector3 xDirection = callInfo.target == callInfo.caster || overrideWithFacingDirection ? callInfo.caster.transform.right : Vector3.Cross(zDirection, Vector3.down).normalized;
        float offsetMult = offsetMultiplier.GetValue(callInfo);
        Vector3 dashTarget = callInfo.target.transform.position + offsetMult *( zDirection * offset.z +
              xDirection * offset.x +  Vector3.up * offset.y);

        

        float speed = knockbackSpeed.GetValue(callInfo);
        callInfo.target.CantDashThroughCharacters = !canKnockThroughCharacters;
        callInfo.target.CantDashThroughWalls = !canKnockThroughWalls;
        if (nonauth)
        {
            callInfo.target.NonAuthoritativeDash(dashTarget, speed, callInfo.caster.ID, waitForCommand);
        }
        else
        {
            callInfo.target.Dash(dashTarget, speed, callInfo.caster.ID);
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
