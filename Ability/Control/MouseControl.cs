using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New MouseControl", menuName = "Ability/Control/Mouse")]
public class MouseControl : Control
{
	public int mouseButton;

	bool didPress = false;
	public override bool IsPassive()
	{
		return false;
	}
	public override bool ActualHold()
	{
		if (InMenus())
			return false;
		if (Input.GetMouseButton(mouseButton))
        {
			return didPress;
		}
		else
        {
			didPress = false;
        }
		return false;
	}

	public override string ToString()
	{
		return "Mouse: " + mouseButton;
	}

    public override bool ActualPress()
    {
		if (InMenus())
			return false;
		if (Input.GetMouseButtonDown(mouseButton))
		{
			didPress = true;
			return true;
		}
		return false;
	}

    public override bool ActualRelease()
    {
		if (InMenus())
			return false;
		if (Input.GetMouseButtonUp(mouseButton))
        {
			didPress = false;
			return true;
        }
		return false;
	}
}
