using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


[CreateAssetMenu(fileName = "New Attribute Generator", menuName = "Items/AttributeGenerator")]
public class AttributeGenerator : SerializedScriptableObject
{
	[Tooltip("ID must be unique")]
	public string ID;

	[Tooltip("Don't show descriptions of this attribute, only works for ability attributes (look into if necessary?)")]
	public bool HideThisAttribute = false;

	[Tooltip("Chance that this will be rerolled if chosen as an Attribute.")]
	public float RerollChance = 0;

	[Tooltip("If set, ignore everything except RerollChance")]
	public AttributeGenerator duplicateThisGenerator = null;

	[HideIf("@duplicateThisGenerator == null")]
	public bool OverwriteConditions = false;

	[ShowIf("@duplicateThisGenerator == null")]
	public List<Bonus> possibleBonuses = new List<Bonus>();
	[ShowIf("@duplicateThisGenerator == null")]
	public int minBonuses = 0;
	[ShowIf("@duplicateThisGenerator == null")]
	public int maxBonuses = int.MaxValue;

	[ShowIf("@duplicateThisGenerator == null")]
	public List<Ability> possibleActiveAbilities = new List<Ability>();
	[ShowIf("@duplicateThisGenerator == null")]
	public int minActives = 0;
	[ShowIf("@duplicateThisGenerator == null")]
	public int maxActives = 0;
	[ShowIf("@duplicateThisGenerator == null")]
	public List<Ability> possiblePassiveAbilities = new List<Ability>();
	[ShowIf("@duplicateThisGenerator == null")]
	public int minPassives = 0;
	[ShowIf("@duplicateThisGenerator == null")]
	public int maxPassives = 0;
	[ShowIf("@duplicateThisGenerator == null")]
	public int maxAbilities = int.MaxValue;


	[Tooltip("Block Info")]
	public bool CanBlock = false;

	[ShowIf(nameof(CanBlock))]
	[Tooltip("Block Info")]
	public BlockInfo blockInfo = new BlockInfo();


	public bool OverrideDescription = false;
	[ShowIf(nameof(OverrideDescription))]
	[TextArea(15, 20)]
	public string Description = "";

	public Attribute Generate()
	{
		return GenerateWrapper();
	}


	List<int> tempInts = new List<int>();
	protected virtual Attribute GenerateWrapper()
    {
		AttributeGenerator generator = GetDuplicate();


		Attribute a = new Attribute();
		a.myAttributeGenerator = this;

		int numBonuses = Random.Range(minBonuses, maxBonuses);
		for (int i = 0; i < numBonuses; i++)
		{
			if (generator.possibleBonuses.Count == 0)
				break;
			Bonus newBonus = generator.possibleBonuses[ReturnUniqueIndex(generator.possibleBonuses, tempInts)].Duplicate();
			newBonus.type = Bonus.Type.Item;
			a.bonuses.Add(newBonus);
		}

		int passives = Random.Range(generator.minPassives, generator.maxPassives);
		int actives = Random.Range(generator.minActives, generator.maxActives);

		while (passives + actives > generator.maxAbilities && generator.maxAbilities >= 0)
		{
			if (Random.value > .5 && passives > 0)
				passives--;
			else if (actives > 0)
				actives--;
		}

		for (int i = 0; i < passives; i++)
		{
			int toAdd;
			string currentAbility;
			do
			{
				toAdd = Random.Range(0, generator.possiblePassiveAbilities.Count);
				currentAbility = generator.possiblePassiveAbilities[toAdd].ID;
			} while (a.passiveAbilities.Contains(currentAbility) && i < generator.possiblePassiveAbilities.Count);
			a.passiveAbilities.Add(currentAbility);
		}

		for (int i = 0; i < actives; i++)
		{
			int toAdd;
			string currentAbility;
			do
			{
				toAdd = Random.Range(0, generator.possibleActiveAbilities.Count);
				currentAbility = generator.possibleActiveAbilities[toAdd].ID;
			} while (a.activeAbilities.Contains(currentAbility) && i < generator.possibleActiveAbilities.Count);
			a.activeAbilities.Add(currentAbility);
		}

		for (int i = 0; i < actives * 5; i++)
		{
			a.randomActiveFloats.Add(Random.value);
		}

		for (int i = 0; i < passives * 5; i++)
		{
			a.randomPassiveFloats.Add(Random.value);
		}

		
		a.UniqueID = GetUniqueID(a, generator);

		if (Attribute.AllAttributes.Count == 0)
        {
			Attribute.AllAttributes[""] = null;
        }
		if (CanBlock)
        {
			a.UniqueID += "B";
        }

		if (CanBlock)
        {
			//blockInfo.AttributeID = a.UniqueID;
        }

		foreach (Bonus b in a.bonuses)
        {
			//b.attributeID = a.UniqueID;
        }

		return a;
	}
	static int ReturnUniqueIndex<T>(List<T> possibleIndex, List<int> indexStorage)
	{
		int index;
		do
		{
			index = Random.Range(0, possibleIndex.Count);
		} while (indexStorage.Contains(index) && indexStorage.Count < possibleIndex.Count);
		indexStorage.Add(index);
		return index;
	}

