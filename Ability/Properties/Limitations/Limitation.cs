using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

[System.Serializable]
public abstract class Limitation : Property
{
	public Limitation()
	{
		name = this.GetType().ToString() + Random.value;
	}

	[BoxGroup("Main", false)]
	//this is implemented in Activation.CanCast
	[Tooltip("if true Return opposite valuses, aka return true if target isn't in range or the ability IS on cooldown")]
	public bool invert = false;

	[Tooltip("if true can interrupt a windup")]
	public bool canInterrupt = false;

	[ShowIf(nameof(canInterrupt))]
	public bool canInterruptWinddown = false;

	[BoxGroup("Main", false)]
	[Tooltip("If true then you can't prepare the ability if this says you can't cast it")]
	public bool CantPrepare = false;

	[BoxGroup("Main", false)]
	[ShowIf(nameof(CantPrepare))]
	public string CustomCantPrepareMessage = "";

	[BoxGroup("Main", false)]
	[ShowIf(nameof(CantPrepare))]
	public Color CustomCantPrepareColor = Color.white;


	public abstract bool CanCast(CallInfo callInfo);

	/*
	[MenuItem("Ability/Activation/AddLimitation/TARGET")]
	static void AddLimitation()
	{
		AddLimitation<Target>();
	}
	*/

	public int GetID(Activation activation)
	{
		int i = 0;
		foreach (Limitation limitation in activation.limitations)
		{
			if (limitation == this)
				return i;
			i++;
		}
		return -1;
	}
}
