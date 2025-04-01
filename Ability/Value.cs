using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Sirenix.OdinInspector;

[GUIColor(0.3f, 0.8f, 0.8f, 1f)]
[HideReferenceObjectPicker]
[System.Serializable]
public class Value
{
	public enum Type
	{
		Number,
		Single,
		Multiply,
		Add,
		Ability,
		ThreeMultiply,
		SkillUpgrade,
		Divide
	}

	[LabelWidth(100)]
	[BoxGroup("BoxMain", false)]
	public Type type = Type.Number;

	[LabelWidth(100)]
	[BoxGroup("BoxMain", false)]
	[HideIf("@type == Type.Ability || type == Type.SkillUpgrade")]
	[Tooltip("Base number is always added to the final sum. ")]
	public float baseNumber = 0f;

	[LabelWidth(100)]
	[BoxGroup("BoxMain", false)]
	[HideIf("@type == Type.Ability || type == Type.SkillUpgrade")]
	[Tooltip("Base number is always multiplied to the final sum. (not including the baseNumber) ")]
	public float baseMult = 1f;

	[GUIColor(0.8f, 0.6f, 0.9f, 1f)]
	[HideReferenceObjectPicker]
	[BoxGroup("BoxMain/MainSub", false)]
	[HideIf("@type == Type.Ability || type == Type.SkillUpgrade || type == Type.Number")]
	public SubValue mainSubValue = new SubValue();


	[GUIColor(0.8f, 0.8f, 0.3f, 1f)]
	[HideReferenceObjectPicker]
	[BoxGroup("BoxMain/Second", false)]
	[HideIf("@type == Type.Single || type == Type.SkillUpgrade || type == Type.Ability || type == Type.Number")]
	public SubValue secondValue = new SubValue();


	[GUIColor(0.3f, 0.8f, 0.8f, 1f)]
	[HideReferenceObjectPicker]
	[BoxGroup("BoxMain/Third", false)]
	[ShowIf("@type == Type.ThreeMultiply")]
	public SubValue thirdValue = new SubValue();

	[LabelWidth(100)]
	[BoxGroup("BoxMain", false)]
	[ShowIf(
		nameof(type), Type.Ability
		)]
	[Tooltip("Make sure this value is actually on an ability")]
	public int WhichAbilityValue;

	[LabelWidth(100)]
	[BoxGroup("BoxMain", false)]
	[ShowIf(
		nameof(type), Type.SkillUpgrade
		)]
	public UpgradeID upgradeID = UpgradeID.DAMAGE_A;

	public Value Clone()
	{
		NetworkWriter writer = new NetworkWriter();

		writer.Write(this);
		NetworkReader reader = new NetworkReader(writer.ToArray());
		return reader.Read<Value>();
	}


	public float GetValue(CallInfo callInfo, float extraMultiplier = 1f, 
		Skill skillOverride = null)
	{
		switch (type)
		{
			case Type.Number:
				return (baseNumber + baseMult) *extraMultiplier;
			case Type.Single:
				return (mainSubValue.GetValue(callInfo, skillOverride) * baseMult + baseNumber) * extraMultiplier;
			case Type.Multiply:
				float product = mainSubValue.GetValue(callInfo, skillOverride) * secondValue.GetValue(callInfo, skillOverride);
				return (product * baseMult + baseNumber) *extraMultiplier;
			case Type.Add:
				float sum = mainSubValue.GetValue(callInfo, skillOverride) + secondValue.GetValue(callInfo, skillOverride);
				return (sum * baseMult + baseNumber) *extraMultiplier;
			case Type.Ability:
				return (callInfo.ability.Values[WhichAbilityValue].GetValue(callInfo)) * extraMultiplier;
			case Type.ThreeMultiply:
				float threeProduct = mainSubValue.GetValue(callInfo, skillOverride) * secondValue.GetValue(callInfo, skillOverride)
					*thirdValue.GetValue(callInfo, skillOverride);
				return (threeProduct * baseMult + baseNumber) *extraMultiplier;
			case Type.Divide:
				float quotient = (mainSubValue.GetValue(callInfo, skillOverride) / secondValue.GetValue(callInfo, skillOverride));
				return (quotient * baseMult + baseNumber) * extraMultiplier;
			case Type.SkillUpgrade:
				if (callInfo.caster == null || callInfo.caster.playerSkills == null || (skillOverride == null && (callInfo.ability == null || callInfo.ability.mySkill == null)))
					return 0f;
				if (skillOverride != null)
				{
					return skillOverride.GetUpgradeValue(upgradeID, callInfo);
				}
				return callInfo.ability.mySkill.GetUpgradeValue(upgradeID, callInfo);
		}

		throw new System.Exception("Something went wrong with Value Calculation; used PLACEHOLDER");

	}

