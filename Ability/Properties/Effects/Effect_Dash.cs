using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Effect_Dash : Effect
{
	[Header("Dashes origin to target point")]
	[Tooltip("Speed in units per second")]
	public Value DashSpeed = new Value();

	[Header("Dash in a fixed time instead of at a fixed speed. DashSpeed is in seconds")]
	public bool DashSpeedIsTimeInstead = false;

	public bool CantDashThroughWalls = false;

	public bool CantDashThroughCharacters = false;
	[Tooltip("Use for things like on hit effects so that only the caster can call this.")]
	public bool waitForCommand;
	public override void LocalEffect(CallInfo callInfo)
	{
		base.LocalEffect(callInfo);
		if (callInfo.origin == null)
			return;
		
		if (!callInfo.caster.IsAuthoritative() && waitForCommand)
			return;

		float speed = DashSpeed.GetValue(callInfo);
		if (DashSpeedIsTimeInstead && callInfo.targetPoint.HasValue)
		{
			speed = Vector3.Distance(callInfo.origin.transform.position, callInfo.targetPoint.Value) / speed;
		}


		bool nonAuth = callInfo.caster.IsAuthoritative() && !callInfo.origin.IsAuthoritative();
		if (nonAuth && callInfo.targetPoint.HasValue)
        {
            callInfo.origin.NonAuthoritativeDash(callInfo.targetPoint.Value, speed, callInfo.caster.ID, waitForCommand);
        }

		if (callInfo.origin == null || !callInfo.origin.IsAuthoritative())
			return;
		if (callInfo.caster == callInfo.origin && callInfo.caster.HasStatus(Status.Type.CantInputMoveCommands))
			return;


		callInfo.origin.CantDashThroughCharacters = CantDashThroughCharacters;
        callInfo.origin.CantDashThroughWalls = CantDashThroughWalls;
		if (callInfo.targetPoint.HasValue)
			callInfo.origin.Dash(callInfo.targetPoint.Value, speed, callInfo.caster.ID);
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
		return false; //can't dash to characters
    }

    public override bool CanHitPoints()
    {
		return true;
    }
}
