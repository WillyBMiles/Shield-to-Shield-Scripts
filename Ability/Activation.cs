using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Dynamic;


[HideReferenceObjectPicker]
[CreateAssetMenu(fileName = "New Activation", menuName = "Ability/Activation")]
[System.Serializable]
public class Activation : SerializedScriptableObject
{
	[Header("IDs must be UNIQUE")]
	public string ID;

	[Tooltip("Ignore control")]
	public bool CastAutomatically = false;

	[FoldoutGroup("Winding")]
	[Tooltip("During windup/winddown can't cast any other abilities")]
	public Value WindUp = new Value { baseMult = 0f };
	[FoldoutGroup("Winding")]
	[Tooltip("During windup/winddown can't cast any other abilities")]
	public Value WindDown = new Value { baseMult = 0f };

	[FoldoutGroup("Winding")]
	public bool WindingDontScaleWithAbilityBonus = false;

	[FoldoutGroup("Cursor")]
	[Tooltip("Minimum distance CURSOR is from caster")]
	public float MinCursorDistance;

	[FoldoutGroup("Cursor")]
	[Tooltip("Maximum distance CURSOR is from caster")]
	public float MaxCursorDistance = Mathf.Infinity;

	[FoldoutGroup("Cursor")]
	public bool LockCursorDuringWindUp = true;

	[FoldoutGroup("Cursor")]
	public bool IndicateRangeOnUI = false;
	[FoldoutGroup("Cursor")]
	public bool IndicatePointOnUI = false;
	[FoldoutGroup("Cursor")]
	[ShowIf(nameof(IndicatePointOnUI))]
	public float IndicatePointWidth = 1;
	[FoldoutGroup("Cursor")]
	public bool PointToCursorOnUI = false;
	[ShowIf(nameof(PointToCursorOnUI))]
	[FoldoutGroup("Cursor")]
	public Value ArrowWidth = new Value();
	[FoldoutGroup("Cursor")]
	public Timing PointTowardsCursor = Timing.StartWindUp;
	[FoldoutGroup("Cursor")]
	public bool PointTowardsWhilePassive = false;

	[FoldoutGroup("Static Cooldown")]
	[Tooltip("Use as a normal cooldown. Can also use Coooldown limitation for more options")]
	public Value StaticCooldown = new Value() { baseNumber = 0f, baseMult = 0f, type = Value.Type.Number, };

	[FoldoutGroup("Static Cooldown")]
	[Tooltip("Can't prepare if on static cooldown.")]
	public bool CantPrepareStaticCooldown = true;

	[FoldoutGroup("Static Cooldown")]
	[Tooltip("Stored float ID of the cooldown")]
	public string CooldownID = "Cooldown";

	[FoldoutGroup("Static Cooldown")]
	[Tooltip("Stored float ID of the cooldown...is it global or local?")]
	public bool CooldownGlobal = false;

	[ShowIf(nameof(CooldownGlobal))]
	[FoldoutGroup("Static Cooldown")]
	[Tooltip("Stored float ID of the cooldown...is it global or local?")]
	public bool CooldownSave = false;

	[FoldoutGroup("Static Cooldown")]
	[Tooltip("Start on Cooldown?")]
	public bool CooldownStartOnCooldown = false;

	[ShowIf(nameof(CooldownStartOnCooldown))]
	[FoldoutGroup("Static Cooldown")]
	[Tooltip("Start with a number between 0 and max cooldown on cooldown")]
	public bool CooldownStartWithRandomCooldown = false;

	[FoldoutGroup("Static Cooldown")]
	[Tooltip("When does the cooldown start, only use normal timings, no callbacks")]
	public Timing CooldownTiming = Timing.Cast;

	[FoldoutGroup("Static Cooldown")]
	[Tooltip("Dont use special unique ID for static")]
	public bool DontUseUniqueID = false;

	[FoldoutGroup("Static Cooldown")]
	public bool CooldownDontScaleWithAbilityBonus = false;

    [FoldoutGroup("Stamina")]
	public Value StaminaCost = new Value() { baseMult = 1f };
	[FoldoutGroup("Stamina")]
	public Timing StaminaActivation = Timing.Cast;
	[FoldoutGroup("Stamina")]
	public bool DontCostStaminaWhilePassive = true;
	[FoldoutGroup("Stamina")]
	public bool DontResetStaminaTimer = false;
	[FoldoutGroup("Stamina")]
	public bool StaminaDontUseAbilityBonus = false;
	[FoldoutGroup("Stamina")]
    public bool StaminaGainDontUseAbilityBonus = false;
    [FoldoutGroup("Stamina")]
    public bool StaminaGainCDDontUseAbilityBonus = false;

	[FoldoutGroup("Animation")]
	[Header("ONLY USE ON SIMPLE ABILITIES FOR NOW")]
	[Tooltip("Plays on caster, with standard settings, Use PlayAnimation for more options.")]
	public AnimationClip animationClip = null;
	[FoldoutGroup("Animation")]
	public Value animationSpeed = new Value() { mainSubValue = new SubValue() { type = SubValue.Type.Trait, traitType = Trait.AttackSpeed } };
	[FoldoutGroup("Animation")]
	public Timing animationTiming = Timing.StartWindUp;
	[FoldoutGroup("Animation")]
	public bool AnimationDontScaleWithAbilityBonus = false;

