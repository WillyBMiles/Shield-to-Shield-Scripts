using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

[System.Serializable]
public class Projectile : Effect
{
	[BoxGroup("Vital", false)]
	[Tooltip("Which callbacks do I trigger")]
	public string TriggeringCallbackID = "Callback";

	[BoxGroup("Vital", false)]
	[Tooltip("Projectile will hit these layers")]
	public LayerMask layerMask = (int) Mathf.Pow(2,8); //"Character"

	[BoxGroup("Vital", false)]
	[Tooltip("Projectile will hit these targets")]
	public TargetMask targetMask = TargetMask.MyEnemies;

	[BoxGroup("Vital", false)]
	[Tooltip("Projectiles will just stop at these layers")]
	public LayerMask stopLayerMask = (int)Mathf.Pow(2, 7); //"Walls"

	[BoxGroup("Vital", false)]
	[Tooltip("What target ID should we override. The point will be where the projectile currently is, " +
		"and the characters will be characters that are currently being collided with by the projectile -- "+
		"Use TargetID for targetting the projectile's motion")]
	public string TargetOverride = "Projectile";

	[BoxGroup("Destruction", false)]
	[Tooltip("Destroy projectile upon reaching target, only works if there is a target point")]
	public bool destroyUponReachingTargetPoint = false;
	[BoxGroup("Destruction", false)]
	[Tooltip("Destroy projectile upon reaching target, only works if there is a target character")]
	public bool destroyUponReachingTargetCharacter = false;

	[BoxGroup("ExtraTargetting", false)]
	[Tooltip("Target characters, for the seek characters motion")]
	public bool TargetCharacters = false;

    [BoxGroup("ExtraTargetting", false)]
    [Tooltip("Origin characters")]
    public bool OverrideOriginWithCharacter = false;

    [BoxGroup("ExtraTargetting", false)]
	[Tooltip("Target points, for seek direction or points motions")]
	public bool TargetPoints = true;
	[BoxGroup("ExtraTargetting", false)]
	[Tooltip("If this is a shield...probably do false")]
	public bool StoppedByShields = true;

	[BoxGroup("ExtraTargetting", false)]
	[Tooltip("If this projectile follows the caster, check true")]
	public bool InheritCasterTime = false;


	[BoxGroup("ExtraTargetting", false)]
	[Tooltip("Can only hit the target, just phases through everything else")]
	public bool CanOnlyHitTarget = false;
	[BoxGroup("ExtraTargetting", false)]
	[Tooltip("Can hit the same target multiple times, careful it will be called every frame it collides " +
		"(good for reapplying status effects, not dealing damage or generating new projectiles)")]
	public bool CanHitSameTarget = false;

	[ShowIf(nameof(CanHitSameTarget))]
	[BoxGroup("ExtraTargetting", false)]
	[Tooltip("How often can it hit the same target")]
	public float TickRate = .25f;

	[HideIf(nameof(CanHitSameTarget))]
	[BoxGroup("ExtraTargetting", false)]
	[Tooltip("Projectiles with matching repeatTargetIDs cannot hit the same target")]
	public string RepeatTargetID = "";

	[ShowIf("@!CanHitSameTarget && RepeatTargetID != null && RepeatTargetID.Length != 0")]
	[BoxGroup("ExtraTargetting", false)]
	[Tooltip("When this projectile is fired reset RepeatTargetID")]
	public bool InitializeRepeatTarget = false;

	[BoxGroup("ExtraTargetting", false)]
	[Tooltip("Can hit targets behind the caster? This is relative to the projectile's movement direction")]
	public bool CanHitTargetsBehind = true;

	[BoxGroup("ExtraTargetting", false)]
	[Tooltip("Can't hit origin character")]
	public bool CantHitOriginCharacter = false;

	[BoxGroup("ExtraTargetting", false)]
	[Tooltip("Can hit untargettable characters")]
	public bool canTargetUntargettable = false;


	[BoxGroup("Values", false)]
	[Tooltip("The model/particles of the projectile. Not strictly necessary.")]
	public GameObject appearence = null;

	[BoxGroup("Values", false)]
	[Tooltip("Projectile must have item model, also this must be on an item.")]
	public bool InheritItemColor = false;


	[ShowIf(nameof(InheritItemColor))]
	[BoxGroup("Values", false)]
	[Tooltip("Projectile must have item model, also this must be on an item.")]
	public bool InheritItemModel = false;

