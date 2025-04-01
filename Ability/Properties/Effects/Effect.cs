using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

[System.Serializable]
public abstract class Effect : Property
{
	public Effect()
	{
		EffectID = GetType().ToString();
		name = EffectID;
	}

	[HideReferenceObjectPicker]
	public AdvancedEffectOptions advancedOptions = new AdvancedEffectOptions();

	[HideReferenceObjectPicker]
	public EffectSyncOptions syncOptions = new EffectSyncOptions();

	[Tooltip("Conditions for this effect")]
	public List<EffectCondition> conditions = new List<EffectCondition>();

	[Tooltip("If there are no delays...don't check the conditions")]
	[BoxGroup("main", false)]
	public bool CheckConditionsBeforeDelay = false;

	[BoxGroup("main", false)]
	public string EffectID;

	[HideInInspector]
	public string UniqueID = "";

	[BoxGroup("main", false)]
	[ValueDropdown(nameof(_targets))]
	[Tooltip("'SELF' always targets the caster, 'CURSOR' always targets the mouse on the ground, 'HIT' ONLY works with callback Timings")]
	public string TargetID = "CURSOR";

	[BoxGroup("main", false)]
	[ValueDropdown(nameof(_targets))]
	[Tooltip("Where the origin of this ability is, point or character")]
	public string OriginID = "SELF";

	[BoxGroup("main", false)]
	[Tooltip("Will get callbacks only from effects with matching callbackIDs (think Projectile callbacks). If no callbacks, safe to leave as empty.")]
	public string CallbackID = "Callback";

	static Dictionary<string, Effect> effectDict = new Dictionary<string, Effect>();

	/// <summary>
	/// Local effects are run on each client, server effects are run once on the server only.
	/// Server effects for things that must be synced (aka damage)
	/// </summary>
	public virtual void LocalEffect(CallInfo callInfo) {
		if (advancedOptions.GenerateCallback)
		{
			CallInfo newInfo = callInfo.Duplicate();
			newInfo.CallbackID = advancedOptions.TriggeringCallback;

            callInfo.activation.OnCallback?.Invoke(newInfo);
        }
			
	}

	protected abstract bool AbstractHasLocalEffect();

	public bool HasLocalEffect()
    {
		return advancedOptions.GenerateCallback || AbstractHasLocalEffect();
    }

	public virtual void LocalOnlyEffect(CallInfo callInfo)
	{ }

	public abstract bool HasLocalOnlyEffect();

	public virtual void ServerEffect(CallInfo callInfo) { }

	public abstract bool HasServerEffect();

	List<Character> tempCharacters;
	List<Vector3> tempPoints;
	List<Character> tempOrigin;
	List<Vector3> tempOriginPoints;
	public override void Activate(CallInfo callInfo)
	{
		base.Activate(callInfo);

		ActivateWrapper(callInfo);
	}

	public abstract bool CanHitCharacters(); //Can this effect EVER hit characters 
	public abstract bool CanHitPoints(); //Can this effect EVER hit points (e.g. like a status or damage could never hit a point)


