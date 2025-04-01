using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New NothingControl", menuName = "Ability/Control/Nothing")]
public class NothingControl : Control
{
    public bool isPassive = true;
    public override bool IsPassive()
    {
        return isPassive;
    }
    public override bool ActualHold()
    {
        return false;
    }

    public override bool ActualPress()
    {
        return false;
    }

    public override bool ActualRelease()
    {
        return false;
    }
}
