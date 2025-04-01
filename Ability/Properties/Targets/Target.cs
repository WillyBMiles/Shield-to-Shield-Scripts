using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;


[System.Serializable]
public abstract class Target : Property
{
	public Target()
	{
		TargetID = this.GetType().ToString() + Random.value;
		name = TargetID;
	}

    [Tooltip("Must be unique within an ability")]
    public string TargetID;

	[FoldoutGroup("Main")]
	[Tooltip("Add targets to list of targets or clear them first")]
	public bool ClearTargets = true;

	[FoldoutGroup("Main")]
	[Tooltip("Can only get this if you can prepare")]
	public bool CanPrepare = false;

	[FoldoutGroup("Main")]
	public bool LockWhileCasting = false;

	[HideReferenceObjectPicker]
	[FoldoutGroup("Main")]
	public UI uiOptions = new UI();

	[FoldoutGroup("Main")]
	[Tooltip("Max Distance for the target (will bring the target to edge of range), -1 is infinite range (wherever you click), " +
	"only works when targetting points NOT characters")]
	public Value MaxDistance = new Value() { baseMult = float.PositiveInfinity };

	[FoldoutGroup("Main")]
	[Tooltip("Min Distance for the target (will bring the target to edge of range), -1 is no minimum range (wherever you click), " +
		"only works when targetting points NOT characters")]
	public Value MinDistance = new Value() { baseMult = 0f };

	[FoldoutGroup("Main")]
	[ValueDropdown(nameof(_targets))]
	[Tooltip("TargetID for point to test the radius and distance from")]
	public string OriginTargetID = "SELF";

	[FoldoutGroup("Main")]
	[Tooltip("Using Autobounds means automatically bound between min distance and max distance of OriginID")]
	public bool UseAutoBounds = false;

	[FoldoutGroup("Main")]
	public bool UseTargetFilter = false;

	[FoldoutGroup("Main")]
	[HideReferenceObjectPicker]
	[ShowIf(nameof(UseTargetFilter))]
	public TargetFilter targetFilter = new TargetFilter();

	[Tooltip("Can target enemies with the untargettable status")]
	[FoldoutGroup("Main")]
	public bool CanTargetUntargettable = false;

	[ShowIf(nameof(CanTargetUntargettable))]
	[FoldoutGroup("Main")]
	[Tooltip("Can target enemies marked untargettable")]
	public bool CanTargetTrulyUntargettable = false;

	[FoldoutGroup("Main")]
	[Tooltip("Store number of targets (characters only) as float")]
	public bool StoreCountAsFloat = false;

	[ShowIf(nameof(StoreCountAsFloat))]
	[FoldoutGroup("Main")]
	[Tooltip("What float ID to store as, it's always ability-wide")]
	public string CountFloatID;

	public class TargetFilter
    {
		public enum Type
        {
			Equal,
			GreaterThan,
			LessThan,
        }

		public Value value1 = new Value();
		public Type comparison = Type.Equal;
		public Value value2 = new Value();


		public bool FitsFilter(CallInfo callInfo)
        {
			float v1 = value1.GetValue(callInfo);
			float v2 = value2.GetValue(callInfo);

			switch (comparison)
            {
				case Type.Equal:
					return v1 == v2;
				case Type.GreaterThan:
					return v1 > v2;
				case Type.LessThan:
					return v1 < v2;
            }
			throw new System.Exception("Filter type: " + comparison.ToString() + " Not accounted for!!");
        }
    }

	[GUIColor(1f,.2f,.2f,1f)]
	[System.Serializable]
	public class UI
	{
		[Tooltip("Indicate the targets with a circle, centered on the target")]
		public bool IndicateTargets = false;

		[MyBox.ConditionalField("IndicateTargets")]
		[Tooltip("How wide is the handle on targets")]
		public float TargetWidth = 2f;
		
		[Tooltip("Indicate the ranges with circle(s) centered on the origin")]
		public bool IndicateRanges = false;
		[Tooltip("Draw a line from the origin to the target.")]
		public bool PointToTargets = false;
		[ShowIf(nameof(PointToTargets))]
		public Value LineWidth = new Value();

		[Tooltip("How high above the ground will the handles be?")]
		public float HandleHeight = .5f;

	}

	Character[] emptyChar;
	Vector3[] emptyVec;