	static List<float> tempDelays = new List<float>();
	public void ActivateWrapper(CallInfo callInfo, List<Character> HITcharacters = null, List<Vector3> HITpoints = null,

		List<Character> OriginHitCharacters = null, List<Vector3> OriginHitPoints = null
		)
    {
        CallInfo newInfo = callInfo.Duplicate();
        newInfo.effect = this;

        if (advancedOptions.UpdateTargetsWhenCalledback)
        {
            newInfo.activation.ForceUpdateTargets(callInfo);
        }



		if ((!advancedOptions.MatchTargetIndexToDelayIndex || !advancedOptions.AutoDelay) && (advancedOptions.delays == null || advancedOptions.delays.Count == 0))
		{
			List<Character> targetCharacters = null;
			List<Vector3> targetPoints = null;

			if (TargetID == "HIT")
            {
				targetCharacters = HITcharacters;
				targetPoints = HITpoints;
            }
			if (TargetID == "ORIGIN HIT")
            {
				targetCharacters = OriginHitCharacters;
				targetPoints = OriginHitPoints;
            }

			if (OriginID == "HIT")
            {
                newInfo.origin = HITcharacters.Count > 0 ? HITcharacters[0] : null;
				if (HITpoints.Count > 0)
                    newInfo.originPoint = HITpoints[0];
            }
			if (OriginID == "ORIGIN HIT")
			{
                newInfo.origin = OriginHitCharacters.Count > 0 ? OriginHitCharacters[0] : null;
				if (HITpoints.Count > 0)
                    newInfo.originPoint = OriginHitPoints[0];
			}


			ActualActivation(newInfo, targetCharacters, targetPoints, -1);
			
		}
		else
		{
			List<Character> delayCharacters = null;
			List<Vector3> delayPoints = null;
			if (advancedOptions.RetainTargetOnDelay)
			{
				delayCharacters = new List<Character>();
				delayPoints = new List<Vector3>();

				
				if (TargetID == "HIT")
                {
					delayCharacters.AddRange( HITcharacters);
					delayPoints.AddRange(HITpoints);
                }
				else
                {
					FindTargets(newInfo, delayCharacters, delayPoints);
				}
				if (OriginID == "HIT")
                {
					newInfo.origin = HITcharacters.Count == 0 ? null : HITcharacters[0];
                    newInfo.originPoint = HITpoints.Count == 0 ? Vector3.negativeInfinity : HITpoints[0];
				}else
                {
                    newInfo.caster.characterAbilities.GetTargets(newInfo, tempOrigin, OriginID);
                    newInfo.caster.characterAbilities.GetTargets(newInfo, tempOriginPoints, OriginID);
                    newInfo.origin = tempOrigin.Count == 0 ? null : tempOrigin[0];
                    newInfo.originPoint = tempOriginPoints.Count == 0 ? Vector3.negativeInfinity : tempOriginPoints[0];
				}

				if (CheckConditionsBeforeDelay)
                {
					Character[] cs = delayCharacters.ToArray();
					foreach (Character c in cs)
					{
						CallInfo testCallInfo = newInfo.Duplicate();
						testCallInfo.target = c;

						if (!CanDoEffect(testCallInfo))
							delayCharacters.Remove(c);
					}
				}
			}

			if (!advancedOptions.RetainTargetOnDelay && CheckConditionsBeforeDelay)
			{
				if (!CanDoEffect(newInfo))
					return;
			}
			int nextIndex = 0;
			if (tempDelays == null)
			{
				tempDelays = new List<float>();
			}
			tempDelays.Clear();

			if (advancedOptions.AutoDelay && advancedOptions.MatchTargetIndexToDelayIndex)
			{

				float current = 0f;
				float max = Mathf.Max(delayCharacters == null ? 0f : delayCharacters.Count, delayPoints == null ? 0f : delayPoints.Count);
                for (int i = 0; i < max; i++)
				{
                    CallInfo testCallInfo = newInfo.Duplicate();
                    testCallInfo.target = newInfo.target = delayCharacters == null ? null : (delayCharacters.Count > i ? delayCharacters[i] : null);


                    float offset = advancedOptions.AutoDelayTime.GetValue(testCallInfo);
					current += offset;
					tempDelays.Add(current);
					if (!advancedOptions.AutoDelaySum)
					{
						current = 0f;
					}
				}
			}
			else
			{
                foreach (Value delay in advancedOptions.delays)
				{
					tempDelays.Add(delay.GetValue(callInfo));
				}

            }

			foreach (float delay in tempDelays)
			{
				callInfo.caster.characterAbilities.DelayEffect(new DelayHolder()
				{
					callInfo = newInfo,
					delay = delay,

					canBeInterrupted = advancedOptions.DelayCanBeInterrupted,

					storeCharacters = delayCharacters,
					storePoints = delayPoints,
					index = nextIndex++,
				});
			}

		}

	}

	public class DelayHolder
	{
		public CallInfo callInfo;

		public List<Character> storeCharacters;
		public List<Vector3> storePoints;

		public float delay;
		public bool canBeInterrupted;


        public int index; //in the delay list

	}

	public void ActualActivation(CallInfo callInfo, List<Character> targets, List<Vector3> points, int delayIndex)
	{
		if (callInfo.caster == null)
			return;
		if (advancedOptions.LimitByUpgrade)
        {
			if (callInfo.caster.playerSkills == null)
				return;
			if (callInfo.ability.mySkill == null)
				return;
			byte lvl = callInfo.ability.mySkill.GetUpgradeLevel(advancedOptions.upgradeID, callInfo.caster.playerSkills);
			if (lvl > advancedOptions.maxUpgradeLevel || lvl < advancedOptions.minUpgradeLevel)
				return;
        }

		if (callInfo.origin == null)
        {
			callInfo.caster.characterAbilities.GetTargets(callInfo, tempOrigin, OriginID);

			callInfo.origin = tempOrigin.Count == 0 ? null : tempOrigin[0];

		}
		if (callInfo.originPoint == null)
        {
			callInfo.caster.characterAbilities.GetTargets(callInfo, tempOriginPoints, OriginID);
			callInfo.originPoint = tempOriginPoints.Count == 0 ? new Vector3(Mathf.NegativeInfinity, Mathf.NegativeInfinity, Mathf.NegativeInfinity) : tempOriginPoints[0];
		}

		if (targets == null || points == null)
		{
			FindTargets(callInfo, tempCharacters, tempPoints);
			DoMyEffect(callInfo, tempCharacters, tempPoints, delayIndex);
		}
		else
		{
			DoMyEffect(callInfo, targets, points, delayIndex);
		}
		
	}