	[BoxGroup("Values", false)]
	[Header("Width")]
	[Tooltip("Width of collision box, will also be used as a scale for the appearance")]
	public Value widthValue = new Value();

	[BoxGroup("Values", false)]
	[Tooltip("Only works if lifetime is set to a positive value")]
	public bool ChangeWidthOverLifetime = false;

	[BoxGroup("Values", false)]
	[Tooltip("Goes from width to endwidth and then back")]
	[ShowIf(nameof(ChangeWidthOverLifetime))]
	public bool PeakHalfway = false;

	[BoxGroup("Values", false)]
	[ShowIf(nameof(ChangeWidthOverLifetime))]
	[Tooltip("Width of collision box, AT END")]
	public Value endWidthValue = new Value();

	[BoxGroup("Values", false)]
	[Header("Number of Hits")]
	[Tooltip("how many different targets can the projectile hit before being destroyed, negative numbers mean infinite hits")]
	public Value numberOfHitsValue = new Value();

	[BoxGroup("Values", false)]
	[Header("Lifetime")]
	[Tooltip("lifetime in seconds. If <=0 will last infinite time (until destroyed by another source, such as distance)")]
	public Value lifetimeValue = new Value() { baseMult = -1 };

	[Tooltip("How long should the projectile not be able to hit targets? Also stops other things like shields or walls.")]
	[BoxGroup("Values", false)]
	public Value noHitTime = new Value() { baseMult = 0f };
	
	[BoxGroup("Values", false)]
	[Header("Distance")]
	[Tooltip("distance in units (1 unit is the character width). If <=0 will last infinite distance (until destroyed by another source, such as lifetime)")]
	public Value distanceValue = new Value() { baseMult = -1 };

	[BoxGroup("Values", false)]
	[Tooltip("Measure from the origin distance (true) or from the origin character's current position (false)")]
	public bool distanceFromPoint = true;

	
	
	[ExpandableAttribute]
	public ProjectileMotion motion;

	public override void LocalEffect(CallInfo callInfo)
	{
		base.LocalEffect(callInfo);
		if (TargetCharacters && callInfo.target == null)
			return;
		if (TargetPoints && !callInfo.targetPoint.HasValue)
			return;
		if (!TargetCharacters && !TargetPoints)
			return;
		Vector3 originPoint = new Vector3();
		if (callInfo.originPoint.HasValue)
		{
			originPoint = callInfo.originPoint.Value;
		}
		if ((!callInfo.originPoint.HasValue || OverrideOriginWithCharacter ) && callInfo.origin != null)
        {
			originPoint = callInfo.origin.transform.position;
        }
		CallInfo newInfo = callInfo.Duplicate();
		newInfo.originPoint = originPoint;

 		LocalProjectile.CreateProjectile(layerMask, TargetOverride, newInfo,
			motion, widthValue.GetValue(callInfo), appearence, (int) numberOfHitsValue.GetValue(callInfo), CanHitSameTarget,
			distanceValue.GetValue(callInfo), lifetimeValue.GetValue(callInfo),
			distanceFromPoint, CanOnlyHitTarget, EffectID, TriggeringCallbackID, destroyUponReachingTargetPoint, destroyUponReachingTargetCharacter,
			targetMask, stopLayerMask, CanHitTargetsBehind, StoppedByShields, TickRate, RepeatTargetID, InitializeRepeatTarget, CantHitOriginCharacter, canTargetUntargettable, this);
	}

	/*No longer needed
#if UNITY_EDITOR
	public override void RecalculateSubobjects()
	{
		base.RecalculateSubobjects();
		motion = null;
		string grossPath = AssetDatabase.GetAssetPath(this);
		string path = grossPath.Substring(0, grossPath.LastIndexOf("/"));
		string[] subFolders = AssetDatabase.GetSubFolders(path);
		string[] guids = AssetDatabase.FindAssets("t:ProjectileMotion", subFolders);
		foreach (string guid in guids)
		{
			string assetPath = AssetDatabase.GUIDToAssetPath(guid);
			motion = AssetDatabase.LoadAssetAtPath<ProjectileMotion>(assetPath);
		}
	}
#endif
	*/


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
		return TargetCharacters;
	}

	public override bool CanHitPoints()
	{
		return TargetPoints;
	}
}
