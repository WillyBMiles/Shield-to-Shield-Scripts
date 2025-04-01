using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Lock_Target : Target
{
	[Sirenix.OdinInspector.ValueDropdown(nameof(_targets))]
	[Header("Saves OldTargetID to TargetID")]
	[Tooltip("")]
	public string OldTargetID;

	public override void Initialize(Activation activation)
	{
		base.Initialize(activation);
		characters = new List<Character>();
		vector3s = new List<Vector3>();
	}

	List<Character> characters;
	List<Vector3> vector3s;

	protected override void TargetActivate(CallInfo callInfo)
	{
		callInfo.caster.characterAbilities.GetTargets(callInfo, characters, OldTargetID);
		callInfo.caster.characterAbilities.GetTargets(callInfo, vector3s, OldTargetID);

		AddTargets(callInfo, ClearTargets, characters.ToArray());
		AddTargets(callInfo, ClearTargets, vector3s.ToArray());
	}

	public override bool IsDeterministic()
	{
		return true;
	}

}
