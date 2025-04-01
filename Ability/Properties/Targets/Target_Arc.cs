using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Target_Arc : Target
{
	[Tooltip("LayerMask")]
	public TargetMask layerMask = TargetMask.MyEnemies;

	[Sirenix.OdinInspector.ValueDropdown(nameof(_targets))]
	[Tooltip("'SELF' always targets the caster, 'CURSOR' always targets the mouse on the ground, TARGETS POSITIONS")]
	public string TargetDirectionID = "CURSOR";

	[Tooltip("Arc angle, centered around  the middle")]
	public Value angle = new Value();

	[Tooltip("Number of targets it can target in range")]
	public Value number = new Value();

	[Tooltip("Target Points too, so you can use their positions")]
	public bool TargetPointsToo;

	[Tooltip("Can target caster?")]
	public bool CanTargetCaster = true;

	public bool UIShowAngle = false;

	static GameObject linePrefab { get
		{
			if (_linePrefab == null)
				_linePrefab = (GameObject)Resources.Load("Line_Handle");
			return _linePrefab;
		} }
	static GameObject _linePrefab;

	static GameObject arcHandlePrefab
	{
		get
		{
			if (_arcHandlePrefab == null)
				_arcHandlePrefab = (GameObject)Resources.Load("Arc_Handle");
			return _arcHandlePrefab;
		}
	}
	static GameObject _arcHandlePrefab;

	public override void Initialize(Activation activation)
	{
		base.Initialize(activation);
		arcArrows = new List<GameObject>();
		arcCircles = new List<ArcUI>();
		tempPositionsOrigin = new List<Vector3>();
		tempPositionsTarget = new List<Vector3>();
		tempCharacters = new List<Character>();
	}

	List<Vector3> tempPositionsOrigin;
	List<Vector3> tempPositionsTarget;
	List<Character> tempCharacters;
	protected override void TargetActivate(CallInfo callInfo)
	{
		tempPositionsOrigin.Clear();
		tempPositionsTarget.Clear();


        callInfo.caster.characterAbilities.GetTargets(callInfo, tempPositionsOrigin, OriginTargetID);
        callInfo.caster.characterAbilities.GetTargets(callInfo, tempPositionsTarget, TargetDirectionID);
		if (tempPositionsOrigin.Count > 0)
		{
			Vector3 originPosition = tempPositionsOrigin[0];
			int j = 0;
			float maxTargets = number.GetValue(callInfo);
			Vector2 originV2 = new Vector2(originPosition.x, originPosition.z);

			foreach (Vector3 target in tempPositionsTarget)
			{
				Vector2 targetV2 = new Vector2(target.x, target.z);
				tempCharacters.Clear();
                callInfo.caster.GetAllInRange(originPosition, tempCharacters, MaxDistance.GetValue(callInfo) + 15, layerMask, CanTargetUntargettable, CanTargetTrulyUntargettable);
				foreach (Character c in tempCharacters)
				{
					if (j >= maxTargets)
						return;
					if (c == callInfo.caster && !CanTargetCaster)
						continue;
					Vector2 targetCharacterV2 = new Vector2(c.transform.position.x, c.transform.position.z);

					float calcAngle = angle.GetValue(callInfo) / 2f;
					Vector2 dirV2 = (targetV2 - originV2).normalized;
					Quaternion rotLeft = Quaternion.AngleAxis(calcAngle, Vector3.forward);
					Quaternion rotRight = Quaternion.AngleAxis(calcAngle, -Vector3.forward);
					Ray2D leftRay = new Ray2D(originV2,  rotLeft * dirV2);
					Ray2D rightRay = new Ray2D(originV2, rotRight * dirV2);

					if (!(Vector2.Distance(targetCharacterV2, originV2) >= MinDistance.GetValue(callInfo) - c.hitBoxWidth / 2f //far enough
						&&
						Vector2.Distance(targetCharacterV2, originV2) <= MaxDistance.GetValue(callInfo) + c.hitBoxWidth / 2f //close enough
						))
						continue;
					if (!(Vector2.Angle(targetV2 - originV2, targetCharacterV2 - originV2) <= calcAngle //correct angle
						||
						(Vector2.Angle(targetV2 - originV2, targetCharacterV2 - originV2) <= calcAngle * 2 //OR close to correct angle
						&&
						(
						DistanceToRay(leftRay, targetCharacterV2) < c.hitBoxWidth / 2f //and pretty close to the left line
						||
						DistanceToRay(rightRay, targetCharacterV2) < c.hitBoxWidth / 2f //or the right line
						)
						)))
						continue;
					AddTargets(callInfo, false, c);
					if (TargetPointsToo)
					{
						AddTargets(callInfo, false, c.transform.position);
					}
					j++;

				}
				
			}
			
		}
	}

	List<GameObject> arcArrows;

	List<ArcUI> arcCircles;
	protected override void ShowUI(CallInfo callInfo)
	{
		base.ShowUI(callInfo);
		DontUI();
		if (!UIShowAngle)
			return;
		int j = 0;
		int k = 0;
		tempPositionsOrigin.Clear();
		tempPositionsTarget.Clear();


        callInfo.caster.characterAbilities.GetTargets(callInfo, tempPositionsOrigin, OriginTargetID);
        callInfo.caster.characterAbilities.GetTargets(callInfo, tempPositionsTarget, TargetDirectionID);
		if (tempPositionsOrigin.Count > 0)
		{
			Vector3 origin = tempPositionsOrigin[0];
			float maxDist = MaxDistance.GetValue(callInfo);
			float minDist = MinDistance.GetValue(callInfo);

			if (minDist == 0)
            {
				while (arcCircles.Count < tempPositionsTarget.Count)
				{
					arcCircles.Add(GameObject.Instantiate(arcHandlePrefab).GetComponent<ArcUI>());
				}
			}
			else
            {
				while (arcCircles.Count < tempPositionsTarget.Count * 2)
				{
					arcCircles.Add(GameObject.Instantiate(arcHandlePrefab).GetComponent<ArcUI>());
				}

			}

			foreach (Vector3 target in tempPositionsTarget)
			{
				Vector2 t2 = new Vector2(target.x, target.z);
				Vector2 o2 = new Vector2(origin.x, origin.z);

				float startAngle = Vector2.Angle(t2 - o2, Vector2.right);
				if ((t2 - o2).y < 0)
					startAngle = 360 - startAngle;
				float angleFloat = angle.GetValue(callInfo) / 2f;
				float leftAngle = angleFloat + startAngle;
				Vector2 leftPoint = o2 + new Vector2(Mathf.Cos(Mathf.Deg2Rad * leftAngle), Mathf.Sin(Mathf.Deg2Rad * leftAngle)) *
					maxDist;
				Vector2 leftMinPoint = o2 + new Vector2(Mathf.Cos(Mathf.Deg2Rad * leftAngle), Mathf.Sin(Mathf.Deg2Rad * leftAngle)) *
					minDist;

				float rightAngle = -angleFloat + startAngle;
				Vector2 rightPoint = o2 + new Vector2(Mathf.Cos(Mathf.Deg2Rad * rightAngle), Mathf.Sin(Mathf.Deg2Rad * rightAngle)) *
					maxDist;
				Vector2 rightMinPoint = o2 + new Vector2(Mathf.Cos(Mathf.Deg2Rad * rightAngle), Mathf.Sin(Mathf.Deg2Rad * rightAngle)) *
					minDist;

				arcCircles[k].SetUI(o2, leftPoint, rightPoint);
				k++; 
				if (minDist != 0)
                {
					arcCircles[k].SetUI(o2, leftMinPoint, rightMinPoint);
					k++;
				}

				if (minDist == 0)
                {
					j = ShowLine(j, arcArrows, origin, new Vector3(leftPoint.x, 0f, leftPoint.y), 0f, linePrefab, 0f);
					j = ShowLine(j, arcArrows, origin, new Vector3(rightPoint.x, 0f, rightPoint.y), 0f, linePrefab, 0f);
				}
				else
                {
					j = ShowLine(j, arcArrows, new Vector3(leftMinPoint.x, 0f, leftMinPoint.y), new Vector3(leftPoint.x, 0f, leftPoint.y), 0f, linePrefab, 0f);
					j = ShowLine(j, arcArrows, new Vector3(rightMinPoint.x, 0f, rightMinPoint.y), new Vector3(rightPoint.x, 0f, rightPoint.y), 0f, linePrefab, 0f);
				}
				
			}
		}
	}

	private void DontUI()
	{
		for (int i = 0; i < arcArrows.Count; i++)
        {
			if (arcArrows[i] == null)
            {
				arcArrows.RemoveAt(i);
				i--;
            }
        }

		for (int i = 0; i < arcCircles.Count; i++)
        {
			if (arcCircles[i] == null)
			{
				arcCircles.RemoveAt(i);
				i--;
			}
		}

		foreach (GameObject arrow in arcArrows)
		{
			arrow.SetActive(false);
		}
	}

	public override bool IsDeterministic()
	{
		return true;
	}
}
