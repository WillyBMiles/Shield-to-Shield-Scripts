using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

[System.Serializable]
public class CurrentPosition : Target
{
	public override void Initialize(Activation activation)
	{
		base.Initialize(activation);
		tempCharacters = new List<Character>();
	}

	[ValueDropdown(nameof(_targets))]
	[Header("On timing, saves the current position of all characters in CharacterTargeteID into TargetID")]
	[Tooltip("Used to 'lock in' a location of a character, if you want a delayed effect at a charcter's previous location when they start casting for example")]
	public string CharacterTargetID = "SELF";

	List<Character> tempCharacters;
	protected override void TargetActivate(CallInfo callInfo)
	{
		tempCharacters.Clear();
		AddTargets(callInfo, ClearTargets, tempCharacters.ToArray());
		callInfo.caster.characterAbilities.GetTargets(callInfo, tempCharacters, CharacterTargetID);

		foreach (Character target in tempCharacters)
		{
			if (target != null)
				AddTargets(callInfo, false, target.transform.position);
		}
		

	}

	public override bool IsDeterministic()
	{
		return true;
	}
}
