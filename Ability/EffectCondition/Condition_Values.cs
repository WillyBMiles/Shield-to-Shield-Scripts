using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Condition_Values", menuName = "Ability/EffectCondition/Values")]
public class Condition_Values : EffectCondition
{
    public Value value1 = new Value();
    public Limitation_CompareValues.Sign sign;
    public Value value2 = new Value();

    public override bool CheckCondition(CallInfo callInfo)
    {

        float v1 = value1.GetValue(callInfo);
        float v2 = value2.GetValue(callInfo);

        bool output = false;
        switch (sign)
        {
            case Limitation_CompareValues.Sign.EqualTo:
                output = v1 == v2;
                break;
            case Limitation_CompareValues.Sign.GreaterThan:
                output = v1 > v2;
                break;
            case Limitation_CompareValues.Sign.LessThan:
                output = v1 < v2;
                break;
        }

        return output && base.CheckCondition(callInfo);
    }
}
