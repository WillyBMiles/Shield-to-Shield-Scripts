using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

[System.Serializable]
public class TargetDistance : Limitation
{
	[ValueDropdown(nameof(_targets))]
	[Header("Check distance from TargetID to OriginID, if it's between min and max return true")]
	[Tooltip("Checks all targets")]
	public string TargetID = "CURSOR";
	public bool TargetCharacters;
	[ValueDropdown(nameof(_targets))]
	[Tooltip("Checks all origins")]
	public string OriginID = "SELF";
	public bool OriginCharacters;

	public bool CanCastIfNoTarget = false;

	public Value MinDistance = new Value() { baseMult = 0f };
	public Value MaxDistance = new Value() { baseMult = Mathf.Infinity };

	public override void Initialize(Activation activation)
	{
		base.Initialize(activation);
		targetPoints = new List<Vector3>();
		targets = new List<Character>();
		originPoints = new List<Vector3>();
		origins = new List<Character>();
	}

	List<Vector3> targetPoints;
	List<Character> targets;
	List<Vector3> originPoints;
	List<Character> origins;

	public override bool CanCast(CallInfo callInfo)
	{
		if (TargetCharacters)
		{
			callInfo.caster.characterAbilities.GetTargets(callInfo, targets, TargetID);
			targetPoints.Clear();
			foreach (Character c in targets)
			{
				if (c != null)
					targetPoints.Add(c.transform.position);
			}
		}else
		{
			callInfo.caster.characterAbilities.GetTargets(callInfo, targetPoints, TargetID);
		}

		if (OriginCharacters)
		{
			callInfo.caster.characterAbilities.GetTargets(callInfo, origins, OriginID);
			originPoints.Clear();
			foreach (Character c in origins)
			{
				originPoints.Add(c.transform.position);
			}
		}
		else
		{
			callInfo.caster.characterAbilities.GetTargets(callInfo, originPoints, OriginID);
		}
		if (targetPoints.Count == 0)
			return CanCastIfNoTarget;

		foreach (Vector3 t in targetPoints)
		{
			foreach (Vector3 o in originPoints)
			{
				float dist = Vector3.Distance(o, t);
				float minDist = MinDistance.GetValue(callInfo);
				float maxDist = MaxDistance.GetValue(callInfo);
				if (minDist > maxDist)
					return false;
				if (Mathf.Clamp(dist,minDist, maxDist) != dist)
				{
					return false;
				}
			}

		}
		return true;

	}
}

