using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Interrupt : Effect
{
    public bool showAnimation = false;
    public bool allActivations = false;
    public bool windingUp = false;
    public override void LocalEffect(CallInfo callInfo)
    {
        base.LocalEffect(callInfo);

        if (allActivations)
        {
            foreach (Activation a in callInfo.ability.activations)
            {
                a.Interrupt(callInfo, windingUp, showAnimation, !callInfo.activation.DontSyncAnything);
            }
        }else
        {
            callInfo.activation.Interrupt(callInfo, windingUp, showAnimation, !callInfo.activation.DontSyncAnything);
        }
        
    }

    public override bool HasLocalOnlyEffect()
    {
        return false;
    }

    public override bool HasServerEffect()
    {
        return false;
    }

    protected override bool AbstractHasLocalEffect()
    {
        return true;
    }

    public override bool CanHitCharacters()
    {
        return true;
    }

    public override bool CanHitPoints()
    {
        return true;
    }
}
