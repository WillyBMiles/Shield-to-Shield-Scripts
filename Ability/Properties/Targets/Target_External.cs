using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class Target_External : Target
{
    [Header("Use to reference an external target structure. (Like universal rooms)")]
    public bool __;

    protected override void TargetActivate(CallInfo callInfo)
    {
        return;
    }

    public override bool IsDeterministic()
    {
        return false;
    }
}
