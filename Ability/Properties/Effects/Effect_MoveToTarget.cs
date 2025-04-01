using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Effect_MoveToTarget : Effect
{
	[Tooltip("Move towards a target character instead of point.")]
	public bool TowardsCharacter = false;

	[Tooltip("Move target to origin")]
	public bool SwitchOriginAndTarget;

	public override void LocalEffect(CallInfo callInfo)
	{
		base.LocalEffect(callInfo);
		Character actualOrigin = callInfo.origin;
		Character actualTargetCharacter = callInfo.target;
		Vector3 actualTargetPoint = callInfo.targetPoint ?? callInfo.target.transform.position;
		if (SwitchOriginAndTarget)
        {
			actualTargetCharacter = callInfo.origin;
			actualTargetPoint = callInfo.originPoint ?? callInfo.origin.transform.position;
			actualOrigin = callInfo.target;
        }
		if (actualOrigin == null)
			return;

		if (!actualOrigin.IsAuthoritative())
			return;

		if (callInfo.caster == actualOrigin && callInfo.caster != null && callInfo.caster.HasStatus(Status.Type.CantInputMoveCommands))
        {
			return;
        }

		Vector3 newPoint = actualTargetPoint;
		if (TowardsCharacter && actualTargetCharacter != null)
		{
			newPoint = actualTargetCharacter.transform.position;
		}
		if (float.IsInfinity(newPoint.x))
			return;


		if (actualOrigin != null)
			actualOrigin.MoveTo(newPoint, true);
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
		return TowardsCharacter || SwitchOriginAndTarget;
	}

	public override bool CanHitPoints()
	{
		return !TowardsCharacter;
	}

}