	[FoldoutGroup("Sound")]
	[Header("ONLY USE ON SIMPLE ABILITIES FOR NOW")]
	[Tooltip("Plays nonlooping, at caster's position, with standard settings. Use Effect_Sound for more options")]
	public AudioClip soundClip = null;
	[FoldoutGroup("Sound")]
	public Value soundVolume = new Value() { baseMult = .2f};
	[FoldoutGroup("Sound")]
	public Timing soundTiming = Timing.Cast;

	[FoldoutGroup("Button Info")]
	[Tooltip("Hold the button down to cast repeatedly, instead of on release")]
	public bool HoldButtonToCast = false;

	[FoldoutGroup("Button Info")]
	[Tooltip("If set to false, other abilities being cast interrupt this.")]
	public bool SetCastingAbility = true;
	[FoldoutGroup("Button Info")]
	[Tooltip("If set to false, it doesn't interrupt other ability's UI.")]
	public bool SetPreparingAbility = true;
	[FoldoutGroup("Button Info")]
	public bool DontBuffer = false;
	[ShowIf(nameof(SetCastingAbility))]
	[FoldoutGroup("Button Info")]
	public bool DontBufferOnSelf = false;
	[HideIf(nameof(HoldButtonToCast))]
	[Tooltip("Cast on press instead of release.")]
	[FoldoutGroup("Button Info")]
	public bool CastOnPress = false;

    [FoldoutGroup("Cast Conditions")]
	[Tooltip("Ignores status effects and stagger.")]
	public bool CastAnyTime = false;
	[FoldoutGroup("Cast Conditions")]
	[Tooltip("Can cast if dead or collapsed")]
	public bool CanCastIfDead = false;
	[FoldoutGroup("Cast Conditions")]
	[ShowIf(nameof(CanCastIfDead))]
	[Tooltip("Can ONLY cast if dead or collapsed")]
	public bool CanOnlyCastIfDead = false;
	[FoldoutGroup("Cast Conditions")]
	public bool CantBeInterrupted = false;

	[FoldoutGroup("Display")]
	[Tooltip("If this can prepare then show that this ability can be cast.")]
	public bool ShowCanCastIfCanPrepare = false;

	[FoldoutGroup("Display")]
	[ShowIf(nameof(ShowCanCastIfCanPrepare))]
	[Tooltip("If this can prepare and all activations above it can't, then show this icon instead")]
	public Sprite ReplaceIcon;

	[FoldoutGroup("Sync Options")]
	[Tooltip("Dont send any commands, only run locally. This includes Server effects like Damage. Even if this is on the server.")]
	public bool DontSyncAnything = false;

	[HideIf(nameof(DontSyncAnything))]
	[FoldoutGroup("Sync Options")]
	[Tooltip("Dont send any commands, only run locally on each client, including all logic related to it. ")]
	public bool RunLocallyOnClient = false;

	[HideIf(nameof(DontSyncAnything))]
	[FoldoutGroup("Sync Options")]
	[Tooltip("Send all commands reliably, default behaviour is unreliable.")]
	public bool SendReliably = false;

	


	public List<Target> targets = new List<Target>();
	public List<Limitation> limitations = new List<Limitation>();
	public List<Effect> effects = new List<Effect>();

	static Dictionary<string, Activation> activations = new Dictionary<string, Activation>();

	#region Simple Activation Syncing
	[HideInInspector]
	public bool isSimple = false;

	/// <summary>
	/// A simple activation is a "fire and forget" style activation.
	/// Where once it starts winding up the whole ability is deterministic until wind down, and it does nothing outside of that.
	/// 
	/// To run a simple activation: 
	/// 1. Wait for a trigger (button press e.g.)
	/// 2. Run start windup
	/// 3. Wait windup time (run wind up)
	/// 4. Run cast
	/// 5. Wait winddown time (run wind down)
	/// 6. Run endwinddown
	/// 
	/// For the case of syncing, step 1 will be triggered by a message
	/// </summary>
	public string InitializeSimple()
	{
		isSimple = true;
		if (OverrideThisIsNotSimple)
		{

			isSimple = false;
			return "Override this is not simple.";
		}
		if (!_ability.LockCursorForAll && !LockCursorDuringWindUp) //cursor must lock during windup
		{
			isSimple = false;
			return "Cursor not locked";
		}

		Timing simpleTiming = Timing.StartWindUp | Timing.WindUp | Timing.Cast | Timing.WindDown |
		Timing.ProjectileDestroy | Timing.ProjectileHit | Timing.BasicCallback;
		foreach (Target t in targets)
		{
			if ((!t.IsDeterministic() && t.timing != Timing.StartWindUp))
            {
				isSimple = false;
				return t.name + " must be deterministic or start windup.";
			} 
			if ((t.timing & (simpleTiming | Timing.Update)) != t.timing)
			{
				isSimple = false;
				return t.name + " must have a simple timing.";
				
			}

		}
		foreach (Effect e in effects)
		{
			if (e == null)
			{
				Debug.Log("\"" + name + "\" has unclean effects.");
				continue;
			}

			if ((e.timing & simpleTiming) != e.timing) //must have one of the above timings
			{
				isSimple = false;
				return e.name + " must have a simple timing.";
			}
			/* //maybe effect conditions are ok!
			if (e.conditions.Count > 0) //must have no conditions on its effects
			{
				isSimple = false;
				return e.name + " can't have effect conditions.";
			}
			*/
			bool found = false;
			switch (e.TargetID)
			{
				case "SELF":
				case "HIT":
				case "CURSOR":
				case "ORIGIN HIT":
					found = true;
					break;
			}
			if (!found)
			{
				foreach (Target t in targets)
				{
					if (t.TargetID == e.TargetID)
					{
						found = true;
						break;
					}
				}
				foreach (Effect e2 in effects)
                {
					if (e2 is Projectile p)
                    {
						if (p.TargetOverride == e.TargetID)
                        {
							found = true;
							break;
                        }
							
                    }
                }
			}

			if (!found)
			{
				isSimple = false;
				return "Not all targets found.";
			}
		}
		return "Simple!";
	}



