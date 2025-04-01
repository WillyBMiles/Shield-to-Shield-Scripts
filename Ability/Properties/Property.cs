using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;


[System.Flags]
public enum Timing
{
	None = 0,
	WindUp = 1,
	WindDown = 2,
	Cast = 4,
	Update = 8,
	Press = 16,
	Hold = 32,
	Release = 64,
	StartWindUp = 128,
	EndWindDown = 256,
	ProjectileHit = 512,
	ProjectileDestroy = 1024,
	PreparingAbility = 2048,
	ReadyToCast = 4096,
	NotPreparingAbility = 8192,
	BasicCallback = 16384,
    OnHitDamage = 32768,
	//PLACEHOLDER = 65536,
	TargetWhenTriggered = 131072,
	Interrupt = 262144,
	//Adding a new timing??
	//Dont forget to add a callback
	//a reference to the callback here
	//AND null it in activation.initialize
}

[System.Serializable]
public abstract class Property 
{
	public string name;

	[Tooltip("Use Projectile timings only if there is a projectile effect on this ability somewhere. " +
		"If this is an effect, projectile hit and destroy only callback if the projectile's TargetOverride is the same as this's TargetID.")]
	public Timing timing = Timing.Cast;

	[HideInInspector, SerializeField]
	public List<string> _targets = new List<string>() { "SELF", "HIT", "ORIGIN HIT"};

	public static void AddActivationCallbacks(Timing timing, Activation activation, Activation.TimingCallback activate)
	{
		if (CheckTiming(timing, Timing.Cast))
			activation.OnCast += activate;
		if (CheckTiming(timing, Timing.WindUp))
			activation.OnWindUp += activate;
		if (CheckTiming(timing, Timing.WindDown))
			activation.OnWindDown += activate;
		if (CheckTiming(timing, Timing.WindUp))
			activation.OnWindUp += activate;
		if (CheckTiming(timing, Timing.Hold))
			activation.OnHold += activate;
		if (CheckTiming(timing, Timing.Press))
			activation.OnPress += activate;
		if (CheckTiming(timing, Timing.Release))
			activation.OnRelease += activate;
		if (CheckTiming(timing, Timing.Update))
			activation.OnUpdate += activate;
		if (CheckTiming(timing, Timing.StartWindUp))
			activation.OnStartWindUp += activate;
		if (CheckTiming(timing, Timing.EndWindDown))
			activation.OnEndWindDown += activate;
		if (CheckTiming(timing, Timing.PreparingAbility))
			activation.OnPreparingCast += activate;
		if (CheckTiming(timing, Timing.ReadyToCast))
			activation.OnReadyToCast += activate;
		if (CheckTiming(timing, Timing.NotPreparingAbility))
			activation.OnNotPreparingCast += activate;
		if (CheckTiming(timing, Timing.Interrupt))
			activation.OnInterrupt += activate;
	}

	public static void AddHitCallbacks(Timing timing, Activation activation, Activation.BasicCallback activate)
	{
		if (CheckTiming(timing, Timing.ProjectileHit))
			activation.OnHit += activate;
		if (CheckTiming(timing, Timing.ProjectileDestroy))
			activation.OnDestroy += activate;

		if (CheckTiming(timing, Timing.OnHitDamage))
			activation.OnHitDamage += activate;
	}

	public virtual void Initialize(Activation activation)
	{
		AddActivationCallbacks(timing, activation, Activate);

		AddHitCallbacks(timing, activation, OnHitCallback);

		if (CheckTiming(timing, Timing.BasicCallback))
			activation.OnCallback += BasicCallback;

		/* old system
		switch (timing)
		{
			case Timing.Cast:
				activation.OnCast += Activate;
				break;
			case Timing.WindUp:
				activation.OnWindUp += Activate;
				break;
			case Timing.WindDown:
				activation.OnWindDown += Activate;
				break;
			case Timing.Hold:
				activation.OnHold += Activate;
				break;
			case Timing.Press:
				activation.OnPress += Activate;
				break;
			case Timing.Release:
				activation.OnRelease += Activate;
				break;
			case Timing.Update:
				activation.OnUpdate += Activate;
				break;
			case Timing.StartWindUp:
				activation.OnStartWindUp += Activate;
				break;
			case Timing.EndWindDown:
				activation.OnEndWindDown += Activate;
				break;
			case Timing.ProjectileHit:
				activation.OnHit += OnHitCallback;
				break;
			case Timing.ProjectileDestroy:
				activation.OnDestroy += OnHitCallback;
				break;

		}*/
		activation.OnUpdate += UpdateThis;
	}

	private static bool CheckTiming(Timing bitMask, Timing timing)
	{
		return ((bitMask & timing) == timing);
	}

	public virtual void Activate(CallInfo callInfo)
	{
		//NOOP
	}

	protected virtual void UpdateThis(CallInfo callInfo)
	{
		//NOOP
	}

	public virtual void UpdateTargets(List<string> targets)
    {
		_targets.Clear();
		_targets.AddRange(targets);
    }

	protected virtual void OnHitCallback(CallInfo callInfo)
	{
		//Override without calling if you're going to override
		Activate(callInfo);
		
	}

	protected virtual void BasicCallback(CallInfo callInfo)
	{
		//Override without calling if you're going to override
		Activate(callInfo);

	}

#if UNITY_EDITOR
	public virtual void RecalculateSubobjects()
	{
		//NOOP by default
	}
#endif


}
