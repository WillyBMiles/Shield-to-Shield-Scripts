using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.EventSystems;

public static class ControlExtension
{
	public static void WriteControl(this NetworkWriter writer, Control control)
    {
		if (control == null)
        {
			writer.Write<bool>(false);
			return;
		}
			
		writer.Write<bool>(true);
		writer.Write(control.UniqueID);
    }

	public static Control ReadControl(this NetworkReader reader)
    {
		bool b = reader.Read<bool>();
		if (b)
        {
			return Control.AllControls[reader.Read<string>()];
		}
		return null;
		
    }

}

public abstract class Control : ScriptableObject
{
	public string UniqueID = "";

	public static Dictionary<string, Control> AllControls = new Dictionary<string, Control>();

    private void OnEnable()
    {
		AllControls[UniqueID] = this;

    }

	public abstract bool IsPassive();

    public bool Press()
    {
		var state = Gatherer.GetState(this);
		if (state == null)
			return false;
		return state.isPressed;
    }
	public bool Hold()
    {
		var state = Gatherer.GetState(this);
		if (state == null)
			return false;
		return state.isHolding;
	}

	public abstract bool ActualHold();
	public abstract bool ActualPress();
	public abstract bool ActualRelease();
	public bool Release()
    {
		var state = Gatherer.GetState(this);
		if (state == null)
			return false;
		return state.isReleased;
	}

	public class State
    {
		public bool isHolding;
		public bool didPress;
		public bool isPressed;
		public bool isReleased;
	}

	public void UpdateThis(State state)
    {
		if (state.isReleased)
        {
			state.isReleased = false;
        }
			
		if (state.isPressed)
        {
			state.isPressed = false;
		}
			

		if ((ActualRelease() && state.isHolding) || (state.isHolding && !ActualHold()))
        {
			state.isReleased = true;
		}
		if ((ActualPress() && state.isHolding) || (!state.isHolding && ActualHold()) )
        {
			state.isPressed = true;
		}
		state.isHolding = ActualHold();
    }

	public static bool InMenus()
	{
		
		return Universal.InMenus();//(AbilityBarSlot.currentSlot != null && Input.GetButton("Show Details")) || ItemList.hovering || AbilityBarSlot.swappingSlots 
			//|| PopupButton.hovering || EscapeMenu.InMenu || Blacksmith.Hovering || LevelUpController.InMenus;
	}

	public virtual Vector3 Cursor(Character source)
	{
		return LocalMouseCursor();
	}

	Vector3 lastKnownMousePosition = new Vector3();
	float lastMouseGotten = -1f;
	Vector3 LocalMouseCursor()
	{
		if (Time.time != lastMouseGotten && 
			Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f, LayerMask.GetMask("Ground Plane")))
		{
			lastKnownMousePosition = hit.point;
			lastMouseGotten = Time.time;
		}
		return lastKnownMousePosition;
	}

	public static bool IsPointerOverUIElement()
	{
		return IsPointerOverUIElement(GetEventSystemRaycastResults());
	}


	//Returns 'true' if we touched or hovering on Unity UI element.
	private static bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
	{
		for (int index = 0; index < eventSystemRaysastResults.Count; index++)
		{
			RaycastResult curRaysastResult = eventSystemRaysastResults[index];
			if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("UI"))
				return true;
		}
		return false;
	}


	//Gets all event system raycast results of current mouse or touch position.
	static List<RaycastResult> GetEventSystemRaycastResults()
	{
		PointerEventData eventData = new PointerEventData(EventSystem.current);
		eventData.position = Input.mousePosition;
		List<RaycastResult> raysastResults = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventData, raysastResults);
		return raysastResults;
	}


	public static Control GetControl(string UniqueID)
    {
		if (!AllControls.ContainsKey(UniqueID))
			return null;
		return AllControls[UniqueID];
    }

	List<Object> objs = new List<Object>();
#if false
#if UNITY_EDITOR
	private void OnValidate()
	{
		try
		{
			objs.Clear();
			objs.AddRange(UnityEditor.PlayerSettings.GetPreloadedAssets());
			if (!objs.Contains(this))
			{
				objs.Add(this);
			}
			objs.Remove(null);

			UnityEditor.PlayerSettings.SetPreloadedAssets(objs.ToArray());
		}
		catch
		{
			//PASS
		}

	}

#endif
#endif
}
