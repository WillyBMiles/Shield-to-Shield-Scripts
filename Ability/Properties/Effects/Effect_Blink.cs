using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Effect_Blink : Effect
{
	[Header("Teleports origin characater to target POSITION")]
	[Tooltip("Ignore origincharacter and instead target the caster")]
	public bool OverrideOrigin = false;

	[Tooltip("Switch it so that the target CHARACTER is teleported to the originPoint")]
	public bool SwapTargetAndOrigin = false;

	[Tooltip("Add a tiny bit of randomness so that multiple characters teleporting to the same spot looks better.")]
	public bool JiggleALittle = false;

	[Tooltip("If you don't set this, you won't be able to blink while dashing at all.")]
	public bool CancelDashes = false;
	public override void LocalOnlyEffect(CallInfo callInfo)
	{
		base.LocalEffect(callInfo);
		Character origin = SwapTargetAndOrigin ? callInfo.target : (OverrideOrigin ? callInfo.caster : callInfo.origin);

		if (CancelDashes)
			origin.EndDash();
		Vector3 blinkPosition = SwapTargetAndOrigin ? callInfo.originPoint ?? callInfo.origin.transform.position : callInfo.targetPoint ?? callInfo.target.transform.position;

		if (float.IsInfinity(blinkPosition.x) || origin == null)
			return;
		if (origin.isDashing || origin._isDashing) //can't teleport while dashing
			return;
		
		if (JiggleALittle)
        {
			Vector2 v2 = Random.insideUnitCircle;
			blinkPosition += new Vector3(v2.x, 0f, v2.y);
        }
		if (origin == callInfo.caster)
        {
            callInfo.caster.AuthoritativeBlink(blinkPosition);
        }
			
		else
		{
            callInfo.caster.CmdBlink(origin.ID, blinkPosition);
		}
		
	}

	public override bool HasLocalOnlyEffect()
	{
		return true;
	}

	public override bool HasServerEffect()
	{
		return false;
	}

	protected override bool AbstractHasLocalEffect()
	{
		return false;
	}


	public override bool CanHitCharacters()
	{
		return SwapTargetAndOrigin;
	}

	public override bool CanHitPoints()
	{
		return !SwapTargetAndOrigin;
	}
}
