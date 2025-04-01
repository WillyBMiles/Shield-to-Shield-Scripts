using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

[System.Serializable]
public class CheckFloat : Limitation
{
    public enum Sign
	{
        Equal,
        LessThan,
        GreaterThan
	}
	[Tooltip("Sign is from the perspective of the float, so greater than works when FloatID > checkAgainst")]
    public Sign sign;
	public string FloatID;
	[ValueDropdown(nameof(_targets))]
	[Tooltip("Which characters to check, SELF is caster")]
	public string TargetID = "SELF";

	[Tooltip("will be multiplied by checkAgainst if not null")]
	public Value value = new Value();

	public bool CanCastIfThereIsNoMatchingFloat = true;

	[Tooltip("If the float is stored character wide (true) or just for the ability (false)")]
	public bool CharacterWide = false;

	[ShowIf(nameof(CharacterWide))]
	public bool SavedFloat = false;


	public override void Initialize(Activation activation)
	{
		base.Initialize(activation);
		tempCharacters = new List<Character>();
	}

	List<Character> tempCharacters;
	public override bool CanCast(CallInfo callInfo)
	{
        callInfo.caster.characterAbilities.GetTargets(callInfo, tempCharacters, TargetID);


		bool finalReturn = true;

		if (tempCharacters.Count == 0)
			return CanCastIfThereIsNoMatchingFloat;
		foreach (Character character in tempCharacters)
		{
			if (character == null)
				continue;
			if (character.CheckStoredFloat(callInfo.ability, callInfo.itemID, FloatID, 0f, CharacterWide, SavedFloat) == 0f 
				&& character.CheckStoredFloat(callInfo.ability, callInfo.itemID, FloatID, 1f, CharacterWide, SavedFloat) == 1f) {
				finalReturn = CanCastIfThereIsNoMatchingFloat && finalReturn;
				continue;
			} //float doesn't exist
			

			float floatValue;
			floatValue = character.CheckStoredFloat(callInfo.ability, callInfo.itemID, FloatID, 0f, CharacterWide, SavedFloat);



			float checkAgainstFinal = value.GetValue(callInfo);
			switch (sign)
			{
				case Sign.Equal:
					finalReturn = floatValue == checkAgainstFinal  && finalReturn;
					break;
				case Sign.GreaterThan:
					finalReturn = floatValue > checkAgainstFinal && finalReturn;
					break;
				case Sign.LessThan:
					finalReturn = floatValue < checkAgainstFinal && finalReturn;
					break;
			}

		}
		return finalReturn;
	}
}
