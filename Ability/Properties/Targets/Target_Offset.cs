using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.AI;
using System.Dynamic;

[System.Serializable]
public class Target_Offset : Target
{
	[Sirenix.OdinInspector.ValueDropdown(nameof(_targets))]
	[Header("Saves OldTargetID to TargetID, plus an offset")]
	[Tooltip("")]
	public string OldTargetID = "SELF";

	public Vector3 offset;

	public Value offsetMultipler = new Value();

	[HideIf(nameof(UseAwayTowardsCoordinates))]
	[Tooltip("If false use the caster's coordinate system instead")]
	public bool UseAbsoluteCoordinates;

	[HideIf(nameof(UseAbsoluteCoordinates))]
	[Tooltip("If true, z is towards and away from the character, and X the cross of Up and the new Z")]
	public bool UseAwayTowardsCoordinates;

	[ShowIf(nameof(UseAwayTowardsCoordinates))]
	public bool OverrideDirection = false;

	[Sirenix.OdinInspector.ValueDropdown(nameof(_targets))]
	[ShowIf("@UseAwayTowardsCoordinates && OverrideDirection")]
	public string DirectionID = "CURSOR";

	[Tooltip("Check only if DirectionID contains characters.")]
	[ShowIf("@UseAwayTowardsCoordinates && OverrideDirection")]
	public bool InterpretDirectionIDAsCharacter = false;


	[Tooltip("Add a bit of randomness. If you set the offset to 0,0,0 and then have nonZero jiggle you will just have a random vector")]
	public Value MinJiggle = new Value() { baseMult = 0f };

	[Tooltip("Will also make sure the offset lands on the NavMesh")]
	public bool SamplePosition = false;


	[Tooltip("Add a bit of randomness. If you set the offset to 0 it will be all randomness")]
	public Value MaxJiggle = new Value() { baseMult = 0f };

	[Tooltip("If true each point will offset a different amount")]
	public bool SeparateOffsets;


	[ShowIf("@SamplePosition && SeparateOffsets")]
	[Tooltip("If set to true, after sampling the position it will recalculate the jiggle if you end too close")]
	public bool GuaranteeMinJiggle = false;

	[ShowIf("@SamplePosition && GuaranteeMinJiggle && SeparateOffsets")]
	[Tooltip("How many times will it check before giving up on guaranteeing the min jiggle?")]
	public int HowManyChecks = 5;

	[ShowIf("@SamplePosition && GuaranteeMinJiggle && SeparateOffsets")]
	[Tooltip("As a proportion of minJiggle, how close do we have to be to retry? ")]
	public float MinDistanceGuarantee = .8f;


	[Tooltip("Check if oldtargetid is only characters")]
	public bool UseCharacterPositionsAsOrigins = false;

	[Tooltip("How many points to generate, and put on the same target id. Useful for synccing with delays.")]
	public Value HowManyPoints = new Value();

	public override void Initialize(Activation activation)
	{
		base.Initialize(activation);
		characters = new List<Character>();
		vector3s = new List<Vector3>();
		vector3Directions = new List<Vector3>();
		characterDirections = new List<Character>();
		originCs = new List<Character>();
		originPs = new List<Vector3>();
	}

	List<Character> characters;
	List<Vector3> vector3s;
	List<Vector3> vector3Directions;
	List<Character> characterDirections;
	List<Character> originCs;
	List<Vector3> originPs;

	protected override void TargetActivate(CallInfo callInfo)
	{
        callInfo.caster.characterAbilities.GetTargets(callInfo, characters, OldTargetID);
        callInfo.caster.characterAbilities.GetTargets(callInfo, vector3s, OldTargetID);

        callInfo.caster.characterAbilities.GetTargets(callInfo, originCs, OriginTargetID);
        callInfo.caster.characterAbilities.GetTargets(callInfo, originPs, OriginTargetID);

		Vector3 origin = originCs.Count == 0 ? (originPs.Count == 0 ? callInfo.caster.transform.position : originPs[0]) : originCs[0].transform.position;
		Vector3 actualOffset = offsetMultipler == null ? offset : offset * offsetMultipler.GetValue(callInfo);

		Vector3 direction = new Vector3();
		if (UseAwayTowardsCoordinates && OverrideDirection)
        {
			if (InterpretDirectionIDAsCharacter)
            {
                callInfo.caster.characterAbilities.GetTargets(callInfo, characterDirections, DirectionID);
				foreach (Character c in characterDirections)
                {
					if (c == null)
						continue;
					direction = new Vector3(c.transform.position.x, 0f, c.transform.position.z);
					break;
				}

            }
            else
            {
                callInfo.caster.characterAbilities.GetTargets(callInfo, vector3Directions, DirectionID);
				if (vector3Directions.Count > 0)
				{
					direction = new Vector3(vector3Directions[0].x, 0f, vector3Directions[0].z);
				}
			}
			
				
		}
		if (UseCharacterPositionsAsOrigins)
        {
			vector3s.Clear();
			foreach (Character c in characters)
            {
				if (c != null)
					vector3s.Add(c.transform.position);
			}
		}
		

		AddTargets(callInfo, ClearTargets, characters.ToArray());

		int numberOfPoints = HowManyPoints == null ? 1 : (int) HowManyPoints.GetValue(callInfo);
		for (int j = 0; j < numberOfPoints; j++)
        {
			Vector2 jiggle2 = GetJiggle(callInfo);
			float minJiggle = MinJiggle.GetValue(callInfo);

			foreach (Vector3 point in vector3s)
			{
				if (SeparateOffsets)
					jiggle2 = GetJiggle(callInfo);

				Vector3 point2 = new Vector3(point.x, 0f, point.z);
				Vector3 ctrans2 = new Vector3(origin.x, 0f, origin.z);

				Vector3 newOffset;
				Vector3 newTarget;
				Vector3 finalPoint;

				int i = 0;
				do
				{
					if (SeparateOffsets)
						jiggle2 = GetJiggle(callInfo);

					newOffset = UseAbsoluteCoordinates ? actualOffset : callInfo.caster.transform.localToWorldMatrix.MultiplyVector(actualOffset);
					if (UseAwayTowardsCoordinates)
					{
						Vector3 away = (point2 - ctrans2);
						if (OverrideDirection)
						{
							away = direction - ctrans2;
						}

						away.y = 0f;
						away = away.normalized;
						Vector3 up = Vector3.up;
						Vector3 right = Vector3.Cross(away, up).normalized;
						newOffset = away * actualOffset.z + right * actualOffset.x + up * actualOffset.y;

					}
					newTarget = point2 + newOffset + new Vector3(jiggle2.x, 0, jiggle2.y);
					finalPoint = newTarget;
					if (SamplePosition && NavMesh.SamplePosition(finalPoint, out NavMeshHit hit, 100f, NavMesh.AllAreas))
					{
						finalPoint = hit.position;
					}
					i++;
				} while (SamplePosition && GuaranteeMinJiggle && Vector3.Distance(finalPoint, point) < minJiggle * MinDistanceGuarantee && i < HowManyChecks);

				AddTargets(callInfo, false, finalPoint);
			}
		}
		

		
	}

	Vector2 GetJiggle(CallInfo callInfo)
	{
		float jiggle = Random.Range(MinJiggle.GetValue(callInfo), MaxJiggle.GetValue(callInfo));
		return jiggle * Random.insideUnitCircle.normalized;
	}

	public override bool IsDeterministic()
	{
		return MaxJiggle.NaiveIsZero();
	}
}
