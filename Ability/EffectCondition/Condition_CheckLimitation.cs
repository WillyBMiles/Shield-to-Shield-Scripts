using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "new CheckLimitation", menuName = "Ability/EffectCondition/CheckLimitation")]
public class Condition_CheckLimitation : EffectCondition
{

    public Limitation limitation;
	// Start is called before the first frame update
	public override bool CheckCondition(CallInfo callInfo)
	{
        bool canCast = limitation.CanCast(callInfo);
        bool finalReturn = (limitation.invert ^ canCast) && base.CheckCondition(callInfo);
        return finalReturn;
	}

    public override void Initialize(Activation activation)
    {
        base.Initialize(activation);
        limitation.Initialize(activation);
    }
}
