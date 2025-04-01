using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "new EffectCondition", menuName = "Ability/EffectCondition/Default")]
public class EffectCondition : SerializedScriptableObject
{
    public TargetMask targetMask;

    public virtual bool CheckCondition(CallInfo callInfo)
	{
		Faction faction = callInfo.caster.GetFaction(targetMask);
		return callInfo.target == null || (faction & callInfo.target.MyFaction) != 0;
	}

	public virtual void Initialize(Activation activation)
    {
		//noop
    }

}
