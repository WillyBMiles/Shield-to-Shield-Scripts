using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Sirenix.OdinInspector;

[System.Serializable]
public class Effect_Spawn : Effect
{
	[ShowIf("@spawnableObjects == null || spawnableObjects.Count == 0")]
	[Header("Spawns object at origin POINT, then stores in the object the target position")]
	[Tooltip("Make sure this is actually spawnable in the network manager and has a SpawnableObject script on it")]
	public GameObject spawnableObject;

	[Tooltip("Spawn one of these, at random.")]
	public List<GameObject> spawnableObjects = new List<GameObject>();

	[Tooltip("Don't spawn at origin, spawn at target instead. Must be a point still")]
	public bool SpawnAtTarget = false;

	[Tooltip("Spawn at the location of a character instead of a point")]
	public bool SpawnAtCharacter = false;

	[Tooltip("Duration, if the value resolves to zero, it lasts forever. 'Locks in' duration when cast.")]
	public Value duration = new Value() { baseNumber = 0, };

	[Tooltip("If not \"\" store anything that spawns here in this targetID")]
	public string StoreCharacterTargetID = "";

	[Tooltip("Override direction to point in the Target Direction, only works on points")]
	public bool PointInTargetDirection = false;

	[Tooltip("Override starting health")]
	public bool OverrideStartingHealth = false;
	[ShowIf(nameof(OverrideStartingHealth))]
	public Value StartingHealth = new Value();

	[Tooltip("Both summon and summoner must have a PlayerColor component on them ")]
	public bool InheritColor = false;

	public bool DontOverrideFactions = false;


	public bool CanSpawnInOtherRooms = false;
	public override void ServerEffect(CallInfo callInfo)
	{
		base.ServerEffect(callInfo);

		if (SpawnAtTarget && SpawnAtCharacter && callInfo.target == null)
			return;
		if (SpawnAtCharacter && !SpawnAtTarget && callInfo.origin == null)
			return;
		if (!SpawnAtCharacter && !callInfo.targetPoint.HasValue)
			return;
		GameObject prefab = spawnableObject;
		if (spawnableObjects.Count > 0)
        {
			prefab = spawnableObjects[Random.Range(0, spawnableObjects.Count)];
        }

		Vector3 p = SpawnAtCharacter ?
			(SpawnAtTarget ? callInfo.target.transform.position : callInfo.origin.transform.position) :
			(SpawnAtTarget ? callInfo.targetPoint.Value : callInfo.originPoint.Value);

		if (float.IsInfinity(p.x))
			return;

		Quaternion rot = Quaternion.identity;
		if (PointInTargetDirection)
        {
			Vector3 flattenedP = new Vector3(p.x, 0f, p.z);
			Vector3 flattenedPoint = new Vector3(callInfo.targetPoint.Value.x, 0f, callInfo.targetPoint.Value.z);

			if (flattenedP != flattenedPoint)
            {
				rot = Quaternion.LookRotation(flattenedPoint - flattenedP, Vector3.up);
            }
        }
		if (!CanSpawnInOtherRooms && Room.FindNearestRoom(p) != callInfo.caster.myRoom)
        {
			return;
        }

		GameObject o = Object.Instantiate(prefab, p, 
			rot);

		SpawnableObject so = o.GetComponent<SpawnableObject>();
		so.mySummoner = callInfo.caster;
		so.target = callInfo.targetPoint ?? new Vector3();
		so.duration = duration.GetValue(callInfo);
		so.startingHealth = OverrideStartingHealth ? StartingHealth.GetValue(callInfo) : 0f;
		so.effect = this;
		so.InheritParentFaction = !DontOverrideFactions;
		if (OverrideStartingHealth)
			so.ImplementStartingHealth();
		NetworkServer.Spawn(o);

		if (!DontOverrideFactions)
        {
			Character c = so.GetComponent<Character>();
			c.MyFaction = callInfo.caster.MyFaction;
			c.StartingFaction = callInfo.caster.MyFaction;
			c.MyAllies = callInfo.caster.MyAllies;
			c.MyEnemies = callInfo.caster.MyEnemies;
        }

		if (StoreCharacterTargetID != null && StoreCharacterTargetID != "")
        {
            callInfo.caster.characterAbilities.RpcAddTarget(callInfo.ability.ID, callInfo.item == null ? "" : callInfo.item.uniqueIdentifier, StoreCharacterTargetID, true, so.GetComponent<Character>());
        }
	}

	public override bool HasLocalOnlyEffect()
	{
		return false;
	}

	public override bool HasServerEffect()
	{
		return true;
	}

	protected override bool AbstractHasLocalEffect()
	{
		return false;
	}

	public override bool CanHitCharacters()
	{
		return SpawnAtCharacter && SpawnAtTarget;
	}

	public override bool CanHitPoints()
	{
		return true;
	}
}