	public string BeautyString(CallInfo callInfo, Skill skillOverride = null, float extraMultiplier = 1f)
	{
		string intyBaseNumber = Value.IntifyFloat(baseMult, false);
		string intyBaseMult = Value.IntifyFloat(baseMult);

		string finalString = "(";
		switch (type)
		{
			case Type.Number:
				return ValueString(callInfo, skillOverride, extraMultiplier);
			case Type.Single:
				if (mainSubValue.NeedsTarget())
					return ValueString(callInfo, skillOverride, extraMultiplier);
				else
					return IntifyFloat(GetValue(callInfo));
			case Type.Multiply:
				if (!mainSubValue.NeedsTarget() && !secondValue.NeedsTarget())
				{
					return IntifyFloat(GetValue(callInfo)).ToString(); 
				}
				bool extraMultUsed = false;
				if (mainSubValue.NeedsTarget())
                {
					finalString += mainSubValue.SubValueString(callInfo, skillOverride);
				}
                else
                {
					finalString += IntifyFloat(extraMultiplier * mainSubValue.GetValue(callInfo, skillOverride));
					extraMultUsed = true;
				}
					
				finalString += ") x (";
				if (secondValue.NeedsTarget())
				{
					finalString += secondValue.SubValueString(callInfo, skillOverride);
				}
                else
                {
					finalString += IntifyFloat(extraMultiplier * secondValue.GetValue(callInfo, skillOverride));
					extraMultUsed = true;
				}
					
				finalString = baseMult == 1 ? finalString : intyBaseMult + finalString;
				finalString = baseNumber == 0 ? finalString : finalString + intyBaseNumber;
				finalString += ")";
				
				if (extraMultUsed)
					return finalString;
				break;
			
			case Type.ThreeMultiply:
				if (!mainSubValue.NeedsTarget() && !secondValue.NeedsTarget() && !thirdValue.NeedsTarget())
				{
					return IntifyFloat(GetValue(callInfo));
				}
				if (!mainSubValue.NeedsTarget() && !secondValue.NeedsTarget() && baseNumber == 0 && baseMult == 1)
                {
					return
						"(" +
						IntifyFloat(mainSubValue.GetValue(callInfo, skillOverride) * secondValue.GetValue(callInfo, skillOverride))
						+ ") x (" + thirdValue.SubValueString(callInfo, skillOverride) + ")" + (extraMultiplier == 1f ? "" : " * " + extraMultiplier);
                }
                if (mainSubValue.NeedsTarget())
					finalString += mainSubValue.SubValueString(callInfo, skillOverride);
				else
					finalString += mainSubValue.GetValue(callInfo, skillOverride);
				finalString += ") x (";
				if (secondValue.NeedsTarget())
					finalString += secondValue.SubValueString(callInfo, skillOverride);
				else
					finalString += secondValue.GetValue(callInfo, skillOverride);

				finalString += ") x (";
				if (thirdValue.NeedsTarget())
					finalString += thirdValue.SubValueString(callInfo, skillOverride);
				else
					finalString += thirdValue.GetValue(callInfo, skillOverride);
				finalString = baseMult == 1 ? finalString : intyBaseMult + finalString;
				finalString = baseNumber == 0 ? finalString : finalString + intyBaseNumber;
				finalString += finalString + ")";
				break;

			case Type.Add:
				if (!mainSubValue.NeedsTarget() && !secondValue.NeedsTarget())
				{
					return IntifyFloat(GetValue(callInfo)).ToString();
				}
				if (mainSubValue.NeedsTarget())
					finalString += mainSubValue.SubValueString(callInfo, skillOverride);
				else
					finalString += mainSubValue.GetValue(callInfo, skillOverride);
				finalString += ") + (";
				if (secondValue.NeedsTarget())
					finalString += secondValue.SubValueString(callInfo, skillOverride);
				else
					finalString += secondValue.GetValue(callInfo, skillOverride);
				finalString = baseMult == 1 ? finalString : intyBaseMult.ToString() + finalString;
				finalString = baseNumber == 0 ? finalString : finalString + intyBaseNumber.ToString();
				finalString += ")";
				break;

			case Type.Ability:
				return "An ability based value";

			case Type.Divide:
				if (!mainSubValue.NeedsTarget() && !secondValue.NeedsTarget())
				{
					return IntifyFloat(GetValue(callInfo)).ToString();
				}
				bool extraMultUsed2 = false;
				if (mainSubValue.NeedsTarget())
				{
					finalString += mainSubValue.SubValueString(callInfo, skillOverride);
				}
				else
				{
					finalString += IntifyFloat(extraMultiplier * mainSubValue.GetValue(callInfo, skillOverride));
					extraMultUsed2 = true;
				}

				finalString += ") / (";
				if (secondValue.NeedsTarget())
				{
					finalString += secondValue.SubValueString(callInfo, skillOverride);
				}
				else
				{
					finalString += IntifyFloat(extraMultiplier * secondValue.GetValue(callInfo, skillOverride));
					extraMultUsed2 = true;
				}

				finalString = baseMult == 1 ? finalString : intyBaseMult + finalString;
				finalString = baseNumber == 0 ? finalString : finalString + intyBaseNumber;
				finalString += ")";

				if (extraMultUsed2)
					return finalString;
				break;
			case Type.SkillUpgrade:
				return IntifyFloat(GetValue(callInfo));
		}

		return extraMultiplier == 1f ? finalString : "(" + finalString +") * " + extraMultiplier;

		throw new System.Exception("Something went wrong with BeautyString, failed on: " + type.EnumToString());

	}

