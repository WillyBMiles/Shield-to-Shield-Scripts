using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class Limitation_HasTarget : Limitation
{
	[ValueDropdown(nameof(_targets))]
	[Header("Ability must have targets of the given type in the given TargetIDs.")]
	[Tooltip("Which targetIDs are we checking targets for. These will be summed and checked against min/max number of targets below.")]
	public List<string> TargetID;

	[Tooltip("Check Characters? Must check either characters or targets.")]
	public bool Characters;
	[Tooltip("Check points? Must check either characters or targets.")]
	public bool Points;

	[Tooltip("There must be at least this many targets in targetID, inclusive")]
	public Value MinNumberOfTargets = new Value() { baseNumber = 1 };
	[Tooltip("There must be at most this many targets in targetID, inclusive")]
	public Value MaxNumberOfTargets = new Value() { baseNumber = float.PositiveInfinity} ;

	public override void Initialize(Activation activation)
	{
		base.Initialize(activation);
		tempCharacters = new List<Character>();
		tempPoints = new List<Vector3>();
	}

	List<Character> tempCharacters;
	List<Vector3> tempPoints;
	public override bool CanCast(CallInfo callInfo)
	{
		if (!Characters && !Points)
			Debug.Log("Forgot to set characters or points on Limitation_HasTarget");
			
		if (Characters)
		{
			tempCharacters.Clear();
			callInfo.caster.characterAbilities.GetTargets(callInfo, tempCharacters, TargetID.ToArray());
			if (Mathf.Clamp(tempCharacters.Count, MinNumberOfTargets.GetValue(callInfo), 
				MaxNumberOfTargets.GetValue(callInfo)) != tempCharacters.Count)
			{
				return false;
			}
		}
		if (Points)
		{
			tempPoints.Clear();
			callInfo.caster.characterAbilities.GetTargets(callInfo, tempPoints, TargetID.ToArray());
			if (Mathf.Clamp(tempCharacters.Count, MinNumberOfTargets.GetValue(callInfo), 
				MaxNumberOfTargets.GetValue(callInfo)) != tempCharacters.Count)
			{
				return false;
			}
		}
		return true;

	}
}
