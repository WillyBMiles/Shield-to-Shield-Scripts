using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

[System.Serializable]
public class Effect_Status : Effect
{
	public Effect_Status()
    {
		TargetID = "SELF";
    }

	public List<Status> statuses = new List<Status>();
	[Tooltip("If stack is false, applying the same status multiple times will just extend the duration")]
	public bool stack = false;

	[ShowIf(nameof(stack))]
	[Tooltip("Refresh statcks when a new one is applied")]
	public bool refreshStacks = false;


	[ShowIf(nameof(stack))]
	[Tooltip("Don't stack from the same item and character. But do stack from different items/character")]
	public bool dontStackFromSameSource = false;

	public override void LocalEffect(CallInfo callInfo)
	{
		base.LocalEffect(callInfo);
		if (callInfo.target == null)
			return;
		foreach (Status status in statuses)
		{
			status.AddToCharacter(callInfo, stack, refreshStacks, !dontStackFromSameSource);
		}
		
	}

	[Button]
	void AddStatus()
	{
		statuses.Add(new Status() { StatusID = System.DateTime.Now.ToString() });
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
		return false;
	}
}