	public static string IntifyFloat(float f, bool usePercents = true, int maxDecimals = -1)
	{
		//maybe percents in the future
		/*if (f < 1f && usePercents)
			return ((int)(f * 100f)).ToString() + "%";*/
		if (maxDecimals >= 0)
		{
			float pow = Mathf.Pow(10, maxDecimals);
			f = ((int)(f * pow)) / pow;
		}

		return (f < .5f ? (int)((f + (f < 0 ? -.001 : .001))*100)/100f : (((int)f < 3 ? (int)((f + (f < 0 ? -.01 : .01)) * 10) / 10f : (int)f))).ToString();
	}

	public bool NaiveIsZero()
    {
		return type == Type.Number && baseMult == 0f && baseNumber == 0f;
    }

	public string ValueString(CallInfo callInfo, Skill skillOverride = null, float extraMultiplier = 1f)
	{
		string intyBaseNumber = IntifyFloat(baseNumber, false);
		
		string intyBaseMult = IntifyFloat(baseMult);
		bool isPercent = intyBaseMult[intyBaseMult.Length - 1] == '%';

		switch (type)
		{
			case Type.Number:
				return (IntifyFloat((baseNumber + baseMult) * extraMultiplier)).ToString();
			case Type.Single:
				string s = baseNumber == 0f ? "(" + mainSubValue.SubValueString(callInfo, skillOverride, extraMultiplier) + ")" :
					"(" + mainSubValue.SubValueString(callInfo, skillOverride, extraMultiplier) + ") + " + (intyBaseNumber).ToString();
				s = baseMult == 1f ? s : (intyBaseMult).ToString() + " x " + s;
				return s;
			case Type.Add:
				string s2 = "(" + mainSubValue.SubValueString(callInfo, skillOverride) + ") + " +
					"(" + secondValue.SubValueString(callInfo, skillOverride) + ")";
				s2 = baseMult == 1f ? s2 : (intyBaseMult).ToString() + " x " + s2;
				s2 += baseNumber == 0f ? s2 + " + " + (intyBaseNumber).ToString() : s2;
				s2 = extraMultiplier == 1f ? s2 : "(" + s2 + ") * " + IntifyFloat(extraMultiplier);
				return s2;
			case Type.Multiply:
				string s3 = "(" + mainSubValue.SubValueString(callInfo, skillOverride, extraMultiplier) + ") x (" + secondValue.SubValueString(callInfo, skillOverride) + ")";
				if (isPercent)
					s3 = baseMult == 1f ? s3 : (intyBaseMult) + " of " + s3;
				else
					s3 = baseMult == 1f ? s3 : (intyBaseMult) + " x " + s3;
				
				s3 = baseNumber == 0f ? s3 : s3 + " + " + (intyBaseNumber).ToString();
				return s3;
			case Type.ThreeMultiply:
				string s4 = "(" + mainSubValue.SubValueString(callInfo, skillOverride, extraMultiplier) + ") x (" + secondValue.SubValueString(callInfo, skillOverride) + ")";
				s4 += "x (" + thirdValue.SubValueString(callInfo, skillOverride) + ")";
				s4 = baseMult == 1f ? s4 : (intyBaseMult).ToString() + " x " + s4;
				s4 = baseNumber == 0f ? s4 : s4 + " + " + (intyBaseNumber).ToString();
				return s4;
			case Type.Ability:
				return "An ability based Value";
			case Type.Divide:
				string s6 = "(" + mainSubValue.SubValueString(callInfo, skillOverride, extraMultiplier) + ") / (" + secondValue.SubValueString(callInfo, skillOverride) + ")";
				s6 = baseNumber == 0f ? s6 : s6 + " + " + (intyBaseNumber).ToString();
				return s6;
			case Type.SkillUpgrade:
				if (skillOverride == null && (callInfo.caster == null || callInfo.caster.playerSkills == null || callInfo.ability == null || callInfo.ability.mySkill == null))
					return base.ToString();
				Skill skill = skillOverride == null ? callInfo.ability.mySkill : skillOverride;
				return callInfo.ability.mySkill.GetUpgradeValueString(upgradeID, callInfo.caster.playerSkills, callInfo);
		}

		return base.ToString();
	}

