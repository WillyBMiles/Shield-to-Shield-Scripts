using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New KeyboardControl", menuName = "Ability/Control/Keyboard")]
public class KeyboardControl : Control
{
	public KeyCode code;

    public override bool IsPassive()
    {
		return false;
    }

    public override bool ActualHold()
	{
		return Input.GetKey(code);
	}

	public override string ToString()
	{
		return code.ToString();
	}

    public override bool ActualPress()
    {
        return Input.GetKeyDown(code);
    }

    public override bool ActualRelease()
    {
        return Input.GetKeyUp(code);
    }
}
