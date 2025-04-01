using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


[System.Serializable]
public class Effect_GenerateCallback : Effect
{
	[Header("Timing is when to reset, aka when are the conditions reset? TargetID doesn't matter except for triggering timing")]
	[Tooltip("Generate the callback once per hit")]
	public bool OncePerTarget = false;

	[ShowIf(nameof(OncePerTarget))]
	public float TargetExpiration = 10f;

	[Tooltip("Which callbackID do I trigger?")]
	public string TriggeringCallbackID;

	[Tooltip("Generate a callback when a new target apppears on this")]
	[Dropdown(nameof(_targets))]
	public string TriggeringTargetID;

	static readonly Dictionary<(Character, Item, Effect), (List<(Character,float)>,List<(Vector3, float)>)> hitTargets = 
		new Dictionary<(Character, Item, Effect), (List<(Character,float)>, List<(Vector3, float)>) >();

	List<Vector3> myTempPoints;
	List<Character> myTempCharacters;

	public override void Initialize(Activation activation)
	{
		base.Initialize(activation);

		myTempPoints = new List<Vector3>();
		myTempCharacters = new List<Character>();
	}

	protected override void UpdateThis(CallInfo callInfo)
	{
		base.UpdateThis(callInfo);
		//foreach target in targetid call "OnCallback" once per update, with the conditions above

		callInfo.caster.characterAbilities.GetTargets(callInfo, myTempCharacters, TriggeringTargetID);
		callInfo.caster.characterAbilities.GetTargets(callInfo, myTempPoints, TriggeringTargetID);

		if (OncePerTarget && !hitTargets.ContainsKey((callInfo.caster, callInfo.item, this)))
			hitTargets.Add((callInfo.caster, callInfo.item, this), (new List<(Character, float)>(), new List<(Vector3,float)>()));

		foreach (Character target in myTempCharacters)
		{

			if (!OncePerTarget || CheckTarget(callInfo.caster, callInfo.item, target))
			{
                CallInfo newInfo = callInfo.Duplicate();
                newInfo.CallbackID = TriggeringCallbackID;
                newInfo.target = target;
                callInfo.activation.OnCallback(newInfo);
            }
				

		}
		foreach (Vector3 point in myTempPoints)
		{
			if (!OncePerTarget || CheckTarget(callInfo.caster,callInfo.item, point))
			{
                CallInfo newInfo = callInfo.Duplicate();
                newInfo.CallbackID = TriggeringCallbackID;
                newInfo.targetPoint = point;
				callInfo.activation.OnCallback(newInfo);
			}
		}

		//ticking the timers down
		var lists = hitTargets[(callInfo.caster, callInfo.item, this)];
		for (int i =0; i< lists.Item1.Count;i++)
		{
			lists.Item1[i] = (lists.Item1[i].Item1, lists.Item1[i].Item2 - Time.deltaTime);
			if (lists.Item1[i].Item2 < 0)
			{
				lists.Item1.RemoveAt(i);
				i--;
			}
				
		}
		for (int i = 0; i < lists.Item2.Count; i++)
		{
			lists.Item2[i] = (lists.Item2[i].Item1, lists.Item2[i].Item2 - Time.deltaTime);
			if (lists.Item2[i].Item2 < 0)
			{
				lists.Item2.RemoveAt(i);
				i--;
			}
				
			
		}
	}

	public bool CheckTarget(Character character, Item item, Character target)
	{
		bool contains = false;
		foreach (var t in hitTargets[(character, item, this)].Item1)
		{
			if (t.Item1 == target)
				contains = true;
		}

		if (!contains)
		{
			hitTargets[(character, item, this)].Item1.Add((target, TargetExpiration));
			return true;
		}
		return false;
	}
	public bool CheckTarget(Character character, Item item, Vector3 point)
	{
		bool contains = false;
		foreach (var t in hitTargets[(character, item, this)].Item2)
		{
			if (t.Item1 == point)
				contains = true;
		}

		if (!contains)
		{
			hitTargets[(character, item, this)].Item2.Add((point, TargetExpiration));
			return true;
		}
		return false;
	}

	public override void LocalEffect(CallInfo callInfo)
	{
		base.LocalEffect(callInfo);
		hitTargets[(callInfo.caster, callInfo.item, this)].Item1.Clear();
		hitTargets[(callInfo.caster, callInfo.item, this)].Item2.Clear();
	}

	public override bool HasLocalOnlyEffect()
	{
		return false;
	}

	public override bool HasServerEffect()
	{
		return false;
	}

	protected override bool AbstractHasLocalEffect()
	{
		return true;
	}

	public override bool CanHitCharacters()
	{
		return true;
	}

	public override bool CanHitPoints()
	{
		return true;
	}
}