	public override void Activate(CallInfo callInfo)
	{
		base.Activate(callInfo);
		if (!callInfo.caster.IsAuthoritative() && !IsDeterministic() && !callInfo.activation.RunLocallyOnClient)
        {
			return;//don't get non deterministic stuff unless you're supposed to
        }
		if (CanPrepare && !callInfo.activation.CanPrepare(callInfo, false))
			return;
		if (LockWhileCasting && callInfo.caster.characterAbilities.CheckCastingAbility(callInfo.ability, callInfo.itemID))
			return;

		if (tempCs == null)
		{
			tempCs = new List<Character>();
		}
		if (originTargets == null)
		{
			originTargets = new List<Vector3>();
		}

        callInfo.caster.characterAbilities.GetTargets(callInfo, originTargets, OriginTargetID);
        callInfo.caster.characterAbilities.GetTargets(callInfo, tempCs, OriginTargetID);
        if (originTargets.Count != 0)
		{
			callInfo.originPoint = originTargets[0];
		}
		if (tempCs.Count != 0)
		{
			callInfo.origin = tempCs[0];
		}

        AddTargets(callInfo, ClearTargets, emptyVec);
		AddTargets(callInfo, ClearTargets, emptyChar);
		TargetActivate(callInfo);
	}

	protected abstract void TargetActivate(CallInfo callInfo);

	protected void AddTargets(CallInfo callInfo, bool ClearTargets, params Vector3[] points)
	{
		if (UseAutoBounds)
        {
			callInfo.caster.characterAbilities.GetTargets(callInfo, originTargets, OriginTargetID);
			if (originTargets.Count != 0)
            {
				tempTargetPoints.Clear();
				foreach (Vector3 p in points)
				{
					Vector3 boundPoint = CharacterAbilities.BoundVector3InRange(p, MinDistance.GetValue(callInfo),
					MaxDistance.GetValue(callInfo), originTargets[0]);
					tempTargetPoints.Add(boundPoint);
				}
				callInfo.caster.characterAbilities.AddTarget(callInfo.ability, callInfo.itemID, TargetID, ClearTargets, tempTargetPoints.ToArray());
				return;
			}
		}

		

		callInfo.caster.characterAbilities.AddTarget(callInfo.ability, callInfo.itemID, TargetID, ClearTargets, points);
	}

	static List<Character> filterTemps = new List<Character>();
	static List<Character> filterTemps2 = new List<Character>();
	static List<Character> tempCs = new List<Character>();
	protected void AddTargets(CallInfo callInfo, bool ClearTargets, params Character[] characters)
	{
		if (tempCs == null)
			tempCs = new List<Character>();

		tempCs.Clear();
		tempCs.AddRange(characters);
		if (!CanTargetUntargettable)
        {
			foreach (Character c in characters)
            {
				if (c == null)
					continue;
				if (c.HasStatus(Status.Type.Untargettability))
					tempCs.Remove(c);
            }
        }
		if (!CanTargetTrulyUntargettable)
        {
			foreach (Character c in characters)
			{
				if (c == null)
					continue;
				if (c.Untargettable)
					tempCs.Remove(c);
			}
		}
			
		if (!UseTargetFilter)
			callInfo.caster.characterAbilities.AddTarget(callInfo.ability, callInfo.itemID, TargetID, ClearTargets, tempCs.ToArray());
		else
        {
			if (filterTemps == null)
				filterTemps = new List<Character>();
			if (filterTemps2 == null)
				filterTemps2 = new List<Character>();
			filterTemps.Clear();
			filterTemps.AddRange(tempCs);
			filterTemps2.Clear();
			filterTemps2.AddRange(tempCs);
			foreach (Character ft in filterTemps2)
            {
				CallInfo testInfo = callInfo.Duplicate();
				testInfo.target = ft;

				if (!targetFilter.FitsFilter(testInfo))
                {
					filterTemps.Remove(ft);
                }
            }
			callInfo.caster.characterAbilities.AddTarget(callInfo.ability, callInfo.itemID, TargetID, ClearTargets, filterTemps.ToArray());
		}
		if (StoreCountAsFloat)
        {
			callInfo.caster.characterAbilities.GetTargets(callInfo, tempCs, TargetID);
			if (CountFloatID == null)
				return;
			callInfo.caster.StoreFloat(callInfo.ability, callInfo.itemID, CountFloatID, tempCs.Count, false, false);
		}

		
	}

	public int GetID(Activation activation)
	{
		int i = 0;
		foreach (Target target in activation.targets)
		{
			if (target == this)
				return i;
			i++;
		}
		return -1;
	}

	/*
	[MenuItem("Ability/Activation/AddTarget/TARGET")]
	static void AddTarget()
	{
		AddTarget<>();
	}
	*/

	static GameObject circleHandle;
	static GameObject arrowHandle;

