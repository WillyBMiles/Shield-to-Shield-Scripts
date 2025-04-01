using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class Effect_ArrowUI : Effect_UI
{
	public override void LocalEffect(CallInfo callInfo)
	{
		if (!callInfo.caster.hasAuthority)
			return;

		GameObject obj = null;
		if (TargetCharacter)
		{
			if (callInfo.target != null)
				obj = Object.Instantiate(UIHandle, callInfo.target.transform.position, Quaternion.identity);
		}
		else
		{
			if (callInfo.targetPoint.HasValue)
				obj = Object.Instantiate(UIHandle, callInfo.targetPoint.Value, Quaternion.identity);
		}

		if (obj != null)
		{
			UIArrow arrow = obj.GetComponent<UIArrow>();
			Vector3 originPosition = callInfo.targetPoint.Value;
			if (callInfo.origin != null)
			{
				originPosition = callInfo.origin.transform.position;
			}

			if (float.IsInfinity(originPosition.x + originPosition.y + originPosition.z))
			{
				Debug.Log("Effect_Arrow does not have a proper origin");
				return;
			}
			obj.transform.position = new Vector3(obj.transform.position.x, YPosition, obj.transform.position.z);
			float rad = radius.GetValue(callInfo);
			obj.transform.localScale = new Vector3(rad, rad, rad);
			arrow.PointTowards(originPosition, obj.transform.position, 1f);
			
			
		}
	}

}
