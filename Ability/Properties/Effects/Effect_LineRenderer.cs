using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Effect_LineRenderer : Effect
{

    
    public Material material;

    [Tooltip("Tile material horizontally[x direction] based on line length")]
    public bool tileMaterial = true;
    public bool UsePointAsOrigin;
    public Value height = new Value();
    public Value width = new Value();

    public bool MatchAllProjectilesFromThisAbility = true;
    [HideIf(nameof(MatchAllProjectilesFromThisAbility))]
    public string ProjectileEffectID = "";

    public override void LocalEffect(CallInfo callInfo)
    {
        base.LocalEffect(callInfo);
        if (UsePointAsOrigin)
        {
            LineRendererController.CreateLRProjectileTrack(callInfo.originPoint.Value,callInfo,this);
        }else
        {
            if (callInfo.origin != null)
                LineRendererController.CreateLRProjectileTrack(callInfo.origin.gameObject, callInfo, this);
        }
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

    public override bool CanHitCharacters()
    {
        return false;
    }

    public override bool CanHitPoints()
    {
        return true;
    }
}
