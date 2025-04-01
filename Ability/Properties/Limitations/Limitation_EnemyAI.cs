using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Limitation_EnemyAI : Limitation
{
    [HideIf(nameof(isDodging))]
    public EnemyAI.Behaviour behaviour = EnemyAI.Behaviour.Fighting;
    public bool isDodging = false;

    public override bool CanCast(CallInfo callInfo)
    {
        if (callInfo.caster == null)
            return false;
        EnemyAI eai = callInfo.caster.enemyAI;
        if (eai == null)
            return false;
        if (isDodging)
        {
            return eai.behaviour switch
            {
                EnemyAI.Behaviour.DodgingShort => true,
                EnemyAI.Behaviour.DodgingLong => true,
                EnemyAI.Behaviour.DodgingProjectile => true,
                _ => false,
            };
        }
        return eai.behaviour == behaviour;
    }
}
