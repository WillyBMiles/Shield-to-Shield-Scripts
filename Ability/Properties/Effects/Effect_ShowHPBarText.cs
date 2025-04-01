using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class Effect_ShowHPBarText : Effect
{
    public string text;
    public Color color;
    [Tooltip("How long to wait until it disappears.")]
    public Value delay = new Value();

    public override void LocalEffect(CallInfo callInfo)
    {
        base.LocalEffect(callInfo);
        if (callInfo.target != null && callInfo.target.extraHPBarText != null)
        {
            callInfo.target.extraHPBarText.ShowText(text, delay.GetValue(callInfo), color);
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
