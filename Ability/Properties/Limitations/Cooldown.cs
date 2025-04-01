using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

[System.Serializable]
public class Cooldown : Limitation
{
	[Header("Need only be unique in this ability, also timing is when cooldown starts")]
	public string StoredFloatID;
	[Tooltip("Is the stored floatID character wide or just ability wide")]
	public bool StoredFloatCharacterWide = false;

	[ShowIf(nameof(StoredFloatCharacterWide))]
	public bool SaveFloat = false;

	[Tooltip("Value is in seconds")]
	public Value lengthValue = new Value();

	[Tooltip("Start on cooldown?")]
	public bool StartOnCooldown = false;

	[Tooltip("Probably want this to be false if you're storing the float characterWide")]
	public bool EnsureUniqueStorage = true;

	[Tooltip("If the conditions aren't met then the cooldown will not reset at timing.")]
	public List<EffectCondition> conditions = new List<EffectCondition>();



	public override void Activate(CallInfo callInfo)
	{
		base.Activate(callInfo);
		string ID = StoredFloatCharacterWide ? StoredFloatID : Activation.GetUniqueStorageID(callInfo.item, StoredFloatID);

		foreach (EffectCondition condition in conditions)
		{
			if (!condition.CheckCondition(callInfo))
				return;
		}
		callInfo.caster.StoreFloat(callInfo.ability, callInfo.itemID, ID, MaxCooldown(callInfo), StoredFloatCharacterWide, SaveFloat);
	}

	protected override void UpdateThis(CallInfo callInfo)
	{
		base.UpdateThis(callInfo);
		string ID = StoredFloatCharacterWide ? StoredFloatID : Activation.GetUniqueStorageID(callInfo.item, StoredFloatID);
		float value = CheckCooldown(callInfo, false);
		callInfo.caster.StoreFloat(callInfo.ability, callInfo.itemID, ID, value - Time.deltaTime, StoredFloatCharacterWide, SaveFloat);
	}

	public override bool CanCast(CallInfo callInfo)
	{
		return CheckCooldown(callInfo, false) <= 0f;
	}

	public float CheckCooldown(CallInfo callInfo, bool relative)
	{
		string ID = StoredFloatCharacterWide ? StoredFloatID : Activation.GetUniqueStorageID(callInfo.item, StoredFloatID);
		float abs;
		abs = callInfo.caster.CheckStoredFloat(callInfo.ability, callInfo.itemID, ID, StartOnCooldown ? MaxCooldown(callInfo) : 0f, 
			StoredFloatCharacterWide, SaveFloat);

		if (!relative)
			return abs;
		else
		{
			return abs / MaxCooldown(callInfo);
		}
	}

	float MaxCooldown(CallInfo callInfo)
	{
		return lengthValue.GetValue(callInfo);
	}
}
