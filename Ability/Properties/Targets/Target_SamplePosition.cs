using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class Target_SamplePosition : Target
{
	[Sirenix.OdinInspector.ValueDropdown(nameof(_targets))]
	[Header("On timing, takes the StartingTargetID POINT and samples the position of the navmesh")]
	[Tooltip("TargetID is always a walkable point on the navmesh")]
	public string StartingTargetID = "SELF";

	public override void Initialize(Activation activation)
	{
		base.Initialize(activation);
		tempPositions = new List<Vector3>();
		tempOrigins = new List<Vector3>();
	}

	List<Vector3> tempPositions;
	List<Vector3> tempOrigins;
	protected override void TargetActivate(CallInfo callInfo)
	{
		tempPositions.Clear();
		tempOrigins.Clear();
		AddTargets(callInfo, ClearTargets, tempPositions.ToArray());
		callInfo.caster.characterAbilities.GetTargets(callInfo, tempPositions, StartingTargetID);
		callInfo.caster.characterAbilities.GetTargets(callInfo, tempOrigins, OriginTargetID);
		Vector3 origin = new Vector3();
		if (tempOrigins.Count != 0)
        {
			origin = tempOrigins[0];
        }

		foreach (Vector3 target in tempPositions)
		{
			Vector3 t = target;
			if (tempOrigins.Count != 0)
				t = ClampPoint(target, origin, MinDistance.GetValue(callInfo), 
					MaxDistance.GetValue(callInfo), true);

			if (NavMesh.SamplePosition(t, out NavMeshHit hit, 1000f, NavMesh.AllAreas))
			{
				AddTargets(callInfo, false, hit.position);
			}
			
		}


	}

	public override bool IsDeterministic()
	{
		return true;
	}
}