	#endregion

	#region Callbacks
	public delegate void TimingCallback(CallInfo callInfo);
	public delegate void BasicCallback(CallInfo callInfo);

	[HideInInspector]
	public TimingCallback OnUpdate;
	[HideInInspector]
	public TimingCallback OnPress;
	[HideInInspector]
	public TimingCallback OnRelease;
	[HideInInspector]
	public TimingCallback OnHold;
	[HideInInspector]
	public TimingCallback OnWindUp;
	[HideInInspector]
	public TimingCallback OnStartWindUp;
	[HideInInspector]
	public TimingCallback OnCast;
	[HideInInspector]
	public TimingCallback OnWindDown;
	[HideInInspector]
	public TimingCallback OnEndWindDown;
	[HideInInspector]
	public TimingCallback OnPreparingCast;
	[HideInInspector]
	public TimingCallback OnNotPreparingCast;
	[HideInInspector]
	public TimingCallback OnReadyToCast;
	[HideInInspector]
	public TimingCallback OnInterrupt;

	[HideInInspector]
	public BasicCallback OnHit;
	[HideInInspector]
	public BasicCallback OnDestroy;
	[HideInInspector]
	public BasicCallback OnCallback;

	[HideInInspector]
	public BasicCallback OnHitDamage;

	/*
	public void TriggerOnHit(int InstanceID, Character character, Ability ability, Attribute itemAttribute, Character origin, Character target, HitInfo hitInfo)
	{
		if (!character.IsAuthoritative() && !RunLocallyOnClient) //that is don't do this if you don't have authority
			return;
		foreach (Target t in targets)
		{
			if (t is Target_OnHit toh)
			{
				toh.OnHit(InstanceID, character, this, ability, itemAttribute, origin, target, hitInfo);
			}
		}
	}*/
	#endregion

	[HideInInspector]
	public Ability _ability;

	#region Initialize

	List<Vector3> tempTargetPoints = new List<Vector3>();
	public void Initialize(Ability ability)
	{
		_ability = ability;
		if (!activations.ContainsKey(ID))
		{
			OnUpdate = null;
			OnPress = null;
			OnRelease = null;
			OnHold = null;
			OnWindUp = null;
			OnStartWindUp = null;
			OnCast = null;
			OnWindDown = null;
			OnEndWindDown = null;
			OnPreparingCast = null;
			OnNotPreparingCast = null;
			OnReadyToCast = null;
			OnInterrupt = null;

			OnHit = null;
			OnDestroy = null;
			OnCallback = null;
			OnHitDamage = null;
			

			activations.Add(ID, this);
			InitializeStaticCooldown();
			InitializeStamina();
			InitializeCursor();
			InitializeAnimation();
			InitializeSound();
			foreach (Limitation limitation in limitations)
			{
				if (limitation != null) //failsafe for deleting limitations
					limitation.Initialize(this);
			}
			foreach (Target target in targets)
			{
				if (target != null) //failsafe
					target.Initialize(this);
			}
			foreach (Effect effect in effects)
			{
				if (effect != null) //failsafe
					effect.Initialize(this);
			}
		}
		if (tempTargetPoints == null)
		{
			tempTargetPoints = new List<Vector3>();
		}

		InitializeSimple();
	}
	#endregion

