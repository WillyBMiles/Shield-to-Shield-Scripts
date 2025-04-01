using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Effect_Heal : Effect
{
	[Tooltip("If revive is set to true, awaken instead of heal. Recommend only targetting the CollapsedCharacters layer. They awaken with amount health.")]
	public bool Revive;
    public Value Amount = new Value();

	public bool ShowHealingText = true;
	[Tooltip("Don't heal characters with the OneDamageAtATimeAttribute")]
	public bool DontHitOneDamageAtAtime = false;

    public override void LocalOnlyEffect(CallInfo callInfo)
    {
        base.LocalEffect(callInfo);
		if (!callInfo.caster.IsAuthoritative())
			return;
		if (callInfo.target == null)
			return;
		if (DontHitOneDamageAtAtime && callInfo.target.OneDamageAtATime)
			return;
		float amount = Amount.GetValue(callInfo);
		if (Revive)
		{
			callInfo.target.Revive(amount);
		}
		else
		{
			callInfo.target.Heal(callInfo.caster.ID, amount, ShowHealingText, true);
		}

	}

	public override bool HasLocalOnlyEffect()
	{
		return true;
	}

	public override bool HasServerEffect()
	{
		return false;
	}

	protected override bool AbstractHasLocalEffect()
	{
		return false;
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
