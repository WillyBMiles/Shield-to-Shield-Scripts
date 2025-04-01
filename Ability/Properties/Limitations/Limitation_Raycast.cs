using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Limitation_Raycast : Limitation
{
    [Header("Returns false if there is something between start and end on the layer LayerMask. Only checks the first in each target category.")]
    [ValueDropdown(nameof(_targets))]
    public string start = "SELF";
    public bool startCharacters = true;
    [ValueDropdown(nameof(_targets))]
    public string end = "CURSOR";
    public bool endCharacters = false;

    public LayerMask layerMask = (int)Mathf.Pow(2, 7); //"Walls"

    public bool StayAtHeight = true;

    [ShowIf(nameof(StayAtHeight))]
    public int height = 1;

    public bool CanCastIfNoTargets = false;
    public override void Initialize(Activation activation)
    {
        base.Initialize(activation);
        tempCs = new List<Character>();
        tempPs = new List<Vector3>();
    }

    List<Character> tempCs;
    List<Vector3> tempPs;
    public override bool CanCast(CallInfo callInfo)
    {
        Vector3 startpoint;
        Vector3 endpoint;

        if (startCharacters)
        {
            callInfo.caster.characterAbilities.GetTargets(callInfo, tempCs, start);
            if (tempCs.Count == 0)
                return CanCastIfNoTargets;
            startpoint = tempCs[0].transform.position;
        }else
        {
            callInfo.caster.characterAbilities.GetTargets(callInfo, tempPs, start);
            if (tempPs.Count == 0)
                return CanCastIfNoTargets;
            startpoint = tempPs[0];
        }

        if (endCharacters)
        {
            callInfo.caster.characterAbilities.GetTargets(callInfo, tempCs, end);
            if (tempCs.Count == 0)
                return CanCastIfNoTargets;
            endpoint = tempCs[0].transform.position;
        }
        else
        {
            callInfo.caster.characterAbilities.GetTargets(callInfo, tempPs, end);
            if (tempPs.Count == 0)
                return CanCastIfNoTargets;
            endpoint = tempPs[0];
        }

        if (StayAtHeight)
        {
            startpoint = new Vector3(startpoint.x, height, startpoint.z);
            endpoint = new Vector3(endpoint.x, height, endpoint.z);
        }

        if (Physics.Raycast(startpoint, (endpoint - startpoint).normalized ,Vector3.Distance(startpoint,endpoint), layerMask ))
        {
            return false;
        }
        return true;

    }
}
