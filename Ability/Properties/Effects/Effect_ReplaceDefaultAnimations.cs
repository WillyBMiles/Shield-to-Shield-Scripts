using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_ReplaceDefaultAnimations : Effect
{
    public Effect_ReplaceDefaultAnimations()
    {
        TargetID = "SELF";
    } 

    public List<DefaultAnimation> defaultAnimations = new List<DefaultAnimation>();
    public List<AnimationClip> clipsToReplaceWith = new List<AnimationClip>();


    public override void LocalEffect(CallInfo callInfo)
    {
        base.LocalEffect(callInfo);
        if (callInfo.target == null)
            return;
        AnimationController ac = callInfo.target.animationController;
        if (ac == null)
            return;
        if (defaultAnimations.Count != clipsToReplaceWith.Count)
            return;
        for (int i = 0; i < defaultAnimations.Count; i++)
        {
            ac.ReplaceDefaultAnimation(defaultAnimations[i], clipsToReplaceWith[i]);
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
        return true;
    }

    public override bool CanHitPoints()
    {
        return false;
    }
}