	#region Update and supporting functions
	public void UpdateThis(CallInfo callInfo, bool isAuthoritative)
	{
		if (callInfo.caster== null || callInfo.caster.characterAbilities == null)
			return;

		if (RunLocallyOnClient)
		{
			isAuthoritative = true; //everyone is authoritative in these
		}
		if (DontSyncAnything && !isAuthoritative)
			return; //hey you're not supposed to be here

		if (!isAuthoritative && !isSimple)
			return; //for now only simple abilities can be run non-authroitatively
		int instanceID = callInfo.caster.characterAbilities.GetCurrentInstanceID(this, callInfo.item);


        OnUpdate?.Invoke(callInfo);



		//Keeps casting while it can
		/*
		if (character.characterAbilities.IsReadyAbility(ability, itemAttribute) && RepeatCast)
		{
			if (!character.characterAbilities.CheckActivationState(this, itemAttribute, true) 
				&& !character.characterAbilities.CheckActivationState(this, itemAttribute, false))
			{
				if (CanCast(character, ability, itemAttribute, false))
				{
					StartWindUp(character, ability, true, itemAttribute);
				}
				else
				{
					OnReadyToCast?.Invoke(character, this, ability, itemAttribute);
				}
			}

		}*/
		#region Button Management
		if (isAuthoritative && callInfo.control != null)
		{


			if (callInfo.control.Press() || CastAutomatically)
			{

				if (CanPrepare(callInfo, !Ability.IsPassive(callInfo.control)) && SetPreparingAbility)
				{
                    callInfo.caster.characterAbilities.SetPreparingAbility(callInfo);
				}
				else if (callInfo.caster.IsPlayerCharacter && SetPreparingAbility)
				{
					if (!CastAutomatically && !Ability.IsPassive(callInfo.control) && !CastAnyTime)

					{
						if (callInfo.ability.CanBeDisarmed && callInfo.caster.HasStatus(Status.Type.Disarm))
							TextEffect.disarmed.Show();
						else if (callInfo.ability.CanBeSilenced && callInfo.caster.HasStatus(Status.Type.Silence))
							TextEffect.silenced.Show();
					}
					if (!HoldButtonToCast && !callInfo.ability.CanPrepareAnotherActivation(callInfo.caster, callInfo.item) &&
						!CastAutomatically && !Ability.IsPassive(callInfo.control) && CheckCooldown(callInfo, false, false) > 0f)
					{
						TextEffect.cooldown.Show();
						SoundManager.PlayCooldown(callInfo.caster);
					} else if (!callInfo.caster.characterAbilities.IsCastingAbility() && !callInfo.ability.DontMakeInvalidNoise)
                    {
						SoundManager.PlayInvalid(callInfo.caster);
					}
				}

			}

			if (callInfo.control.Press())
			{
				OnPress?.Invoke(callInfo);
			}

			if (SetPreparingAbility)
			{
				//Preparing ability is which ability are you "holding down"
				if (callInfo.caster.characterAbilities.CheckPreparingAbility(callInfo) && CanPrepare(callInfo, false))
				{
					OnPreparingCast?.Invoke(callInfo);
					//Cursor UI

					ShowCursorUI(callInfo);
				}
				else
					OnNotPreparingCast?.Invoke(callInfo);
			}



			if (callInfo.control.Hold())
			{
				OnHold?.Invoke(callInfo);
			}
			if (callInfo.control.Hold() || CastAutomatically)
			{
				//holding the button doesn't do anything by itself, unless holdbuttontocast is true

				if (HoldButtonToCast && CanPrepare(callInfo) && SetPreparingAbility)
				{
                    callInfo.caster.characterAbilities.SetPreparingAbility(callInfo);
				}
				else if (HoldButtonToCast && SetPreparingAbility && callInfo.caster.characterAbilities.CheckCastingAbility(callInfo.ability, callInfo.itemID))
				{
					OnPreparingCast?.Invoke(callInfo);
				}

				if (HoldButtonToCast
					&& (CastAnyTime || (!callInfo.caster.characterAbilities.CheckActivationState(this, callInfo.itemID, true)
					&& !callInfo.caster.characterAbilities.CheckActivationState(this, callInfo.itemID, false))))
				{
					TriggerStartWindUp(callInfo);

				}
			}

			if (callInfo.control.Release())
			{
				OnRelease?.Invoke(callInfo);
			}

			if ((callInfo.control.Release() && !CastOnPress) || CastAutomatically || (callInfo.control.Press() && CastOnPress))
			{
				bool didCast = false;

				//releasing the button will begin wind up if you don't need to hold
				if (!HoldButtonToCast)
				{
					/*
					if (!callInfo.control.IsPassive() && callInfo.caster.IsPlayerCharacter && !CastAutomatically)
						Debug.Log("Cast attempted"); */
					didCast = TriggerStartWindUp(callInfo);

				}
				//buffering:
				if (!Ability.IsPassive(callInfo.control) && callInfo.caster.IsPlayerCharacter && !HoldButtonToCast && !didCast && !DontBuffer
					&& !(SetCastingAbility && DontBufferOnSelf && callInfo.caster.characterAbilities.CheckCastingAbility(callInfo.ability, callInfo.itemID))
					&& !CastAnyTime &&
					(!SetPreparingAbility || callInfo.caster.characterAbilities.IsPreparingAbility()
					|| callInfo.caster.characterAbilities.IsCastingAbility() || !callInfo.caster.CanCast())) //don't buffer casting for disarm/silence
				{
                    callInfo.caster.characterAbilities.BufferActivation(callInfo);
				}

			}
		}
		#endregion

		//timer for checking wind up
		//since windup should be unset if the cast is interrupted, we can just check it here
		if (callInfo.caster.characterAbilities.CheckActivation(this, callInfo.itemID, WindingUp: true))
		{
			Cast(callInfo);

		}
		//checking the winddown, THIS TRIGGERS ENDWINDDOWN
		if (callInfo.caster.characterAbilities.CheckActivation(this, callInfo.itemID, WindingUp: false))
		{
			EndWindDown(callInfo);
		}


		//Invoke timings for if this activation is winding up or down
		if (callInfo.caster.characterAbilities.CheckActivationState(this, callInfo.itemID, true))
		{
			OnWindUp?.Invoke(callInfo);
		}
		if (callInfo.caster.characterAbilities.CheckActivationState(this, callInfo.itemID, false))
		{
			OnWindDown?.Invoke(callInfo);
		}

		if (CantBeInterrupted)
			return;
		//---------------------------------------------------------------------Nothing below this
		//INTERRUPTS:

		//if you can't cast the wind up stops
		if (isAuthoritative)
		{
			if (!CanCast(callInfo, true) &&
			callInfo.caster.characterAbilities.CheckActivationState(this, callInfo.itemID, true))
			{
				Interrupt(callInfo, true, true, !DontSyncAnything);
			}
			if (!CanCast(callInfo, true, true) &&
					callInfo.caster.characterAbilities.CheckActivationState(this, callInfo.itemID, false))
			{
				Interrupt(callInfo, false, true, !DontSyncAnything);
			}
		}
	}

