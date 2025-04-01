using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Target_PointAndClick : Target
{
	public override void Initialize(Activation activation)
	{
		base.Initialize(activation);
		overrides = new List<Character>();
		origins = new List<Character>();
	}

	public Value ClickWidth = new Value() { 
	baseNumber = float.PositiveInfinity};

	[Tooltip("Which characters will actually be targetted in the targetID")]
	public TargetMask TargetMask;
	[Tooltip("Which layer(s) can you click on? (should include Character probably)")]
	public LayerMask DetectionLayer;


	private List<Character> overrides;
	private List<Character> origins;
	protected override void TargetActivate(CallInfo callInfo)
	{
		if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f, DetectionLayer)){
 			GameObject hitObject = hit.collider.gameObject;
			Vector3 point = hit.point;
			callInfo.caster.characterAbilities.GetTargets(callInfo, origins, OriginTargetID);
			Character origin = callInfo.caster;
			if (origins.Count > 0)
				origin = origins[0];

			float clickWidth = ClickWidth.GetValue(callInfo);

			overrides.Clear();
			callInfo.caster.GetAllInRange(point, overrides, clickWidth, TargetMask, CanTargetUntargettable, CanTargetTrulyUntargettable);

			if (overrides.Count == 0)
				return;

			float minDist = MinDistance.GetValue(callInfo);
			float maxDist = MaxDistance.GetValue(callInfo);
			//Check if character is what's been clicked on
			foreach (Character c in overrides)
			{
				if (hitObject == c.gameObject && (CanTargetUntargettable || !c.HasStatus(Status.Type.Untargettability)) && (CanTargetTrulyUntargettable || c.Untargettable))
				{
					float dist = Vector2.Distance(new Vector2(c.transform.position.x, c.transform.position.z),
					new Vector2(origin.transform.position.x, origin.transform.position.z));
					if (dist <= maxDist && dist >= minDist)
					{
						AddTargets(callInfo, ClearTargets, c);
						return;
					}
					break;
				}
			}
			foreach (Character c in overrides)
			{
				float dist = Vector2.Distance(new Vector2(c.transform.position.x, c.transform.position.z), 
					new Vector2(origin.transform.position.x, origin.transform.position.z));
				if (dist <= maxDist && dist >= minDist)
				{
					AddTargets(callInfo, ClearTargets, c);
					return;
				}
			}

		}

	}

	public override bool IsDeterministic()
	{
		return false;
	}
}
