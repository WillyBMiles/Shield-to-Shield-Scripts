using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Effect_PointTowards : Effect
{
    [Header("Points the origin character towards the target")]
    [Tooltip("If true it will target character instead of point")]
	public bool TargetCharacter = false;

	public override void LocalEffect(CallInfo callInfo)
	{
		base.LocalEffect(callInfo);

		if (callInfo.origin == null || (TargetCharacter && callInfo.target == null))
			return;
		if (!callInfo.targetPoint.HasValue && !TargetCharacter)
			return;
		Vector3 newPoint = TargetCharacter ? callInfo.target.transform.position : callInfo.targetPoint.Value;
		if (newPoint != callInfo.origin.transform.position)
			callInfo.origin.PointTowards(newPoint);
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
        return TargetCharacter;
    }

    public override bool CanHitPoints()
    {
        return !TargetCharacter;
    }
}
