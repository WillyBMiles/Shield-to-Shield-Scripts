using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new ConditionDead", menuName = "Ability/EffectCondition/Dead")]
public class Condition_Dead : EffectCondition
{
	public bool NotDead;
	// Start is called before the first frame update
	public override bool CheckCondition(CallInfo callInfo)
	{
		return (NotDead ? !callInfo.caster.Dead : callInfo.caster.Dead) && base.CheckCondition(callInfo);
	}
}
