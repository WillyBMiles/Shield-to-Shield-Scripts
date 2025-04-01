using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public abstract class ProjectileMotion 
{
    public abstract void UpdateThis(LocalProjectile projectile, CallInfo callInfo, bool dontMove = false);


	/*
	[UnityEditor.MenuItem("Ability/Effect/Motion/MOTION")]
	static void AddMotion()
	{
		AddMotion<>();
	}
	*/
}
