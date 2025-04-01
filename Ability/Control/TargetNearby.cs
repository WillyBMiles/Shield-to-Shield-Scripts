using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


[CreateAssetMenu(fileName = "New TargetNearby", menuName = "Ability/Control/TargetNearby")]
public class TargetNearby : Control
{
	[HideIf(nameof(TargetSelf))]
	public TargetMask mask;

	[HideIf(nameof(TargetSelf))]
	[Tooltip("Target the furthest instead of the closest character")]
	public bool FurthestInstead;

	[HideIf(nameof(TargetSelf))]
	public bool RandomInstead = false;

	public bool TargetSelf = false;
	public bool TargetUntargettable = false;
	public bool TargetTrulyUntargettable = false;


	public override bool IsPassive()
	{
		return true;
	}
	public override bool ActualHold()
	{
		return true;
	}

	List<Character> tempCharacters = new List<Character>();
	public override Vector3 Cursor(Character source)
	{
		if (TargetSelf)
        {
			if (source != null)
				return source.transform.position;
			return new Vector3();

        }

		source.GetAllInRange(source.transform.position, tempCharacters, float.PositiveInfinity, mask, TargetUntargettable, TargetTrulyUntargettable);
		tempCharacters.Remove(source);
		if (tempCharacters.Count == 0)
		{
			if (source != null)
				return source.transform.position;
			return new Vector3(0,0,0);
		}
		Character character;
		int i = 0;
		do
		{
			if (i >= tempCharacters.Count)
            {
				if (RandomInstead)
                {
					character = tempCharacters[Random.Range(0, tempCharacters.Count)];
					break;
                }
				if (source != null)
					return source.transform.position;
				return new Vector3(0, 0, 0);
			}
			
			if (RandomInstead)
				character = tempCharacters[Random.Range(0, tempCharacters.Count)];
			else 
				character = FurthestInstead ? tempCharacters[tempCharacters.Count - i - 1] : tempCharacters[i];
			i++;
		} while (character == null || character.Dead );
		return character.transform.position;

		
	}

    public override bool ActualPress()
    {
		return true;
    }

    public override bool ActualRelease()
    {
		return true;
    }
}
