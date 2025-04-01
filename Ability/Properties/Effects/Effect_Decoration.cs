using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class Effect_Decoration : Effect
{
	[Header("Spawns an object purely for show")]
	[Tooltip("Appearance of object, prefab")]
	public GameObject appearance;
	[Tooltip("How long it lasts, in seconds")]
	public Value duration = new Value();
	public Value width = new Value();
	public Value height = new Value();

	public bool OnlyShowOnOriginPlayer = false;
	public bool OnlyShowOnCaster = false;

	[Tooltip("point towards origin, origin must be a character")]
	public bool PointTowardsOrigin = false;

	//public string DecorationID = "";

	[Tooltip("Only some decorations can use color.")]
	public Color color;

	[Tooltip("Does it follow characters?")]
	public bool FollowCharacter;

	[ShowIf(nameof(FollowCharacter))]
	[Tooltip("Follow character rotation too")]
	public bool FollowCharacterRotation;

	[ShowIf(nameof(FollowCharacter))]
	public bool FollowAfterDeath;

	[Tooltip("Pop in, like go in and out")]
	public bool Pop = false;



    public override void LocalEffect(CallInfo callInfo)
	{
		base.LocalEffect(callInfo);
		if (OnlyShowOnOriginPlayer && (callInfo.origin == null || callInfo.origin != Character.GetLocalCharacter()))
			return;
		if (OnlyShowOnCaster && (callInfo.caster == null || callInfo.caster != Character.GetLocalCharacter()))
			return;
		if (FollowCharacter && callInfo.target == null)
			return;
		if (!FollowCharacter && !callInfo.targetPoint.HasValue)
			return;
		Vector3 point;
		if (FollowCharacter)
			point = callInfo.target.transform.position;
		else
			point = callInfo.targetPoint.Value;

		LocalDecoration.CreateDecoration(point, width.GetValue(callInfo),
			height.GetValue(callInfo), appearance, duration.GetValue(callInfo), callInfo.target, FollowCharacter, color,
			this, callInfo.origin);

		//if (DecorationID != null && DecorationID != "")
		//	character.AddDecoration(DecorationID, ability, itemAttribute, newObj);
		
	}

    public override void LocalOnlyEffect(CallInfo callInfo)
    {
        base.LocalOnlyEffect(callInfo);
		if (!OnlyShowOnCaster)
			return;

		if (FollowCharacter && callInfo.target == null)
			return;
		if (!FollowCharacter && callInfo.targetPoint.HasValue)
			return;
        Vector3 point;
        if (FollowCharacter)
            point = callInfo.target.transform.position;
        else
            point = callInfo.targetPoint.Value;

        LocalDecoration.CreateDecoration(point, width.GetValue(callInfo),
			height.GetValue(callInfo), appearance, duration.GetValue(callInfo), callInfo.target, FollowCharacter, color,
			this, callInfo.origin);

		//if (DecorationID != null && DecorationID != "")
		//	character.AddDecoration(DecorationID, ability, itemAttribute, newObj);

	}

    protected override bool AbstractHasLocalEffect()
    {
		return !OnlyShowOnCaster;
    }

	public override bool HasLocalOnlyEffect()
	{
		return OnlyShowOnCaster;
	}

	public override bool HasServerEffect()
	{
		return false;
	}

	public override bool CanHitCharacters()
	{
		return FollowCharacter;
	}

	public override bool CanHitPoints()
	{
		return !FollowCharacter;
	}
}