	/// Returns (false,null) if values can't be combined
	public static (bool,Value) CombineValues(Value value1, Value value2)
    {
		if (value1.type == Type.Number && value2.type == Type.Number)
		{
			float v1 = (value1.baseNumber + value1.baseMult);
			float v2 = (value2.baseNumber + value2.baseMult);
			Value newValue = new Value()
			{
				type = Type.Number,
				baseNumber = 0,
				baseMult = v1 + v2
			};
			return (true, newValue);
		}
		else
			return (false, null);
    }
}



[System.Serializable]
public class SubValue
{
	public enum Type
	{
		Number,
		SkillUpgrade,
		DeltaTime,
		StoredFloat,
		Trait,
		AbilityBonus,
		Random,
		PLACEHOLDER,
		Ability,
		GlobalValue,
		PLACEHOLDER2,
		PLACEHOLDER3,
	}

	[LabelWidth(130)]
	public Type type = Type.Number;

	[HideIf("@type == Type.Ability || type == Type.SkillUpgrade")]
	[LabelWidth(130)]
	[Tooltip("Final number = baseNumber x Stat Calculation (possibly 1) + independentAdd")]
	public float baseNumber = 1;

	[LabelWidth(130)]
	[HideIf("@type == Type.Number || type == Type.Ability || type == Type.SkillUpgrade")]
	[Tooltip("Final number = baseNumber x Stat Calculation (possibly 1) + independentAdd")]
	public float independentAdd = 0;

