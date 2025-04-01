using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Limitation_Or : Limitation
{
    [Header("Limitations are not updated. Timing is shared with toplevel.")]
    [BoxGroup("Limitation 1")]
    public Limitation limitation1 = null;
    [BoxGroup("Limitation 2")]
    public Limitation limitation2 = null;

    public override void Initialize(Activation activation)
    {
        base.Initialize(activation);
        if (limitation1 != null)
        {
            limitation1.Initialize(activation);
            limitation1._targets = _targets;
        }
        if (limitation2 != null)
        {
            limitation2.Initialize(activation);
            limitation2._targets = _targets;
        }
    }

    public override void Activate(CallInfo callInfo)
    {
        base.Activate(callInfo);
        limitation1.Activate(callInfo);
        limitation2.Activate(callInfo);
    }

    public override bool CanCast(CallInfo callInfo)
    {
        if (limitation1 == null || limitation2 == null)
            return false;
        bool l1 = limitation1.invert ^ limitation1.CanCast(callInfo);
        bool l2 = limitation2.invert ^ limitation2.CanCast(callInfo);
        return l1 || l2;
    }

    public override void UpdateTargets(List<string> targets)
    {
        base.UpdateTargets(targets);
        limitation1.UpdateTargets(targets);
        limitation2.UpdateTargets(targets);
    }
}
