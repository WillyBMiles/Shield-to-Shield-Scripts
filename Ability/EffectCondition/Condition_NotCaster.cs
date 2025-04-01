using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new NotCaster", menuName = "Ability/EffectCondition/NotCaster")]
public class Condition_NotCaster : EffectCondition
{
	public override bool CheckCondition(CallInfo callInfo)
	{
		return callInfo.target != callInfo.caster && base.CheckCondition(callInfo);
	}
}