	public override void Initialize(Activation activation)
	{
		base.Initialize(activation);
		activation.OnPreparingCast += ShowUI;
		activation.OnNotPreparingCast += DontShowUI;

		if (circleHandle == null)
			circleHandle = (GameObject) Resources.Load("Circle_Handle");
		if (arrowHandle == null)
			arrowHandle = (GameObject)Resources.Load("Arrow_Handle");
		arrows = new Dictionary<string,List<GameObject>>();
		circles = new Dictionary<string,List<GameObject>>();
		tempTargets = new List<Character>();
		tempTargetPoints = new List<Vector3>();
		originTargets = new List<Vector3>();

		emptyChar = new Character[0];
		emptyVec = new Vector3[0];
	}


	protected Dictionary<string, List<GameObject>> arrows;
	protected Dictionary<string, List<GameObject>> circles;

	List<Character> tempTargets;
	List<Vector3> tempTargetPoints;
	List<Vector3> originTargets;
	protected virtual void ShowUI(CallInfo callInfo)
	{
 		if (!callInfo.caster.IsAuthoritative())
			return;

		string itemId = callInfo.item == null ? "" : callInfo.item.uniqueIdentifier;

		DontShowUI(callInfo);
		int i = 0;

		tempTargets.Clear();
		callInfo.caster.characterAbilities.GetTargets(callInfo, tempTargets, TargetID);
		tempTargetPoints.Clear();
		callInfo.caster.characterAbilities.GetTargets(callInfo, tempTargetPoints, TargetID);
		originTargets.Clear();
		callInfo.caster.characterAbilities.GetTargets(callInfo, originTargets, OriginTargetID);

		if (!arrows.ContainsKey(itemId))
        {
			arrows[itemId] = new List<GameObject>();
        }
		if (!circles.ContainsKey(itemId))
		{
			circles[itemId] = new List<GameObject>();
		}

		//draw a circle around each target
		if (uiOptions.IndicateTargets)
		{
			foreach (Character target in tempTargets)
			{
				i = ShowCircle(i, circles[itemId], uiOptions.TargetWidth, uiOptions.HandleHeight, target.transform.position);
			}
			foreach (Vector3 target in tempTargetPoints)
			{
				i = ShowCircle(i, circles[itemId], uiOptions.TargetWidth, uiOptions.HandleHeight, target);
			}

		}

		//draw an arrow from origin to the target
		if (uiOptions.PointToTargets)
		{
			float lineWidth = 1f;
			if (uiOptions.LineWidth != null)
            {
				lineWidth = uiOptions.LineWidth.GetValue(callInfo);
            }

			int j = 0;
			if (originTargets.Count > 0)
			{
				foreach (Character target in tempTargets)
				{
					j = ShowArrow(j, arrows[itemId], originTargets[0], target.transform.position, uiOptions.HandleHeight, lineWidth);
				}
				foreach (Vector3 target in tempTargetPoints)
				{
					j = ShowArrow(j, arrows[itemId], originTargets[0], target, uiOptions.HandleHeight, lineWidth);
				}
			}

			
		}

		//draw a circle around the range
		if (uiOptions.IndicateRanges)
		{
			if (originTargets.Count > 0)
			{
				float min = MinDistance.GetValue(callInfo);
				float max = MaxDistance.GetValue(callInfo);
				i = ShowRange( i, circles[itemId], originTargets[0], min, max, uiOptions.HandleHeight);
			}
			i++;
		}
	}


	static List<GameObject> staticCircles = new List<GameObject>();
	static List<GameObject> staticArrows = new List<GameObject>();

	public static void ShowStaticUI(Vector3 origin, Vector3 point, 
		bool showPoint, float pointRadius, 
		bool showRange, float minRange, float maxRange,
		bool arrowPoint, float arrowWidth,  
		float handleHeight)
	{
		DontShowStaticUI();

		int i = 0;
		if (showPoint)
		{
			i = ShowCircle(i, staticCircles, pointRadius, handleHeight, point);
		}
		if (showRange)
		{
			i = ShowRange(i, staticCircles, origin, minRange, maxRange, handleHeight);
		}
		int j = 0;
		if (arrowPoint)
		{
			j = ShowArrow(j, staticArrows, origin, point, handleHeight, arrowWidth);
		}

	}

	#region Show UI Helpers
	protected static int ShowRange(int i, 
		List<GameObject> circles, Vector3 origin,
		float minDistance, float maxDistance, float handleHeight)
	{
		if (circleHandle == null)
			circleHandle = (GameObject)Resources.Load("Circle_Handle");

		while (circles.Count < i + 2)
		{
			GameObject obj = Object.Instantiate(circleHandle, new Vector3(), Quaternion.identity);
			circles.Add(obj);
		}
		circles[i].SetActive(true);
		circles[i + 1].SetActive(true);

		circles[i].transform.position = new Vector3(origin.x, handleHeight, origin.z);
		circles[i + 1].transform.position = new Vector3(origin.x, handleHeight, origin.z);
		float min = minDistance;
		float max = maxDistance;
		circles[i].transform.localScale = new Vector3(min, min, min);
		circles[i + 1].transform.localScale = new Vector3(max, max, max);
		i++;
		return i;

	}

