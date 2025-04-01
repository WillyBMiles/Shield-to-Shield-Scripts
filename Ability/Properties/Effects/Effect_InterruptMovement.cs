using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Effect_InterruptMovement : Effect
{
	public override void LocalEffect(CallInfo callInfo)
	{
		base.LocalEffect(callInfo);

		if (callInfo.target != null)
			callInfo.target.InterruptMovement();
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
