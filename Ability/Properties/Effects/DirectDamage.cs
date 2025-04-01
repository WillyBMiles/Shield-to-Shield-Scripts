using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class DirectDamage : Effect
{
	public Damage damage = new Damage();

	public bool TriggersDamageOnHit;

	[Tooltip("Only apply by the character hit. Use with SyncOptions.")]
	public bool ApplyByCharacterHit = false;

	

	public override void ServerEffect(CallInfo callInfo)
	{
		base.ServerEffect(callInfo);
		if (callInfo.target == null || ApplyByCharacterHit)
			return;
		if (callInfo.caster.IsPlayerCharacter)
			return;
		if (!callInfo.target.IsPlayerCharacter || callInfo.target.IsAuthoritative())
        {
			damage.Apply(callInfo, TriggersDamageOnHit);
		}
		
	}

	public override void LocalEffect(CallInfo callInfo)
	{
		base.LocalEffect(callInfo);
		if (callInfo.target == null)
			return;

		if (callInfo.caster.IsPlayerCharacter && callInfo.caster.IsAuthoritative())
        {
			damage.LocalApply(callInfo, TriggersDamageOnHit);
        }
		else if (callInfo.target.IsAuthoritative() && (ApplyByCharacterHit || (!callInfo.target.isServer && callInfo.target.IsPlayerCharacter)))
        {
			damage.LocalApply(callInfo, TriggersDamageOnHit);
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

#if UNITY_EDITOR
	/*no longer needed
	public override void RecalculateSubobjects()
	{
		base.RecalculateSubobjects();
		damage = null;
		string grossPath = AssetDatabase.GetAssetPath(this);
		string path = grossPath.Substring(0, grossPath.LastIndexOf("/"));
		string[] subFolders = AssetDatabase.GetSubFolders(path);
		string[] guids = AssetDatabase.FindAssets("t:Damage", subFolders);
		foreach (string guid in guids)
		{
			string assetPath = AssetDatabase.GUIDToAssetPath(guid);
			damage = AssetDatabase.LoadAssetAtPath<Damage>(assetPath);
		}
	}
	*/
#endif
}