	protected static int ShowArrow(int j, 
		List<GameObject> arrows, Vector3 origin,
		Vector3 target, float handleHeight, float width = 1f)
	{
		if (arrowHandle == null)
			arrowHandle = (GameObject)Resources.Load("Arrow_Handle");
		return ShowLine(j, arrows, origin, target, handleHeight, arrowHandle, width);
	}

	protected static int ShowLine(int j,
		List<GameObject> lines, Vector3 origin,
		Vector3 target, float handleHeight, GameObject linePrefab, float width = 1f)
	{
		while (lines.Count < j + 1)
		{
			GameObject obj = Object.Instantiate(linePrefab, new Vector3(), Quaternion.identity);
			lines.Add(obj);
		}
		lines[j].SetActive(true);
		Vector3 pos = target;
		Vector3 originPos = origin;
		originPos = new Vector3(originPos.x, handleHeight, originPos.z);
		pos = new Vector3(pos.x, handleHeight, pos.z);

		UIArrow uiArrow = lines[j].GetComponent<UIArrow>();
		uiArrow.PointTowards(originPos, pos, width);
		j++;
		return j;
	}

	protected static int ShowCircle(int i, List<GameObject> circles, float targetWidth, float handleHeight, Vector3 target)
	{
		if (circleHandle == null)
			circleHandle = (GameObject)Resources.Load("Circle_Handle");

		while (circles.Count < i + 1)
		{
			GameObject obj = Object.Instantiate(circleHandle, new Vector3(), Quaternion.identity);
			circles.Add(obj);
		}

		circles[i].SetActive(true);
		Vector3 pos = target;
		pos = new Vector3(pos.x, handleHeight, pos.z);
		circles[i].transform.position = pos;
		circles[i].transform.localScale = new Vector3(targetWidth, targetWidth, targetWidth);
		i++;
		return i;
	}
	#endregion

	public static void DontShowStaticUI()
	{
		for (int i = 0; i < staticCircles.Count; i++)
		{
			GameObject circle = staticCircles[i];
			if (circle == null)
            {
				staticCircles.RemoveAt(i);
				
				i--;
				continue;
			}
				
			circle.SetActive(false);
		}
		for(int i = 0; i < staticArrows.Count; i++)
		{

			GameObject arrow = staticArrows[i];
			if (arrow == null)
			{
				staticArrows.RemoveAt(i);

				i--;
				continue;
			}
			arrow.SetActive(false);
		}
	}

	protected virtual void DontShowUI(CallInfo callInfo)
	{

		string itemId = callInfo.item == null ? "" : callInfo.item.uniqueIdentifier;

		if (circles != null && circles.ContainsKey(itemId))
        {
			for (int i = 0; i < circles[itemId].Count; i++)
            {
				GameObject circle = circles[itemId][i];
				if (circle == null)
                {
					circles[itemId].RemoveAt(i);
					i--;
					continue;
                }
				circle.SetActive(false);
            }
		}
		if (arrows != null && arrows.ContainsKey(itemId))
        {
			for (int i = 0; i < arrows[itemId].Count; i++)
			{
				GameObject arrow = arrows[itemId][i];
				if (arrow == null)
				{
					arrows[itemId].RemoveAt(i);
					i--;
					continue;
				}
				arrow.SetActive(false);
			}
		}
		
	}


	protected static float DistanceToRay(Ray2D ray, Vector2 point)
	{
		return Vector3.Cross(ray.direction, point - ray.origin).magnitude;
	}

	protected static Vector3 ClampPoint(Vector3 point, Vector3 origin, float minDistance, float maxDistance, bool flatten = false)
    {
		if (flatten)
        {
			point = new Vector3(point.x, 0f, point.z);
			origin = new Vector3(origin.x, 0f, origin.z);
        }

		Vector3 finalPoint = point;
		Vector3 dir = (point - origin).normalized;
		float dist = Vector3.Distance(point, origin);
		if (dist < minDistance)
        {
			finalPoint = origin + dir * minDistance;
        }
		if (dist > maxDistance)
        {
			finalPoint = origin + dir * maxDistance;
        }
		return finalPoint;
    }


	public abstract bool IsDeterministic();
}
