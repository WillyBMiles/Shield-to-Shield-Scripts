using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class Target_OnHit : Target
{
	public bool LimitTime = false;

	[ShowIf(nameof(LimitTime))]
	public Value time = new Value();

	
	[Tooltip("So this can trigger on other characters")]
	[Sirenix.OdinInspector.ValueDropdown(nameof(_targets))]
	public string TriggeringCharacterID = "SELF";

	public string TriggeredCallbackID = "Callback";

	[Tooltip("Store the damage dealt as a stored float, if empty string don't store it. (Always pre midigation)")]
	public string StoreFloatDamageDone = "";

	[HideIf("@StoreFloatDamageDone == \"\"")]
	[Tooltip("Store the damage dealt character wide? You have to, to use it as a value. ")]
	public bool StoredFloatCharacterWide = false;

	[ShowIf("@StoredFloatCharacterWide && StoreFloatDamageDone == \"\"")]
	public bool StoredFloatSave = false;

	[Tooltip("Only trigger on these damage types. If empty list triggers on all damage types.")]
	public List<Damage.Type> types = new List<Damage.Type>();

	public Damage.SubType triggerSubType = 0;

	public override void Initialize(Activation activation)
	{
		base.Initialize(activation);
		overrides = new List<Character>();
		origins = new List<Character>();
	}

	[Tooltip("Triggers when YOU hit something")]
	public bool TriggerOnHit = true;
	[Tooltip("Triggers when you GET hit by something")]
	public bool TriggerGotHit = false;

	private List<Character> overrides;
	private List<Character> origins;

	[Tooltip("Both min and max are inclusive")]
	public bool UseDamageLimits;
	[ShowIf(nameof(UseDamageLimits))]
	public Value MinDamage = new Value() { baseMult = 0f };
	[ShowIf(nameof(UseDamageLimits))]
	public Value MaxDamage = new Value() { baseMult = Mathf.Infinity };

	[FoldoutGroup("ExtraConditions")]
	public bool OnlyIfBlocked = false;
	[FoldoutGroup("ExtraConditions")]
	public bool OnlyIfNotBlocked = false;
	[FoldoutGroup("ExtraConditions")]
	public bool OnlyIfArmored = false;
	[FoldoutGroup("ExtraConditions")]
	public bool OnlyIfNotArmored = false;
	[FoldoutGroup("ExtraConditions")]
	public bool OnlyIfDeflected = false;
	[FoldoutGroup("ExtraConditions")]
	public bool OnlyIfNotDeflected = false;
	[FoldoutGroup("ExtraConditions")]
	[Tooltip("Wont trigger on damage tagged 'Dont hitstop'")]
	public bool OnlyOnHitStop = false;




	public void OnHit(CallInfo callInfo,
			Character attacker, Character defender, Character target, Character nonTarget, HitInfo hitInfo)
	{
		if (hitInfo.DontHitStop && OnlyOnHitStop)
			return;
		if (origins == null || overrides == null)
			return;

		if ((callInfo.caster.Dead && !callInfo.activation.CanCastIfDead)
			|| 
			( !callInfo.caster.Dead && callInfo.activation.CanCastIfDead && callInfo.activation.CanOnlyCastIfDead))
		{
			return;
        }

		if (defender == null)
			return;

		if (!(hitInfo.triggersOnHit))
			return;
		if (!TriggerGotHit && defender == target)
			return;
		if (!TriggerOnHit && attacker == target)
			return;

		if (OnlyIfBlocked && !hitInfo.hitBlock)
			return;
		if (OnlyIfNotBlocked && hitInfo.hitBlock)
			return;
		if (OnlyIfArmored && !hitInfo.hitArmor)
			return;
		if (OnlyIfNotArmored && hitInfo.hitArmor)
			return;
		if (OnlyIfDeflected && !hitInfo.deflected)
			return;
		if (OnlyIfNotDeflected && hitInfo.deflected)
			return;

		AddTargets(callInfo, ClearTargets, target);
		if (types != null && types.Count != 0 && !types.Contains(hitInfo.type))
			return;
		if (triggerSubType != Damage.SubType.None && ((triggerSubType & hitInfo.subtypes) == 0))
			return;



		if (UseDamageLimits)
        {
			float minDamage = MinDamage.GetValue(callInfo);
			float maxDamage = MaxDamage.GetValue(callInfo);
			if (Mathf.Clamp(hitInfo.finalAmount, minDamage, maxDamage) != hitInfo.finalAmount)
            {
				return;
            }

        }

		if (StoreFloatDamageDone != "")
		{
            callInfo.caster.StoreFloat(callInfo.ability, callInfo.itemID, StoreFloatDamageDone, hitInfo.finalAmount, StoredFloatCharacterWide, StoredFloatSave);
				
		}
		if (target == null)
			return;
		CallInfo newInfo = callInfo.Duplicate();
		newInfo.CallbackID = TriggeredCallbackID;
		newInfo.target = nonTarget;
		newInfo.targetPoint = nonTarget.transform.position;
		newInfo.origin = target;
		newInfo.originPoint = target.transform.position;
		newInfo.CallbackID = TriggeredCallbackID;

        callInfo.activation.OnHitDamage?.Invoke(newInfo);
	}

	protected override void TargetActivate(CallInfo callInfo)
	{
        callInfo.caster.characterAbilities.GetTargets(callInfo, origins, TriggeringCharacterID);
        foreach (Character origin in origins)
        {
            if (origin.characterAbilities != null)
            {
                origin.characterAbilities.AddOnHit(-1,
                    new CharacterAbilities.OnHitInfo() { callInfo = callInfo, target = this }
                    , LimitTime ? time.GetValue(callInfo) : -1);
            }
        }
    }

	public override bool IsDeterministic()
	{
		return false;
	}
}