	public void FindTargets(CallInfo callInfo, 
		List<Character> charactersToReplace, List<Vector3> pointsToReplace)
	{
		if (callInfo.caster == null)
			return;

		if (advancedOptions.TargetIDs != null && advancedOptions.TargetIDs.Length > 0)
		{
			string[] combined = new string[advancedOptions.TargetIDs.Length + 1];
			System.Array.Copy(advancedOptions.TargetIDs, combined, advancedOptions.TargetIDs.Length);
			combined[advancedOptions.TargetIDs.Length] = TargetID;
            callInfo.caster.characterAbilities.GetTargets(callInfo, charactersToReplace, combined);
            callInfo.caster.characterAbilities.GetTargets(callInfo, pointsToReplace, combined);
		}
		else
		{
            callInfo.caster.characterAbilities.GetTargets(callInfo, charactersToReplace, TargetID);
            callInfo.caster.characterAbilities.GetTargets(callInfo, pointsToReplace, TargetID);
		}

	}


	/// <summary>
	/// May be destructive to the given lists.
	/// </summary>
	/// <param name="character"></param>
	/// <param name="ability"></param>
	/// <param name="activation"></param>
	/// <param name="targets"></param>
	/// <param name="points"></param>
	private void DoMyEffect(CallInfo callInfo,
		List<Character> targets, List<Vector3> points, int delayIndex)
	{
		if (advancedOptions.PointsOnly)
			targets.Clear();
		if (advancedOptions.CharactersOnly)
			points.Clear();
		CallInfo newInfo = callInfo.Duplicate();
		newInfo.effect = this;
		if (delayIndex != -1 && advancedOptions.MatchTargetIndexToDelayIndex)
        {
			Character target = targets.Count > delayIndex ? targets[delayIndex] : null;
			Vector3? point = points.Count > delayIndex ? points?[delayIndex] : null;

			Character[] targetArray = target == null ? new Character[0] : new[] { target };
			Vector3[] pointArray = point.HasValue ? new[] { point.Value } : new Vector3[0];
			if (target != null || point != null)
            {
				callInfo.caster.characterAbilities.DoEffect(newInfo, targetArray, pointArray,
				advancedOptions.BatchPointsAndCharacters);
			}
		}
		else
        {
			callInfo.caster.characterAbilities.DoEffect(newInfo, targets.ToArray(), points.ToArray(),
				advancedOptions.BatchPointsAndCharacters);
		}

	}

	public override void Initialize(Activation activation)
	{
		base.Initialize(activation);
		if (conditions != null)
        {
			foreach (EffectCondition con in conditions)
			{
				if (con != null)
					con.Initialize(activation);
			}
		}

		tempCharacters = new List<Character>();
		tempPoints = new List<Vector3>();
		tempOrigin = new List<Character>();
		tempOriginPoints = new List<Vector3>();
	}



	List<Character> hitCharacters;
	List<Vector3> hitPoints;
	List<Character> originHitCharacters;
	List<Vector3> originHitPoints;
	protected override void OnHitCallback(CallInfo callInfo)
	{

		if (hitCharacters == null)
			hitCharacters = new List<Character>();
		if (hitPoints == null)
			hitPoints = new List<Vector3>();
		if (originHitCharacters == null)
			originHitCharacters = new List<Character>();
		if (originHitPoints == null)
			originHitPoints = new List<Vector3>();

		if (callInfo.CallbackID == CallbackID || this.advancedOptions.CallbackIDs.Contains(CallbackID))
		{
			hitCharacters.Clear();
			hitCharacters.Add(callInfo.target);
			hitPoints.Clear();
			if (callInfo.targetPoint.HasValue)
				hitPoints.Add(callInfo.targetPoint.Value);

			originHitCharacters.Clear();
			originHitCharacters.Add(callInfo.origin);
			originHitPoints.Clear();
			if (callInfo.originPoint.HasValue)
				originHitPoints.Add(callInfo.originPoint.Value);


			ActivateWrapper(callInfo, hitCharacters, hitPoints, originHitCharacters, originHitPoints);
		}
			

	}

	protected override void BasicCallback(CallInfo callInfo)
	{

		if (hitCharacters == null)
			hitCharacters = new List<Character>();
		if (hitPoints == null)
			hitPoints = new List<Vector3>();

		if (this.CallbackID == callInfo.CallbackID || this.advancedOptions.CallbackIDs.Contains(CallbackID))
		{
			hitCharacters.Clear();
			hitCharacters.Add(callInfo.target);
			hitPoints.Clear();
			if (callInfo.targetPoint.HasValue)
				hitPoints.Add(callInfo.targetPoint.Value);
			ActivateWrapper(callInfo, hitCharacters, hitPoints);

		}

	}

