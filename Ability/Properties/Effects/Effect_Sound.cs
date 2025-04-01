using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class Effect_Sound : Effect
{
	[HideIf("@(clips != null && clips.Count > 0) || (SoundID.Length != 0 && CancelSound)")]
	[Tooltip("Clip to play")]
	public AudioClip clip;

	[HideIf("@SoundID.Length != 0 && CancelSound")]
	[Tooltip("Ignore clip above, play one of these at random instead.")]
	public List<AudioClip> clips = new List<AudioClip>();

	[HideIf("@SoundID.Length != 0 && CancelSound")]
	[Tooltip("Volume to play the clip at")]
	public float volume = 1f;

	[HideIf("@SoundID.Length != 0 && CancelSound")]
	[Tooltip("Target characters instead of points")]
	public bool TargetCharactersInstead = false;

	[HideIf("@(SoundID.Length != 0 && CancelSound) || !TargetCharactersInstead")]
	[Tooltip("What's the volume played if you control the target, this is a multiplier")]
	public float NonLocalTargetVolumeModifier = 1f;

	[HideIf("@(SoundID.Length != 0 && CancelSound)")]
	[Tooltip("Play at origin point instead of target")]
	public bool PlayAtOrigin = false;

	[HideIf("@SoundID.Length != 0 && CancelSound")]
	[Tooltip("What's the volume played if you don't control this character, this is a multiplier")]
	public float NonLocalVolumeModifier = 1f;

	[Tooltip("What's this sound's ID, useful for cancelling the sound later.")]
	public string SoundID = "";

	[HideIf("@SoundID.Length == 0")]
	[Tooltip("Cancel the sound instead of playing it")]
	public bool CancelSound = false;

	[HideIf("@SoundID.Length == 0")]
	public bool Loop = false;

	[Tooltip("How long does the clip last?")]
	[HideIf("@SoundID.Length != 0 && (Loop || CancelSound)")]
	public Value Length = new Value() { baseMult = float.PositiveInfinity };

	[Tooltip("Time in seconds")]
	[HideIf("@CancelSound || Loop")]
	public Value FadeOutTime = new Value() { baseMult = 0f };

	public override void LocalEffect(CallInfo callInfo)
	{
		base.LocalEffect(callInfo);
		if (CancelSound && SoundID != "")
		{
			SoundManager.StopSound(SoundID, callInfo.caster);
			return;
		}

		AudioClip myClip = clip;
		if (clips != null && clips.Count > 0)
		{
			myClip = clips[Random.Range(0, clips.Count)];
		}
		if (myClip == null)
			return;

		float length = ((SoundID.Length == 0 && Loop) || Length == null) ? float.PositiveInfinity : Length.GetValue(callInfo);
		float finalVolume = callInfo.caster.IsAuthoritative() ? volume : volume * NonLocalVolumeModifier;
		float fadeOut = FadeOutTime == null ? 0f : FadeOutTime.GetValue(callInfo);

		if (TargetCharactersInstead)
		{
			Character c = PlayAtOrigin ? callInfo.origin : callInfo.target;
			if (c == null)
				return;

			finalVolume = c.IsAuthoritative() ? finalVolume : NonLocalTargetVolumeModifier * finalVolume;

			SoundManager.PlaySound(myClip, c.transform.position, finalVolume, SoundID, Loop, callInfo.caster, length, fadeOut, c);
		}
		else
		{
			Vector3? place = PlayAtOrigin ? callInfo.originPoint : callInfo.targetPoint;
			if (!place.HasValue)
				return;

			SoundManager.PlaySound(myClip, place.Value, finalVolume, SoundID, Loop, callInfo.caster, length, fadeOut);
		}
		
		
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
		return TargetCharactersInstead;
	}

	public override bool CanHitPoints()
	{
		return !TargetCharactersInstead;
	}
}