	public void Interrupt(CallInfo callInfo, bool windingUp, bool showAnimation, bool sendCommand)
	{
		if (CantBeInterrupted || callInfo.caster == null || callInfo.caster.characterAbilities == null)
			return;
		if (!callInfo.caster.Interrupt(callInfo, showAnimation, sendCommand, windingUp))
			return;
		callInfo.caster.characterAbilities.UnsetActivationTimer(this, callInfo.itemID, windingUp);
		
		OnInterrupt?.Invoke(callInfo);
		//todo: send an interrupt message
	}

	public void ShowCursorUI(CallInfo callInfo)
	{
		if (!callInfo.caster.IsPlayerCharacter)
			return;
		Vector3? point = callInfo.caster.characterAbilities.GetCurrentCursorPoint(callInfo.ability, this, callInfo.itemID);

		float arrowWidth = ArrowWidth == null ? 1f : ArrowWidth.GetValue(callInfo);
		if (point.HasValue)
			Target.ShowStaticUI(callInfo.caster.transform.position, point.Value,
				IndicatePointOnUI, IndicatePointWidth, IndicateRangeOnUI,
				MinCursorDistance, MaxCursorDistance, PointToCursorOnUI, arrowWidth, .5f);

	}


	public void Cast(CallInfo callInfo)
	{
		float windDown = WindDown.GetValue(callInfo);

		//Cast Freeze Time
		if (!callInfo.ability.CantMoveWhileCasting && callInfo.ability.CastFreezeTime != null && !callInfo.ability.CastFreezeTime.NaiveIsZero()) 
		{
			float freezeTime = callInfo.ability.CastFreezeTime.GetValue(callInfo);
			if (!callInfo.ability.CastFreezeTimeDontScaleWithAbilityBonus)
			{
                callInfo.caster.CheckAbilityBonus(AbilityBonus.CastTime, callInfo.ability, freezeTime);
			}

            callInfo.caster.characterAbilities.CastFreezeTime(freezeTime);
		}

		if (!WindingDontScaleWithAbilityBonus)
		{
			windDown = callInfo.caster.CheckAbilityBonus(AbilityBonus.CastTime, callInfo.ability,windDown);
		}
		if (windDown == 0f)
		{
			EndWindDown(callInfo);
		}
		else
		{
			callInfo.caster.characterAbilities.SetActivationTimer(this, callInfo.itemID, windDown, false);
		}
		OnCast?.Invoke(callInfo);
	}

	public bool CanCast(CallInfo callInfo, bool onlyInterrupt, bool windDown = false)
	{
		if (!callInfo.caster.characterAbilities.CheckCastingAbility(callInfo.ability, callInfo.itemID)
			&& callInfo.caster.characterAbilities.IsCastingAbility() && !CastAnyTime) //must be casting this ability or no ability
			return false;

		if (!callInfo.caster.CanCast() && !CastAnyTime)
			return false;

		if ((callInfo.caster.HasStatus(Status.Type.Silence) && callInfo.ability.CanBeSilenced) && !CastAnyTime)
			return false;
		if ((callInfo.caster.HasStatus(Status.Type.Disarm) && callInfo.ability.CanBeDisarmed) && !CastAnyTime)
			return false;

		if (!CanCastIfDead && callInfo.caster.Dead)
			return false;
		if (CanCastIfDead && CanOnlyCastIfDead && !callInfo.caster.Dead)
			return false;
		foreach (Limitation limitation in limitations)
		{
			if (onlyInterrupt && !limitation.canInterrupt)
				continue;
			if (onlyInterrupt && windDown && !limitation.canInterruptWinddown)
				continue;
			bool thisLim = limitation.CanCast(callInfo);

			if ((thisLim && limitation.invert) ||
				(!thisLim && !limitation.invert))
				return false;
		}
		if (GetStaticCooldown(callInfo, true) > 0f && !windDown)
			return false;
		if (!CheckStamina(callInfo) && !windDown)
			return false;
		return true;
	}

