using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class Limitation_CompareValues : Limitation
{
	public enum Sign
    {
		LessThan,
		EqualTo,
		GreaterThan
    }

	public Value value1 = new Value();
	public Sign sign;
	public Value value2 = new Value();

	[Tooltip("Target for value 1")]
	[ValueDropdown(nameof(_targets))]
	public string Target1 = "SELF";

	[Tooltip("Target for value 2")]
	[ValueDropdown(nameof(_targets))]
	public string Target2 = "SELF";

	public override void Initialize(Activation activation)
	{
		base.Initialize(activation);
	}

	static List<Character> charList;
	public override bool CanCast(CallInfo callInfo)
	{
		Character t1 = null;
		Character t2 = null;
		if (Target1 != null && Target2 != null)
        {
			if (charList == null)
				charList = new List<Character>();
			callInfo.caster.characterAbilities.GetTargets(callInfo, charList, Target1);
			
			if (charList.Count > 0)
				t1 = charList[0];
            callInfo.caster.characterAbilities.GetTargets(callInfo, charList, Target2);
			
			if (charList.Count > 0)
				t2 = charList[0];
		}


		float v1 = value1.GetValue(callInfo);
		float v2 = value2.GetValue(callInfo);

		switch (sign)
        {
			case Sign.EqualTo:
				return v1 == v2;
			case Sign.GreaterThan:
				return v1 > v2;
			case Sign.LessThan:
				return v1 < v2;
        }

		throw new System.Exception("Something went wrong with Comparison, sign: " + sign.EnumToString());
	}
}
