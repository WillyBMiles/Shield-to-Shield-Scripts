using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Sirenix.OdinInspector;

public class Attribute
{
    [HideInInspector]
    public AttributeGenerator myAttributeGenerator { 
        get
        {
            if (_myAttributeGenerator == null)
                return null;
            return _myAttributeGenerator.GetDuplicate();
        } 
        set
        {
            _myAttributeGenerator = value;
        }
    }
    public AttributeGenerator _myAttributeGenerator;

    [HideInInspector]
    public List<float> randomPassiveFloats      = new List<float>();
    [HideInInspector]
    public List<float> randomActiveFloats       = new List<float>();

    [HideInInspector]
    public List<Bonus> bonuses                  = new List<Bonus>();

    [HideInInspector]
    public List<string> activeAbilities            = new List<string>();
    [HideInInspector]
    public List<string> passiveAbilities           = new List<string>();

    [HideInInspector]
    public string UniqueID { get => _UniqueID;
        set {
            if (_UniqueID != null)
                AllAttributes.Remove(_UniqueID);
            AllAttributes[value] = this;
            _UniqueID = value;
        } }
    string _UniqueID = "null";

    public string myItem;

    public static Dictionary<string, Attribute> AllAttributes = new Dictionary<string, Attribute>()
    {
        { "" , null }
    };
    public void InitializeOLD()
	{
        if (myAttributeGenerator == null)
        {
            UniqueID = "Class" + Random.value + "_" + Random.value;
            return;
        }

        
		foreach (string ability in activeAbilities)
		{
            Ability.GetAbility(ability).Initialize();
		}
        foreach (string ability in passiveAbilities)
        {
            Ability.GetAbility(ability).Initialize();
        }
    }

    /*
	public override string ToString()
	{
        return Stringify();
    }
    
    public string Stringify(bool simplifyAbilities = false, bool dontExpand = false)
    {
        if (myAttributeGenerator != null && myAttributeGenerator.HideThisAttribute)
            return "";
        if (myAttributeGenerator != null && myAttributeGenerator.OverrideDescription)
        {
            return myAttributeGenerator.Description + "\n";
        }

        bool beauty = Input.GetKey(KeyCode.LeftShift);
        string output = "";
        foreach (Bonus b in bonuses)
        {
            output += "+" + b.Stringify(beauty, Character.GetLocalCharacter());
            output += " \n";
        }
        if (myAttributeGenerator == null)
            return output;

        foreach (string i in activeAbilities)
        {
            Ability a = Ability.GetAbility(i);
            if (a == null)
                continue;
            if (simplifyAbilities)
                output += "\n<b>" + a.BeautyName + "</b>";
            else
                output += "\n<b>" + a.BeautyName + ":</b> " +
                    a.Description(Character.GetLocalCharacter(), this, !Input.GetButton("Show Details") || dontExpand) + "\n";
        }

        foreach (string i in passiveAbilities)
        {
            Ability a = Ability.GetAbility(i);
            if (a == null)
                continue;
            if (simplifyAbilities)
                output += "\n<b>" + a.BeautyName + "</b>";
            else
                output += "\n<b>" + a.BeautyName + ":</b> " +
                    a.Description(Character.GetLocalCharacter(), this, !Input.GetButton("Show Details") || dontExpand) + "\n";
        }

        return output;
    }
    */

    public float CheckRandomFloat(string abilityID, bool active)
	{
        List<string> abilities = active ? activeAbilities : passiveAbilities;
        int i = 0;
        foreach (string ability in abilities)
		{
            if (ability == abilityID)
			{
                return CheckRandomFloat(i, active);
			}
            i++;
		}
        return 0f;
    }

    public float CheckRandomFloat(int index, bool active)
	{

        List<float> randoms = active ? randomActiveFloats : randomPassiveFloats;
        int scaledIndex = index * 5;
        if (randoms.Count <= scaledIndex)
		{
            return 0f;
		}
        return randoms[scaledIndex];
	}
    public static Attribute GetAttribute(string ID)
    {
        if (ID == null)
            return null;
        if (AllAttributes.ContainsKey(ID))
            return AllAttributes[ID];
        return null;
    }

    public static BlockInfo GetBlockInfo(string ID)
    {
        Attribute a = GetAttribute(ID);
        if (a == null)
            return null;
        if (a.myAttributeGenerator == null)
            return null;
        if (!a.myAttributeGenerator.CanBlock)
            return null;
        return a.myAttributeGenerator.blockInfo;
    }

	public override int GetHashCode()
	{
        return UniqueID.GetHashCode();
	}

    public static string GetID(Attribute attribute)
    {
        if (attribute == null)
            return "";
        return attribute.UniqueID;
    }

    public bool Equals(Attribute other)
    {
        if (this == null && other == null)
            return true;
        if (other == null)
            return false;
        if (other is Attribute a)
        {
            return a.UniqueID == UniqueID;
        }
        return false;
    }

    public override bool Equals(object obj)
    {
        if (obj is Attribute a)
        {
            return this == a;
        }
        return false;
    }

    public static bool operator ==(Attribute v1, Attribute v2)
    {
        if (v1 is null && v2 is null)
            return true;
        if (v1 is null)
            return false;
        if (v2 is null)
            return false;
        return v1.UniqueID == v2.UniqueID;
    }
    public static bool operator !=(Attribute v1, Attribute v2)
    {
        bool b = (v1 == v2);
        return !b;
    }

    public void Destroy()
    {
        AllAttributes.Remove(UniqueID);
    }


}