	public bool CanPrepare(CallInfo callInfo, bool ShowText = false)
	{
		if (!callInfo.caster.CanCast()
			|| (callInfo.caster.HasStatus(Status.Type.Silence) && callInfo.ability.CanBeSilenced) || (callInfo.caster.HasStatus(Status.Type.Disarm) && callInfo.ability.CanBeDisarmed))
			return false;

		if (ShouldUseStamina(callInfo))
		{
			if (!CheckStamina(callInfo))
			{
				if (ShowText)
				{
					LimitationShowText.Show("Out of Stamina!", Color.green);
					SoundManager.PlayInvalid(callInfo.caster);
				}

				return false;
			}

		}


		foreach (Limitation limitation in limitations)
		{
			bool canCastLim = limitation.CanCast(callInfo);
			if (limitation.CantPrepare
				&& ((!canCastLim && !limitation.invert) || (canCastLim && limitation.invert)))
			{
				if (ShowText && limitation.CustomCantPrepareMessage != "" && limitation.CustomCantPrepareMessage != null)
				{
					LimitationShowText.Show(limitation.CustomCantPrepareMessage, limitation.CustomCantPrepareColor);
					SoundManager.PlayOutOfTheurgy(callInfo.caster);
				}

				return false;
			}

		}
		if (!CastAnyTime && callInfo.caster.characterAbilities.IsCastingAbility())
			return false;

		if (StaticCooldown.GetValue(callInfo) != 0 &&
			CantPrepareStaticCooldown && GetStaticCooldown(callInfo, true) > 0f)
			return false;

		return true;
	}
	public float CheckCooldown(CallInfo callInfo, bool preparing, bool relative)
	{
		float cooldown = -1f;
		foreach (Limitation limitation in limitations)
		{
			if (limitation.GetType() == typeof(Cooldown))
			{
				Cooldown cd = (Cooldown)limitation;
				if (limitation.CantPrepare || !preparing)
				{
					if (cooldown == -1f)
						cooldown = cd.CheckCooldown(callInfo, relative);
					else
					{
						cooldown = Mathf.Max(cooldown, cd.CheckCooldown(callInfo, relative));
					}
				}

			}
		}
		if (!preparing || CantPrepareStaticCooldown)
			cooldown = Mathf.Max(cooldown, GetStaticCooldown(callInfo, relative));
		return cooldown;
	}

	public bool CanCastImediate(CallInfo callInfo)
	{

		if (!CastAnyTime && SetPreparingAbility && !callInfo.caster.characterAbilities.CheckPreparingAbility(callInfo))
			return false;
		if (!CastAnyTime && callInfo.caster.characterAbilities.CheckActivationState(this, callInfo.itemID, true))
			return false;

		return true;
	}

	public bool TriggerStartWindUp(CallInfo callInfo, Vector3? cursorOverride = null)
	{
		if (!CanCast(callInfo, false))
			return false;
		if (!CanCastImediate(callInfo))
			return false;

		if (!(CastAnyTime ||
					(!callInfo.caster.characterAbilities.CheckActivationState(this, callInfo.itemID, true)
					&& !callInfo.caster.characterAbilities.CheckActivationState(this, callInfo.itemID, false))))
			return false; //you are already casting this!!

		if (cursorOverride != null)
		{
			callInfo.caster.characterAbilities.OverrideCursorPoint(callInfo, cursorOverride.Value);
		}
		StartWindup(callInfo);
		if (isSimple && !DontSyncAnything && !RunLocallyOnClient)
		{
			Vector3? cursor = callInfo.caster.characterAbilities.GetCurrentCursorPoint(callInfo.ability, this, callInfo.itemID);
			Vector3 actualCursor = cursor.HasValue ? cursor.Value : Vector3.positiveInfinity;
			if (SendReliably)
                callInfo.caster.characterAbilities.CmdReliableSimpleStartWindup(callInfo,
                    callInfo.caster.characterAbilities.GetCharacterTargetDictionary(callInfo.ability, callInfo.itemID),
                    callInfo.caster.characterAbilities.GetPointTargetDictionary(callInfo.ability, callInfo.itemID),
					actualCursor
					);
			else
                callInfo.caster.characterAbilities.CmdUnreliableSimpleStartWindup(callInfo,
                    callInfo.caster.characterAbilities.GetCharacterTargetDictionary(callInfo.ability, callInfo.itemID),
                    callInfo.caster.characterAbilities.GetPointTargetDictionary(callInfo.ability, callInfo.itemID),
					actualCursor);
		}

		return true;
	}

	public void StartWindup(CallInfo callInfo)
	{
        callInfo.caster.characterAbilities.IterateInstanceID(this, callInfo.item);

		if (SetCastingAbility)
            callInfo.caster.characterAbilities.SetCastingAbility(callInfo);

		if (callInfo.caster.playerTrackAbilityUsage != null && SetCastingAbility)
		{
            callInfo.caster.playerTrackAbilityUsage.LocalSet(callInfo.ability.type);
		}

		OnStartWindUp?.Invoke(callInfo);

		float windUp = WindUp.GetValue(callInfo);
		if (!WindingDontScaleWithAbilityBonus)
        {
			windUp = callInfo.caster.CheckAbilityBonus(AbilityBonus.CastTime, callInfo.ability, windUp);
        }

		if (windUp != 0)
            callInfo.caster.characterAbilities.SetActivationTimer(this, callInfo.itemID, windUp, true);
		else
		{
			if (!(callInfo.caster.CanCast() &&

				(callInfo.caster.HasStatus(Status.Type.Silence) && callInfo.ability.CanBeSilenced) || 
					(callInfo.caster.HasStatus(Status.Type.Disarm) && callInfo.ability.CanBeDisarmed))
				||
				CastAnyTime)
				Cast(callInfo);
		}

		if (SetPreparingAbility && callInfo.caster.characterAbilities.CheckPreparingAbility(callInfo))
		{
            callInfo.caster.characterAbilities.UnsetPreparingAbility();
		}
	}
	void EndWindDown(CallInfo callInfo)
	{
		OnEndWindDown?.Invoke(callInfo);
	}
	#endregion

	#region Static Cooldown

	void InitializeStaticCooldown()
	{
		if (StaticCooldown.NaiveIsZero() && !DontUseUniqueID) //this staticColdown is useless
			return; 
		OnUpdate += UpdateStaticCooldown;

		Property.AddActivationCallbacks(CooldownTiming, this, ActivateStaticCooldown);

	}

