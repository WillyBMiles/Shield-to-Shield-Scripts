using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

[System.Serializable]
public class PlayAnimation : Effect
{
	public PlayAnimation()
    {
		TargetID = "SELF";
    }

	[Header("Make sure the target has an animationController, or else this won't do anything!")]
	[Tooltip("Note not all characters will necessarily have all animations")]
	[HideIf("@animationClip != null || CycleAnimations")]
	public DefaultAnimation animation;
	[Tooltip("Playmode built in to Animations")]
	public PlayMode playmode = PlayMode.StopAll;
	[Tooltip("Animation speed is multiplied by the normal speed.")]
	public Value animationSpeed = new Value() {  };

	public bool DontScaleSpeedWithAbilityBonus = false;

	[HideIf(nameof(CycleAnimations))]
	[Tooltip("Clip instead of type of animation")]
	public AnimationClip animationClip = null;

	[Tooltip("Should this animation interrupt itself if it is already playing?")]
	public bool interruptSelf = true;

	[Tooltip("Have multiple animations that you cycle.")]
	public bool CycleAnimations = false;

	[ShowIf(nameof(CycleAnimations))]
	[Tooltip("After 1 second of not playing animations, reset the cycle")]
	public bool ResetCyle = true;

	[HideIf("@!CycleAnimations")]
	public List<AnimationClip> animationsToCycle = new List<AnimationClip>();

	[Tooltip("Don't interrupt these.")]
	public List<AnimationClip> dontInterrupt = new List<AnimationClip>();


	public override void LocalEffect(CallInfo callInfo)
	{
		base.LocalEffect(callInfo);
		if (callInfo.target == null)
			return;
		AnimationController animationController = callInfo.target.animationController;
		if (animationController == null)
			return;
		float aSpeed = animationSpeed.GetValue(callInfo);

		if (dontInterrupt != null && dontInterrupt.Count > 0)
		{
			foreach (AnimationClip clip in dontInterrupt)
			{
                if (animationController.IsPlayingAnimation(clip))
				{
					return;
				}
            }
        }
		
		if (!DontScaleSpeedWithAbilityBonus)
		{
			aSpeed = callInfo.caster.CheckAbilityBonus(AbilityBonus.CastTime, callInfo.ability, aSpeed, negate: true);
		}

		if (!CycleAnimations)
        {
			if (animationClip == null)
			{
				animationController.PlayAnimation(animation, playmode, aSpeed, interruptSelf);
			}
			else
			{
				animationController.PlayAnimation(animationClip, EffectID, playmode, aSpeed, interruptSelf);
			}
		}

		if (CycleAnimations)
        {
			string id = GetUniqueID();
			if (ResetCyle)
			{
				string resetID = GetUniqueID() + "_RESET";
				float time = callInfo.target.CheckStoredFloat(callInfo.ability, callInfo.itemID, resetID, 0f, false, false);
				if (Time.time -1f > time)
				{
                    callInfo.target.StoreFloat(callInfo.ability, callInfo.itemID, id, 0, false, false);
                }
				callInfo.target.StoreFloat(callInfo.ability, callInfo.itemID, resetID, Time.time, false, false);
			}
			int thisAnim = (int) (callInfo.target.CheckStoredFloat(callInfo.ability, callInfo.itemID, id, 0f, false, false)) % animationsToCycle.Count;
			AnimationClip clip = animationsToCycle[thisAnim];
			animationController.PlayAnimation(clip, EffectID + thisAnim, playmode, aSpeed, interruptSelf);
			int nextAnim = (thisAnim + 1) % animationsToCycle.Count;
			callInfo.target.StoreFloat(callInfo.ability, callInfo.itemID, id, nextAnim, false, false);

        }
	}

	string GetUniqueID() {
		return "Cycle_" +EffectID + "_" + animationsToCycle.Count;
	}

    protected override bool AbstractHasLocalEffect()
    {
		return true;
    }

    public override bool HasLocalOnlyEffect()
    {
		return false;
    }

    public override bool HasServerEffect()
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
