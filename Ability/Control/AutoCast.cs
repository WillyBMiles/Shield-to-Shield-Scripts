using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new AutoCast", menuName = "Ability/Control/AutoCast")]
public class AutoCast : Control
{
    public override bool IsPassive()
    {
		return true;
    }

    public override bool ActualHold()
	{
		return true;
	}

	public override string ToString()
	{
		return "Passive";
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
