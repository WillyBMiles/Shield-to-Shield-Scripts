using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target_RandomInRoom : Target
{
    [Tooltip("Lower accuracy means faster execution, but worse distribution.")]
    public float accuracy = 1f;

    [Tooltip("Number of tries to get a point. Higher means better distribution but slower execution.")]
    public int numberOfTries = 10;

    [Tooltip("Can get multiple points in the room")]
    public int NumberOfInstances = 1;

    protected override void TargetActivate(CallInfo callInfo)
    {
        if (callInfo.caster == null)
            return;
        Room room = callInfo.caster.myRoom;
        if (room == null)
            return;
        bool firstClear = ClearTargets;
        for (int i = 0; i < NumberOfInstances; i++)
        {
            Vector3 v3 = room.GetRandomPoint(accuracy, numberOfTries);
            AddTargets(callInfo, firstClear, v3);
            firstClear = false;
        }
        
    }

    public override bool IsDeterministic()
    {
        return false;
    }
}