	public byte GetID(Activation activation)
	{
		byte i = 0;
		foreach (Effect effect in activation.effects)
		{
			if (effect == this)
				return i;
			i++;
		}
		return byte.MaxValue;
	}

	public bool CanDoEffect(CallInfo callInfo)
	{
		foreach (EffectCondition condition in conditions)
		{
			if (!condition.CheckCondition(callInfo))
				return false;
		}
		return true;
	}
}

[GUIColor(.7f, .7f, .7f, 1f)]
[BoxGroup("AdvancedOptions", false)]
public class AdvancedEffectOptions
{
	[Tooltip("Use for multiple callbacks.")]
	public List<string> CallbackIDs = new List<string>();
	[Tooltip("If false: do one effect per target (point or effect), If true: do one effect per target point/effect group (ignore nongrouped targets)")]
	public bool BatchPointsAndCharacters = false;

	[MyBox.ConditionalField(nameof(CharactersOnly), true)]
	[Tooltip("If true only target points")]
	public bool PointsOnly = false;

	[MyBox.ConditionalField(nameof(PointsOnly), true)]
	[Tooltip("If true only target characters")]
	public bool CharactersOnly = false;

    [Tooltip("Use for multiple targetIDs")]
	public string[] TargetIDs = new string[0];

	[Tooltip("The first target is the first delay, the second target is the second delay and so on and so forth.")]
	public bool MatchTargetIndexToDelayIndex = false;

	[Tooltip("Ignore delays, instead have a delay set by the number of targets given.")]
	[ShowIf(nameof(MatchTargetIndexToDelayIndex))]
	public bool AutoDelay = false;

	[Tooltip("If true add the offsets to each other, otherwise compute them separately")]
    [ShowIf("@MatchTargetIndexToDelayIndex && AutoDelay")]
    public bool AutoDelaySum = false;

	[ShowIf("@MatchTargetIndexToDelayIndex && AutoDelay")]
	public Value AutoDelayTime = new Value();

    [HideIf("@MatchTargetIndexToDelayIndex && AutoDelay")]
    [Tooltip("Add a delay to this effect of this value in seconds, each represents a separate instance of this going off")]
	public List<Value> delays = new List<Value>();

	[Tooltip("When delayed, can be interrupted: clearing the delay. Use when delays match animations etc.")]
	public bool DelayCanBeInterrupted = false;

	[Tooltip("false : after delay goes off find target;; true : at activation find target, use same target when delay goes off")]
	public bool RetainTargetOnDelay = false;


	[Tooltip("Can only be hit by this ability once per...how many seconds?")]
	public Value CanOnlyBeHitByThisEffectOncePer = new Value() { baseMult = 0f };

	[Tooltip("Leave as empty string to only affect this effect")]
	public string CanOnlyHitBySharedValue = "";


	public Value CanOnlyHappenOncePer { get; set; } = new Value() { baseMult = 0f };

	[Tooltip("Only happens once per casting. If casting is rapid, may not work as expected.")]
	public bool OnlyOncePerInstance = false;

	[Tooltip("Update targets whenever this is calledback, only works on Update or TargetWhenTriggered timings")]
	public bool UpdateTargetsWhenCalledback = false;

	[Tooltip("Generate a callback. Use Timing Basic Callback to get trigger")]
	public bool GenerateCallback = false;

	[ShowIf(nameof(GenerateCallback))]
	[Tooltip("CallbackID to trigger")]
	public string TriggeringCallback = "Callback";

	[Tooltip("Only works for skills.")]
	public bool LimitByUpgrade = false;

	[Tooltip("UpgradeID for the skill")]
	[ShowIf(nameof(LimitByUpgrade))]
	public UpgradeID upgradeID = UpgradeID.DAMAGE_A;

	[Tooltip("If level is lower than this effect doesn't happen")]
	[ShowIf(nameof(LimitByUpgrade))]
	public byte minUpgradeLevel = 1;

	[Tooltip("If level is higher than this, this effect does not happen")]
	[ShowIf(nameof(LimitByUpgrade))]
	public byte maxUpgradeLevel = 255;

}


[GUIColor(.7f, .7f, .7f, 1f)]
[BoxGroup("SyncOptions", false)]
public class EffectSyncOptions
{
	[Tooltip("Don't sync at all, run locally only.")]
	public bool DontSync = false;

	[Tooltip("Will only work if being called-back")]
	public bool ListenForLocalCallback = false;
}