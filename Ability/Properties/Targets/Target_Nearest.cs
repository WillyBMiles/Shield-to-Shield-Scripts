using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class Target_Nearest : Target
{
	public override void Initialize(Activation activation)
	{
		base.Initialize(activation);
		tempPositions = new List<Vector3>();
		tempCharacters = new List<Character>();
	}

	[Tooltip("LayerMask")]
	public TargetMask layerMask;
	public bool differentStartingLayerMask = false;
	[ShowIf("differentStartingLayerMask")]
	[Tooltip("Starting LayerMask, for example for dead characters")]
	public TargetMask startingMask;

	[Tooltip("Number of targets it can target in range")]
	public Value number = new Value();

	[Tooltip("Target Points too, so you can use their positions")]
	public bool TargetPointsToo;

	[Tooltip("Can target caster?")]
	public bool CanTargetCaster = true;

	[Tooltip("Sort by something other than distance")]
	public bool ExtraSortCondition = false;

	[ShowIf(nameof(ExtraSortCondition))]
	[Tooltip("Make sure to use the 'Target instead' features, otherwise this will be pretty shitty")]
	public Value SortCondition = new Value();

	[Tooltip("Will normally sort closest to furthest/lowest to highest, set this to go highest to lowest")]
	public bool invertSortOrder = false;

	[Tooltip("Can't Target Dead Characters, Collapsed characters are also dead")]
	public bool CantTargetDeadCharacters = false;

	List<Vector3> tempPositions;
	List<Character> tempCharacters;
	protected override void TargetActivate(CallInfo callInfo)
	{
		tempPositions.Clear();
		tempCharacters.Clear();
			

		callInfo.caster.characterAbilities.GetTargets(callInfo, tempPositions, OriginTargetID);
		if (tempPositions.Count > 0)
		{
			Vector3 originPosition = tempPositions[0];

			int j = 0;

			j = AddNearestTargets(callInfo,layerMask, j);
		}
	}

    class ValueCompare : IComparer<Character>
    {
		public Value valueToCompare;
		public CallInfo callInfo;

        public int Compare(Character x, Character y)
        {
			if (valueToCompare == null)
				return 0;
			else
            {
				return valueToCompare.GetValue(callInfo).CompareTo(
					valueToCompare.GetValue(callInfo));
            }
        }
    }
	static ValueCompare comparer;

    int AddNearestTargets(CallInfo callInfo, TargetMask mask, int index)
	{
		Vector3 originPosition = callInfo.actualOriginPoint;


        callInfo.caster.GetAllInRange(originPosition,
			tempCharacters, MaxDistance.GetValue(callInfo) + 15, mask, CanTargetUntargettable, CanTargetTrulyUntargettable);

		if (!CanTargetCaster)
			tempCharacters.Remove(callInfo.caster);

		int i = 0;
		if (ExtraSortCondition)
        {
			if (comparer == null)
				comparer = new ValueCompare();
			comparer.callInfo = callInfo;
			comparer.valueToCompare = SortCondition;
			tempCharacters.Sort(comparer);
        }

		if (invertSortOrder)
			tempCharacters.Reverse();

		while (index < number.GetValue(callInfo) && i < tempCharacters.Count)
		{
			Faction startingFaction = callInfo.caster.GetFaction(startingMask);
			Character target = tempCharacters[i];
			bool onCorrectLayer = (startingFaction & target.StartingFaction) != 0 || startingFaction == 0;
			

			if (Vector3.Distance(target.transform.position, originPosition) >= MinDistance.GetValue(callInfo) - target.hitBoxWidth / 2f 
				&&
				Vector3.Distance(target.transform.position, originPosition) <= MaxDistance.GetValue(callInfo) + target.hitBoxWidth / 2f
				&&
				(!differentStartingLayerMask || onCorrectLayer)
				&&
				(!CantTargetDeadCharacters || !target.Dead )
				)
			{
				AddTargets(callInfo, false,tempCharacters[i]);
				if (TargetPointsToo)
					AddTargets(callInfo, false, tempCharacters[i].transform.position);
				index++;
			}
			i++;
		}
		return index;
	}

    public override bool IsDeterministic()
    {
		return true;
    }
}
