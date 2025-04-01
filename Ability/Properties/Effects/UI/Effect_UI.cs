using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Effect_UI : Effect
{
	[Header("Timing should probably be PreparingAbility")]
	[Tooltip("What UI handle to generate?")]
	public GameObject UIHandle;
	[Tooltip("Radius of Handle")]
	public Value radius = new Value() { baseNumber = 1 };
	[Tooltip("Distance of UIHandle above ground")]
	public float YPosition = .1f;
	[Tooltip("Target characters instead of points")]
	public bool TargetCharacter = false;

	public override void LocalEffect(CallInfo callInfo)
	{
		base.LocalEffect(callInfo);
		if (!callInfo.caster.hasAuthority)
			return;

		GameObject obj = null;
		if (TargetCharacter)
		{
			if (callInfo.target != null)
				obj = Object.Instantiate(UIHandle, callInfo.target.transform.position, Quaternion.identity);
		}
		else
		{
			if (callInfo.targetPoint.HasValue)
				obj = Object.Instantiate(UIHandle, callInfo.targetPoint.Value, Quaternion.identity);
		}

		if (obj != null)
		{
			float r = radius.GetValue(callInfo);
			obj.transform.localScale = new Vector3(r, r, r);
			obj.transform.position = new Vector3(obj.transform.position.x, YPosition, obj.transform.position.z);
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
		return true;
	}
}
