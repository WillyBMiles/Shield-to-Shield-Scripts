using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

[System.Serializable]
public class Status
{
	public enum Type
	{
		Slow,
		Speed,
		Root,
		Invulnerability,
		Untargettability,
		Disarm,
		Silence,
		DamageBoost,
		DamageReduction,
		Vulnerability,
		Defense,

		TraitPercent,
		TraitFlat,

		Resistance,

		DirectionalDamageReduction,
		CantInputMoveCommands,
		TrueStun,
		DamageMultiplierDontAffectStagger,
		CantDash,
		CantBlink,

		BONUS

	}
	[Tooltip("Status ID for stacking")]
	public string StatusID;
	public Type type;
	[HideIf("@type == Type.Invulnerability || " +
		"type == Type.Untargettability || " +
		"type == Type.Disarm ||" +
		"type ==  Type.Silence ||" +
		"type ==  Type.Root" +
		"|| type == Type.CantInputMoveCommands" +
		"|| type == Type.TrueStun" +
		"|| type == Type.CantDash" +
		"|| type == Type.CantBlink" +
		"|| type == Type.BONUS")]
	[Tooltip("Stacks multiplicatively. For these types always leave as zero: root, invulnerability, untargettability, disarm, silence")]
	public Value amount = new Value() { baseNumber = 0f};
	[Tooltip("In seconds")]
	public Value duration = new Value();

	[ShowIf("@type == Type.TraitPercent || " +
		"type == Type.TraitFlat")]
	public Trait trait;

	[ShowIf("@type == Type.Resistance")]
	public Damage.Type damageType;

	float localDuration = 1f;
	float localAmount = 0f;
	float localAngle = 0f;

	[Tooltip("Having higher Defiance reduces the time of this status.")]
	public bool AffectedByDefiance = false;

	[Tooltip("Having higher Defiance INCREASES the time of this status.")]
	[ShowIf("@AffectedByDefiance == true")]
	public bool ReverseAffectedbyDefiance = false;

	[ShowIf("@type == Type.DirectionalDamageReduction")]
	public Vector3 Direction;

	[ShowIf("@type == Type.DirectionalDamageReduction")]
	public Value AngleWidth;

	[ShowIf(nameof(type), Type.BONUS)]
	public Bonus bonus = null;

	[Tooltip("Don't show icon over head")]
	public bool DontShowIcon = false;

	public bool ShowInOverview = false;

	[HideInInspector]
	public string ItemID = "";

	[ShowIf(nameof(ShowInOverview))]
	public string OverviewName = "Status";

	public bool DurationDontUseAbilityBonus = false;
	public bool EffectDontUseAblityBonus = false;

	public bool UpdateThis(Character character, float timeMult)
	{
		float multiplier = 1f;
		if (AffectedByDefiance)
        {
			multiplier = character.CheckTrait(Trait.Defiance, negate : ReverseAffectedbyDefiance);
        }

		localDuration -= multiplier * Time.deltaTime * timeMult;
		if (localDuration <= 0)
		{		
            return false;
        }
			
		return true;
	}

	public void Refresh(Status refreshingStatus)
    {
		if (refreshingStatus.StatusID != StatusID)
			return;
		localDuration = refreshingStatus.localDuration;
    }

	public void AddToCharacter(CallInfo callInfo, 
		bool stack = false, bool refresh = false, bool stackFromSameSource = true)
	{

		if (callInfo.caster == null)
			return;
		localDuration = duration.GetValue(callInfo);
		bool useDebuff = AffectedByDefiance && !ReverseAffectedbyDefiance;
		if (!DurationDontUseAbilityBonus)
		{
			if (useDebuff)
				localDuration = callInfo.caster.CheckAbilityBonus(AbilityBonus.DebuffTime, callInfo.ability, localDuration);
			else
				localDuration = callInfo.caster.CheckAbilityBonus(AbilityBonus.BuffTime, callInfo.ability, localDuration);
		}
		localAmount = amount.GetValue(callInfo);
        if (!EffectDontUseAblityBonus)
        {
            if (useDebuff)
                localDuration = callInfo.caster.CheckAbilityBonus(AbilityBonus.DebuffEffect, callInfo.ability, localDuration);
            else
                localDuration = callInfo.caster.CheckAbilityBonus(AbilityBonus.BuffEffect, callInfo.ability, localDuration);
        }
        if (type == Type.DirectionalDamageReduction)
			localAngle = AngleWidth.GetValue(callInfo);
        if (type == Type.BONUS)
		{
			Bonus newBonus = bonus.Duplicate();

			newBonus.CollapseValue(callInfo, 1f, null);
			newBonus.ID = StatusID;

			newBonus.type = Bonus.Type.Status;
			newBonus.myAbility = callInfo.ability;
			newBonus.mySkill = callInfo.ability.mySkill;
			newBonus.myItem = callInfo.item;
            callInfo.target.bonuses.Add(newBonus);
        }
            
        if (callInfo.item != null)
			ItemID = callInfo.item.uniqueIdentifier;
		else
			ItemID = "";
		callInfo.target.AddStatus((Status)this.MemberwiseClone(), stack, refresh, stackFromSameSource);
	}

	public float GetLocalAmount()
	{
		return localAmount;
	}

	public float GetLocalAngle()
    {
		return localAngle;
    }
}