	static string GetUniqueID(Attribute a, AttributeGenerator generator)
    {
		string uniqueID;
		uniqueID = generator.ID;
		foreach (float f in a.randomActiveFloats)
		{
			uniqueID += f.ToString();
		}
		foreach (float f in a.randomPassiveFloats)
		{
			uniqueID += f.ToString();
		}
		uniqueID += generator.ID;
		uniqueID += "A";
		foreach (string i in a.activeAbilities)
		{
			uniqueID += i;
		}
		uniqueID += "P";
		foreach (string p in a.passiveAbilities)
		{
			uniqueID += p;
		}
		foreach (var b in a.bonuses)
		{
			uniqueID += "b" + b.trait.ToString();
		}
		uniqueID += Universal.RandomString();

		return uniqueID;
	}

	public static AttributeGenerator SelectFromList(List<AttributeGenerator> possibleGenerators)
    {
		int tries = 10;
		AttributeGenerator agen;
		do
		{
			agen = possibleGenerators[Random.Range(0, possibleGenerators.Count)];
			tries--;
		} while (tries > 0 && agen.RerollChance > Random.value);
		return agen;
    }

	public AttributeGenerator GetDuplicate()
    {
		return duplicateThisGenerator == null ? this : duplicateThisGenerator.GetDuplicate();
    }

    private void OnEnable()
    {
		allGenerators.Add(this);
	}

    public static AttributeGenerator GetGeneratorFromID(string ID)
	{

		foreach (AttributeGenerator ag in allGenerators)
		{
			if (ag.ID == ID)
			{
				return ag;
			}
		}
		Debug.Log(ID + " Attribute Generator not found");
		return null;
	}

	/// <summary>
	/// Does Attribute attribute have a matching attribute in checkattributes?
	/// </summary>
	/// <returns>True if: an attribute in checkAttribute has the same ___ as attribute:
	///		Trait bonus Trait, Resistance Damage Type, Stat bonus Stat, passive ability, active ability
	///		
	/// False otherwise</returns>
	public static bool AttributeRepeat(List<Attribute> checkAttributes, Attribute attribute)
    {
		foreach (Attribute a in checkAttributes)
		{
			//NOTE: NO longer checks for trait bonuses
			foreach (string i1 in a.activeAbilities)
            {
				foreach (string i2 in attribute.activeAbilities)
                {
					if (i1 == i2)
                    {
						return true;
                    }
                }
            }

			foreach (string i1 in a.passiveAbilities)
			{
				foreach (string i2 in attribute.passiveAbilities)
				{
					if (i1 == i2)
					{
						return true;
					}
				}
			}
		}

		return false;
    }


	static List<AttributeGenerator> allGenerators = new List<AttributeGenerator>();
	public static void TryAddGenerator(AttributeGenerator generator)
	{
		allGenerators.Add(generator);
	}

}
