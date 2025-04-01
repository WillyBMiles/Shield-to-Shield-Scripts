using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "new AITarget", menuName = "Ability/Control/AITarget")]
public class AITarget : Control
{

    public bool CharacterPositionInstead = false;
    [ShowIf(nameof(CharacterPositionInstead))]
    public bool PredictCharacterPosition = false;
    [Tooltip("Ranged predict ensures you don't shoot the wrong way")]
    [ShowIf(nameof(PredictCharacterPosition))]
    public bool RangedPredict = false;

    public override bool ActualPress()
    {
        return true;
    }

    public override bool ActualRelease()
    {
        return true;
    }

    public override bool ActualHold()
    {
        return true;
    }

    public override bool IsPassive()
    {
        return true;
    }

    public override Vector3 Cursor(Character source)
    {
        if (source.enemyAI != null)
        {
            if (CharacterPositionInstead)
            {

                if (source.enemyAI.target != null)
                {
                    if (PredictCharacterPosition)
                    {
                        if (RangedPredict)
                        {
                            return source.enemyAI.GetRangedPredictPoint();
                        }
                        return source.enemyAI.predictPoint;
                    }
                    return source.enemyAI.target.transform.position;
                }
            }
            else
            {
                return source.enemyAI.targetPoint;
            }
        }

        return base.Cursor(source);
    }
}