	void ActivateStaticCooldown(CallInfo callInfo)
	{
        float cd = StaticCooldown.GetValue(callInfo);
        if (!CooldownDontScaleWithAbilityBonus)
        {
            cd = callInfo.caster.CheckAbilityBonus(AbilityBonus.Cooldown, callInfo.ability, cd);
        }
        string ID = DontUseUniqueID ? CooldownID : (CooldownGlobal ? CooldownID : GetUniqueStorageID(callInfo.item, CooldownID));
        callInfo.caster.StoreFloat(callInfo.ability, callInfo.itemID, ID, cd, CooldownGlobal, CooldownSave);
	}

	void UpdateStaticCooldown(CallInfo callInfo)
	{
		string ID = DontUseUniqueID ? CooldownID : (CooldownGlobal ? CooldownID : GetUniqueStorageID(callInfo.item, CooldownID));

        float cd = StaticCooldown.GetValue(callInfo);
        if (!CooldownDontScaleWithAbilityBonus)
        {
			cd = callInfo.caster.CheckAbilityBonus(AbilityBonus.Cooldown, callInfo.ability, cd);
        }
        if (cd <= 0f)
			return;
		float currentCooldown = GetStaticCooldown(callInfo, false) - Time.deltaTime;
		callInfo.caster.StoreFloat(callInfo.ability, callInfo.itemID, ID, currentCooldown, CooldownGlobal, CooldownSave);

	}

	float GetStaticCooldown(CallInfo callInfo, bool relative)
	{
		string ID = DontUseUniqueID ? CooldownID : (CooldownGlobal ? CooldownID : GetUniqueStorageID(callInfo.item, CooldownID));

		float cd = StaticCooldown.GetValue(callInfo);
		if (!CooldownDontScaleWithAbilityBonus)
		{
			cd = callInfo.caster.CheckAbilityBonus(AbilityBonus.Cooldown, callInfo.ability, cd);
        }

        if (cd <= 0f)
			return 0f;
		float baseCD = cd;
		float defaultCD = CooldownStartOnCooldown ? (
			CooldownStartWithRandomCooldown ?
			baseCD * Random.value
			:
			baseCD
			) : 0f;

		return relative ? callInfo.caster.CheckStoredFloat(callInfo.ability, callInfo.itemID, ID, defaultCD, CooldownGlobal, CooldownSave) 
			/ cd
			: callInfo.caster.CheckStoredFloat(callInfo.ability, callInfo.itemID, ID, defaultCD, CooldownGlobal, CooldownSave);
	}
	#endregion

	#region Stamina

	public void InitializeStamina()
	{
		if (ShouldUseStamina())
		{
			Property.AddActivationCallbacks(StaminaActivation, this, ActivateStamina);
			Property.AddHitCallbacks(StaminaActivation, this, HitActivateStamina);
		}

	}
	public bool ShouldUseStamina(CallInfo? callInfo = null)
	{
		if (StaminaCost != null && (StaminaCost.NaiveIsZero() || StaminaActivation == Timing.None))
			return false;
		if (callInfo.HasValue)
		{
			if (!callInfo.Value.caster.IsAuthoritative())
				return false;
			if (callInfo.Value.caster.characterStamina == null)
				return false;
		}
		if (!DontCostStaminaWhilePassive)
			return true;
		if (!callInfo.HasValue || callInfo.Value.caster == null)
			return !(DontCostStaminaWhilePassive && CastAutomatically);
		return !(DontCostStaminaWhilePassive && (CastAutomatically || (callInfo.HasValue && callInfo.Value.caster.characterAbilities.IsPassive(callInfo.Value))));
	}

	public bool CheckStamina(CallInfo callInfo)
	{
		if (!ShouldUseStamina(callInfo))
			return true;

		if (StaminaCost != null && !StaminaCost.NaiveIsZero())
		{
			return callInfo.caster.characterStamina.CheckStamina();
		}
		return true;
	}

	void HitActivateStamina(CallInfo callInfo)
	{
		ActivateStamina(callInfo);
	}
	void ActivateStamina(CallInfo callInfo)
	{
		if (!ShouldUseStamina(callInfo))
			return;

		float defaultAmount = StaminaCost.GetValue(callInfo);

        float amount = defaultAmount;

        if (!StaminaDontUseAbilityBonus) //check after increasing
        {
            amount = callInfo.caster.CheckAbilityBonus(AbilityBonus.StaminaCost, callInfo.ability, multiplier: amount);
        }
        if (amount > 0)
			callInfo.caster.characterStamina.UseStamina(amount, DontResetStaminaTimer);

	}

	#endregion

	#region Static Reference
	public static Activation GetActivation(string ID)
	{
		if (ID == null || !activations.ContainsKey(ID))
			return null;
		return activations[ID];
	}

	public static string GetUniqueStorageID(Item item, string FloatID)
	{
		if (item == null)
			return FloatID;
		return item.uniqueIdentifier + FloatID;
	}
	#endregion

	#region GETID
	public Effect GetEffect(byte ID)
	{
		if (ID >= effects.Count)
			return null;
		return effects[ID];
	}

	public Target GetTarget(byte ID)
	{
		if (ID >= targets.Count)
			return null;
		return targets[ID];
	}

	public Limitation GetLimitation(byte ID)
	{
		if (ID >= limitations.Count)
			return null;
		return limitations[ID];
	}

