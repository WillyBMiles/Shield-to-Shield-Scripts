using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Condition_CanCast", menuName = "Ability/EffectCondition/CanCast")]
public class Condition_CanCast : EffectCondition
{
	public bool CantCast = false;
	public bool OnlyInterrupts = false;

	public override bool CheckCondition(CallInfo callInfo)
	{ 
		if (CantCast)
        {
			return base.CheckCondition(callInfo) && !callInfo.activation.CanCast(callInfo, OnlyInterrupts);
		}

		return base.CheckCondition(callInfo) && callInfo.activation.CanCast(callInfo, OnlyInterrupts);
	}
}
