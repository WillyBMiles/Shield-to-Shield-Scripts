using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

[System.Serializable]
public class MouseOver : Target
{
	public LayerMask layerMask;
	public bool TargetCharacters;

	[ShowIf(nameof(TargetCharacters))]
	public TargetMask targetMask;

	public bool OriginCharacter = false;

	public override void Initialize(Activation activation)
	{
		base.Initialize(activation);
		tempPoints = new List<Vector3>();
		tempCharacters = new List<Character>();
	}

	List<Vector3> tempPoints;
	List<Character> tempCharacters;

	protected override void TargetActivate(CallInfo callInfo)
    {

		Vector3 originPoint = Vector3.negativeInfinity;
		if (!OriginCharacter)
		{
            callInfo.caster.characterAbilities.GetTargets(callInfo, tempPoints, OriginTargetID);
			if (tempPoints.Count != 0)
				originPoint = tempPoints[0];
		}
		else
		{
            callInfo.caster.characterAbilities.GetTargets(callInfo, tempCharacters, OriginTargetID);
			if (tempCharacters.Count != 0)
				originPoint = tempCharacters[0].transform.position;
		}
		Vector3 flattenedOriginPoint = new Vector3(originPoint.x, 0f, originPoint.z);

		if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f, layerMask))
		{
			if (TargetCharacters)
			{
				Character hitChar = Character.GetCharacter(hit.collider.gameObject);
				if (hitChar != null)
				{
					Vector3 flattenedHitPoint = new Vector3(hitChar.transform.position.x, 0f, hitChar.transform.position.z);
					if (!callInfo.caster.FitsMask(hitChar, targetMask))
						return;
					float dist = Vector3.Distance(flattenedHitPoint, flattenedOriginPoint);
					if (dist < MaxDistance.GetValue(callInfo) && dist > MinDistance.GetValue(callInfo))
						AddTargets(callInfo, ClearTargets, hitChar);
				}
			}else
			{
				Vector3 flattenedHitPoint = new Vector3(hit.point.x, 0f, hit.point.z);
				//If no max/min distance just send the point
 				if (MinDistance.GetValue(callInfo) == -1 && MaxDistance.GetValue(callInfo) == -1)
				{
					AddTargets(callInfo, ClearTargets, flattenedHitPoint);
					return;
				}

				Vector3 dir;
				if ((flattenedHitPoint - flattenedOriginPoint).sqrMagnitude == 0f)
				{
					Vector2 t = Random.insideUnitCircle.normalized;
					dir = new Vector3(t.x, 0, t.y);
				}else 
					dir = (flattenedHitPoint - flattenedOriginPoint).normalized;

				if (MaxDistance.GetValue(callInfo) > 0f && Vector3.Distance(flattenedOriginPoint, flattenedHitPoint) > MaxDistance.GetValue(callInfo))
				{
					flattenedHitPoint = dir * MaxDistance.GetValue(callInfo) + flattenedOriginPoint;
				}

				if (MinDistance.GetValue(callInfo) > 0f && Vector3.Distance(flattenedOriginPoint, flattenedHitPoint) < MinDistance.GetValue(callInfo))
				{
					flattenedHitPoint = dir * MinDistance.GetValue(callInfo) + flattenedOriginPoint;
				}

				AddTargets(callInfo, ClearTargets, flattenedHitPoint);
			}
			
		}
	}

	public override bool IsDeterministic()
	{
		return false;
	}
}
