using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class Effect_NavAgent : Effect
{
	[Tooltip("Should we change the target's angular speed?")]
	public bool SetAngularSpeed;
	[ShowIf(nameof(SetAngularSpeed))]
	[Tooltip("Whats the new absolute AngularSpeed?")]
	public Value newAngularSpeed;

	[Tooltip("Should we change the target's acceleration?")]
	public bool SetAcceleration;
	[ShowIf(nameof(SetAcceleration))]
	[Tooltip("Whats the new absolute Acceleration?")]
	public Value newAcceleration;

	[Tooltip("Should we change the target's Autobraking?")]
	public bool SetAutobraking;
	[ShowIf(nameof(SetAutobraking))]
	[Tooltip("Whats the new absolute Autobreaking?")]
	public bool newAutobraking;

	[Tooltip("How long until it's reset?")]
	public Value duration;

	public override void LocalEffect(CallInfo callInfo)
	{
		base.LocalEffect(callInfo);

		if (callInfo.target == null || !callInfo.target.IsAuthoritative() || callInfo.target.agent == null)
			return;
		if (SetAngularSpeed)
            callInfo.target.SetAngularSpeed(newAngularSpeed.GetValue(callInfo), duration.GetValue(callInfo));
		if (SetAcceleration)
            callInfo.target.SetAcceleration(newAcceleration.GetValue(callInfo), duration.GetValue(callInfo));

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