	[LabelWidth(130)]
	[ShowIf("@type == Type.Random")]
	[Tooltip("Minimum Value")]
	public float min = 0f;

	[LabelWidth(130)]
	[ShowIf("@type == Type.Random")]
	[Tooltip("Maximum value")]
	public float max = 1f;

	[LabelWidth(130)]
	[ShowIf(nameof(type), Type.StoredFloat)]
	[Tooltip("StoredFloatID")]
	public string StoredID;
	[LabelWidth(130)]
	[ShowIf(nameof(type), Type.StoredFloat)]
	[Tooltip("Default float if it hasn't been set")]
	public float defaultFloat = 0f;
	[LabelWidth(130)]
	[ShowIf(nameof(type), Type.StoredFloat)]
	[Tooltip("If target is true it checks a float on the target instead.")]
	public bool checkTarget = false;

	[LabelWidth(130)]
	[ShowIf(nameof(type), Type.StoredFloat)]
	[Header("By default this is character wide")]
	public bool AbilityWide = false;


	[LabelWidth(130)]
	[ShowIf("@type == Type.StoredFloat && !AbilityWide")]
	[Header("SaveWide?")]
	public bool SaveWide = false;

	//bonuses
	[LabelWidth(130)]
	[ShowIf(nameof(type), Type.Trait)]
	[Tooltip("Which Trait to test")]
	public Trait traitType;

	[LabelWidth(130)]
	[ShowIf(nameof(type), Type.AbilityBonus)]
	public AbilityBonus abilityBonus = AbilityBonus.None;

	[LabelWidth(130)]
	[ShowIf("@type == Type.Trait || type == Type.AbilityBonus")]
	[Tooltip("Mark true if lower values are better")]
	public bool negateTrait = false;

	[LabelWidth(130)]
	[ShowIf(nameof(type), Type.Trait)]
	[Tooltip("If target is true it check bonus on the target instead.")]
	public bool checkTargetTrait = false;


	[LabelWidth(130)]
	[ShowIf(nameof(type), Type.Ability)]
	[Tooltip("Which ability number")]
	public int abilityValueNumber = 0;

	[LabelWidth(130)]
	[ShowIf(nameof(type), Type.SkillUpgrade)]
	[Tooltip("Fill in skill upgrade.")]
	public UpgradeID upgradeID = UpgradeID.DAMAGE_A;

	//global values
	public enum GlobalValueType
	{
		NumberOfAlivePlayers,
		NumberOfTotalPlayers,
	}

	[LabelWidth(130)]
	[ShowIf(nameof(type), Type.GlobalValue)]
	[Tooltip("Which Global value")]
	public GlobalValueType globalValueType;

	public bool NeedsTarget()
	{
		switch (type)
		{
			case Type.StoredFloat:
				if (checkTarget)
					return true;
				return false;
			case Type.Trait:
				if (checkTargetTrait)
					return true;
				return false;
			case Type.Number:
			case Type.DeltaTime:
			case Type.Random:
			case Type.GlobalValue:
			case Type.SkillUpgrade:
			case Type.AbilityBonus:
			case Type.Ability:
				return false;

		}
		throw new System.Exception("NeedsTarget incomplete. Missing: " + type.EnumToString());
	}

