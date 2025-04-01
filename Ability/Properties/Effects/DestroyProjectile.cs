using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Dynamic;

[System.Serializable]
public class DestroyProjectile : Effect
{
	[Header("Destroys a projectile with this effectID from the caster")]
	public string EffectIDofProjectile;

	public override void LocalEffect(CallInfo callInfo)
	{
		base.LocalEffect(callInfo);
 		LocalProjectile.DestroyProjectile(callInfo.ability, callInfo.caster, EffectIDofProjectile);
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
        return true;
    }
}
