using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Condition_Static", menuName = "Ability/EffectCondition/Static")]
public class Condition_Static : EffectCondition
{
	public StaticCondition sc;

	public override bool CheckCondition(CallInfo callInfo)
	{
		return base.CheckCondition(callInfo) && sc.Check(callInfo,1f);
	}
}
