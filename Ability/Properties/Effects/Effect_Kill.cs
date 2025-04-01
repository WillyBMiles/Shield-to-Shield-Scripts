using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Dynamic;

[System.Serializable]
public class Effect_Kill : Effect
{
	[Tooltip("Also cancels all on death effects. Doesnt work if not marked DeleteOnDeath")]
	public bool DontShowDeathAnimation = false;

	public bool CanKillUnkillable = false;

	public override void ServerEffect(CallInfo callInfo)
	{
		base.ServerEffect(callInfo);
		if (callInfo.target == null)
			return;
        callInfo.target.Die(DontShowDeathAnimation, CanKillUnkillable);
	}

    public override bool HasLocalOnlyEffect()
    {
        return false;
    }

    public override bool HasServerEffect()
    {
        return true;
    }

    protected override bool AbstractHasLocalEffect()
    {
        return false;
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