    #endregion

    public bool OverrideThisIsNotSimple = false;
    #region Editor

#if UNITY_EDITOR

    static List<Property> properties;
	static List<string> targetIDs;
	[Button]
	public void UpdateTargets()
	{
		if (_ability != null)
			_ability.UpdateTargets();
		else
		{
			if (properties == null)
				properties = new List<Property>();
			if (targetIDs == null)
				targetIDs = new List<string>();

			Ability.InitializeTargetIDs(targetIDs);

			properties.Clear();
			properties.AddRange(effects);
			properties.AddRange(limitations);
			properties.AddRange(targets);
			foreach (Property p in properties)
			{
				Ability.AddTargetID(targetIDs, p);
			}

			foreach (Property p in properties)
			{
				p.UpdateTargets(targetIDs);
			}
		}
	}

	[Button]
	public void UpdateName()
	{
		name = ID;
	}


	/*No longer needed
	public void RecalculateSubobjects()
	{
		effects.Clear();
		targets.Clear();
		limitations.Clear();
		string grossPath = UnityEditor.AssetDatabase.GetAssetPath(this);
		string path = grossPath.Substring(0, grossPath.LastIndexOf("/"));
		string[] subFolders = UnityEditor.AssetDatabase.GetSubFolders(path);
		string[] guids = UnityEditor.AssetDatabase.FindAssets("t:Effect", subFolders);
		foreach (string guid in guids)
		{
			string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
			Effect effect = UnityEditor.AssetDatabase.LoadAssetAtPath<Effect>(assetPath);
			effects.Add(effect);
			effect.RecalculateSubobjects();
		}

		guids = UnityEditor.AssetDatabase.FindAssets("t:Target", subFolders);
		foreach (string guid in guids)
		{
			string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
			Target target = UnityEditor.AssetDatabase.LoadAssetAtPath<Target>(assetPath);
			targets.Add(target);
			target.RecalculateSubobjects();
		}

		guids = UnityEditor.AssetDatabase.FindAssets("t:Limitation", subFolders);
		foreach (string guid in guids)
		{
			string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
			Limitation limitation = UnityEditor.AssetDatabase.LoadAssetAtPath<Limitation>(assetPath);
			limitations.Add(limitation);
			limitation.RecalculateSubobjects();
		}
	}
	*/

	public void UpdateIDs()
	{
		//Updates IDs so that they match this Ability's IDs
		int i = 0;
		foreach (Effect effect in effects)
		{
			effect.EffectID = ID + effect.GetType() + i;
			effect.name = effect.EffectID;
			i++;
		}
		i = 0;
		foreach (Target target in targets)
		{
			target.name = ID + target.GetType() + i;
			i++;
		}
		i = 0;
		foreach (Limitation limitation in limitations)
		{
			limitation.name = ID + limitation.GetType() + i;
			i++;
		}
	}



	
	[Tooltip("Do not use.")]
	public string SimpleOutput;



	[Button]
	private void CheckIfIsSimple()
    {
		SimpleOutput = InitializeSimple();
		
    }

#endif

	#endregion

	#region Extra Utilities

	public void ForceUpdateTargets(CallInfo callInfo)
	{
		foreach (Target target in targets)
		{
			if ((target.timing & Timing.Update) == Timing.Update || (target.timing & Timing.TargetWhenTriggered) == Timing.TargetWhenTriggered)
			{
				target.Activate(callInfo);
			}
		}
	}

	#endregion

	#region Cursor

	public void InitializeCursor()
	{
		if (CastAutomatically)
		{
			return;
		}

		if (PointTowardsWhilePassive || !CastAutomatically)
			Property.AddActivationCallbacks(PointTowardsCursor, this, ActivatePointAt);
	}

	List<Vector3> pointTemp = new List<Vector3>();
	void ActivatePointAt(CallInfo callInfo)
	{
        callInfo.caster.characterAbilities.GetTargets(callInfo, pointTemp, "CURSOR");
		if (pointTemp.Count == 0)
			return;
		Vector3 point = pointTemp[0];
		if (point != callInfo.caster.transform.position)
            callInfo.caster.PointTowards(point);

	}

	#endregion

	#region Animation
	public void InitializeAnimation()
	{
		if (animationClip == null)
		{
			return;
		}

		Property.AddActivationCallbacks(animationTiming, this, ActivateAnimation);
	}

	void ActivateAnimation(CallInfo callInfo)
	{
		if (callInfo.caster.animationController != null)
        {
			float aSpeed = animationSpeed.GetValue(callInfo);
			if (!AnimationDontScaleWithAbilityBonus)
            {
				aSpeed = callInfo.caster.CheckAbilityBonus(AbilityBonus.CastTime, callInfo.ability, aSpeed, negate: true);
            }

            callInfo.caster.animationController.PlayAnimation(animationClip, ID, PlayMode.StopAll, aSpeed, true);
		}
			
	}

	#endregion

	#region Sound
	public void InitializeSound()
	{
		if (soundClip == null)
		{
			return;
		}

		Property.AddActivationCallbacks(soundTiming, this, ActivateSound);
	}

	void ActivateSound(CallInfo callInfo)
	{
		SoundManager.PlaySound(soundClip, callInfo.caster.transform.position, soundVolume.GetValue(callInfo), "", false);
	}
	#endregion
}