	public float GetValue(CallInfo callInfo, Skill skillOverride)
	{
		float finalReturn = baseNumber + independentAdd;
		switch (type)
		{
			case Type.Number:
				finalReturn = baseNumber;
				break;
			case Type.DeltaTime:
				finalReturn = Time.deltaTime * baseNumber + independentAdd;
				break;
			case Type.Random:
				finalReturn =  baseNumber * Random.Range(min, max) + independentAdd;
				break;
			case Type.StoredFloat:
				Character t = callInfo.caster;
				if (checkTarget)
					t = callInfo.target;
				if (t == null)
                {
					finalReturn = defaultFloat * baseNumber + independentAdd;
					break;
				}
				finalReturn = t.CheckStoredFloat(callInfo.ability, callInfo.itemID, StoredID, defaultFloat, !AbilityWide, SaveWide) * baseNumber + independentAdd;
				break;
			case Type.Trait:
				Character t2 = callInfo.caster;
				if (checkTargetTrait)
					t2 = callInfo.target;
				if (t2 == null)
                {
					finalReturn = baseNumber + independentAdd;
					break;
                }
				finalReturn = t2.CheckTrait(traitType, baseNumber, independentAdd, negateTrait);
				break;
			case Type.Ability:
				finalReturn = callInfo.ability.Values[abilityValueNumber].GetValue(callInfo);
				break;
			case Type.GlobalValue:
				float v = 0f;
				switch (globalValueType)
                {
					case GlobalValueType.NumberOfAlivePlayers:
						v = Character.NumberOfAlivePlayers;
						break;
					case GlobalValueType.NumberOfTotalPlayers:
						v = Character.NumberOfTotalPlayers;
						break;
				}
				finalReturn = baseNumber * v + independentAdd;
				break;
			case Type.SkillUpgrade:
				if (skillOverride == null &&(callInfo.ability == null || callInfo.ability.mySkill == null))
					break;
				Skill skill = skillOverride == null ? callInfo.ability.mySkill : skillOverride;
				finalReturn = skill.GetUpgradeValue(upgradeID, callInfo);
                break;
            case Type.AbilityBonus:
				if (callInfo.caster == null || callInfo.ability == null)
					break;
				finalReturn = callInfo.caster.CheckAbilityBonus(abilityBonus, callInfo.ability, baseNumber, independentAdd, negateTrait);
				break;
        }

        return finalReturn;

		//throw new System.Exception("Something went wrong with SubValue Calculation");

	}

	public string SubValueString(CallInfo callInfo, Skill skillOverride, float extraMultiplier = 1f)
	{
		string s = "";
		switch (type)
		{
			case Type.Number:
				return Value.IntifyFloat(extraMultiplier * baseNumber, false);
			case Type.DeltaTime:
				return Value.IntifyFloat(extraMultiplier * baseNumber, false) + " per second";
			case Type.Random:
				return "between " + Value.IntifyFloat((min + independentAdd) * extraMultiplier, false) + " and " 
					+ Value.IntifyFloat((max + independentAdd) * extraMultiplier, false);
			case Type.StoredFloat:
				if (checkTarget)
					s += "the target's ";
				else
					s += "your ";

				s += StoredID + (baseNumber == 1 ? "" : " x " + Value.IntifyFloat(baseNumber * extraMultiplier, false)) +
						(independentAdd == 0 ? "" : " + " + Value.IntifyFloat(independentAdd * extraMultiplier, false));
				break;
			case Type.Trait:
				if (checkTargetTrait)
					s += "the target's ";
				else
					s += "your ";
				s += traitType.EnumToString() + (baseNumber == 1 ? "" : " x " + Value.IntifyFloat(baseNumber * extraMultiplier, true)) +
						(independentAdd == 0 ? "" : " + " + Value.IntifyFloat(independentAdd * extraMultiplier, false));
                break;
            case Type.AbilityBonus:
				s += "the ability's ";
				s += abilityBonus.EnumToString() + (baseNumber == 1 ? "" : " x " + Value.IntifyFloat(baseNumber * extraMultiplier, true)) +
						(independentAdd == 0 ? "" : " + " + Value.IntifyFloat(independentAdd * extraMultiplier, false));
				break;
			case Type.Ability:
				return "An ability based Value";
			case Type.SkillUpgrade:
				if (callInfo.ability != null && callInfo.ability.mySkill == null && callInfo.caster == null && callInfo.caster.playerSkills == null)
                {
					return callInfo.ability.mySkill.GetUpgradeValueString(upgradeID, callInfo.caster.playerSkills, callInfo);
                }
				break;
        }

        return s;
	}

}
