using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;


[GUIColor(0f,.5f,1f,1f)]
[BoxGroup("Damage", false)]
[HideReferenceObjectPicker]
[System.Serializable]
public class Damage 
{
	public enum Type
	{
		Short,
		Long,
		Projectile,
		Extra
	}

	[System.Flags]
	public enum SubType
    {
		None = 0,
		Light = 1,
		Heavy = 2,
		Strike = 4,
		Ability = 8
    }

	
    public Value amount = new Value();
	public bool DontScaleWithAbilityBonus = false;
	public Type type;
	public SubType subType = 0;

	public Value staggerMultiplier = new Value();

	public bool OverrideStaggerAmount = false;
	[Tooltip("Also can't be increased")]
	public bool CantBeReduced = false;
	[ShowIf(nameof(OverrideStaggerAmount))]
	public Value staggerDamage = new Value();
	public bool DontHitStop = false;

	public void Apply(CallInfo callInfo, bool TriggersDamageOnHit)
	{
		if (callInfo.target != null)
        {
			float amountFloat = amount.GetValue(callInfo);
			if (!DontScaleWithAbilityBonus)
            {
				amountFloat = callInfo.caster.CheckAbilityBonus(AbilityBonus.Damage, callInfo.ability, amountFloat);
            }
			float newStaggerDamage = amountFloat;
			if (staggerMultiplier != null)
			{
				newStaggerDamage = OverrideStaggerAmount ? staggerDamage.GetValue(callInfo)
				 : amountFloat;
				newStaggerDamage *= staggerMultiplier.GetValue(callInfo);
			}


			callInfo.target.DealDamage(callInfo.InstanceID, callInfo.caster, amountFloat, type, subType, TriggersDamageOnHit, 
				callInfo.originPoint ?? callInfo.origin.transform.position, newStaggerDamage, CantBeReduced, DontHitStop);
		}
			
	}

	public void LocalApply(CallInfo callInfo, bool ApplyOnHit)
    {
		if (callInfo.target != null)
        {
			float amountFloat = amount.GetValue(callInfo);
			if (!DontScaleWithAbilityBonus)
			{
				amountFloat = callInfo.caster.CheckAbilityBonus(AbilityBonus.Damage, callInfo.ability, amountFloat);
			}
			float newStaggerDamage = amountFloat;
			if (staggerMultiplier != null)
            {
				newStaggerDamage = OverrideStaggerAmount ? staggerDamage.GetValue(callInfo)
				 : amountFloat;
				newStaggerDamage *= staggerMultiplier.GetValue(callInfo);
			}

			

			callInfo.target.ClientDealDamage(callInfo.InstanceID, callInfo.caster,amountFloat, type, subType, ApplyOnHit, callInfo.originPoint ?? callInfo.origin.transform.position, newStaggerDamage, CantBeReduced, DontHitStop);
		}
			
	}


}
