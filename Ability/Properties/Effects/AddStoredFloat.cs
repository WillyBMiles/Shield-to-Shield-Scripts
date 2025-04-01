using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

[System.Serializable]
public class AddStoredFloat : Effect
{
	public AddStoredFloat()
    {
		TargetID = "SELF";
    }

	[Tooltip("Override the TargetID and just target the caster no matter what")]
	public bool AlwaysTargetCaster = false;

	public string StoredFloatID;

    public Value value = new Value();

	[Tooltip("This will be added to the previous float")]
	public bool relative;
	

	[ShowIf("@relative || stacksLastLimitedTime")]
	[Tooltip("Only used if relative is true")]
	public float defaultForRelative = 0f;

	[Tooltip("Stacks reset the timer and it must be ability wide. All stacks falloff at the same time, reseting to defaultForRelative.")]
	[ShowIf("@!CharacterWide")]
	public bool stacksLastLimitedTime = false;

	[ShowIf("@!CharacterWide && stacksLastLimitedTime")]
	public Value stackTime = new Value();

	[Tooltip("Does this count for the whole character (true) or just this ability (false)")]
	public bool CharacterWide = false;

	[Tooltip("Save it to the character")]
	[ShowIf(nameof(CharacterWide))]
	public bool SaveFloat = false;

	public override void LocalEffect(CallInfo callInfo)
	{
		base.LocalEffect(callInfo);

		Character targetCharacter = AlwaysTargetCaster ? callInfo.caster : callInfo.target;
		if (targetCharacter == null)
			return;

		float newValue = value.GetValue(callInfo);


		newValue = relative ? newValue + 
			targetCharacter.CheckStoredFloat(callInfo.ability, callInfo.itemID, StoredFloatID, defaultForRelative, CharacterWide, SaveFloat) : newValue;
		targetCharacter.StoreFloat(callInfo.ability, callInfo.itemID, StoredFloatID, newValue, CharacterWide, SaveFloat);
		
		if (stacksLastLimitedTime && !CharacterWide)
		{
			float time = stackTime.GetValue(callInfo);
			targetCharacter.AddStackFallof(callInfo.ability, callInfo.itemID, StoredFloatID, time, defaultForRelative);
		}
	}

    protected override bool AbstractHasLocalEffect()
    {
		return true;
    }

    public override bool HasLocalOnlyEffect()
    {
		return false;
    }

    public override bool HasServerEffect()
    {
		return false;
    }

	public override bool CanHitCharacters()
	{
		return true;
	}

	public override bool CanHitPoints()
	{
		return false;
	}
}
