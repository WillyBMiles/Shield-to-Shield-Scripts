using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_ShowTextHere : Effect
{
    public bool TargetCharacters = false;
    public Color Color = new Color(0f, 0f, 0f, 1f);
    public int FontSize = 1;
    public string text;
    public Value yOffset = new Value();

    public Sprite showSprite = null;

    public override void LocalEffect(CallInfo callInfo)
    {
        base.LocalEffect(callInfo);
        if (TargetCharacters)
        {
            if (callInfo.target == null)
                return;
            ShowText.ShowTextHere(Color, text, callInfo.target.transform.position + Vector3.up * yOffset.GetValue(callInfo), 2f, FontSize, showSprite);

        }else
        {
            if (callInfo.targetPoint.HasValue)
                ShowText.ShowTextHere(Color, text, callInfo.targetPoint.Value + Vector3.up * yOffset.GetValue(callInfo), 2f, FontSize, showSprite);
        }
    }

    public override bool CanHitCharacters()
    {
        return TargetCharacters;
    }

    public override bool CanHitPoints()
    {
        return !TargetCharacters;
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
