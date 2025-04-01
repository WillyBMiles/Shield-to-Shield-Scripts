using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class ProjectileOwnerOutline : MonoBehaviour
{
    Outline outline;
    LocalProjectile lp;
    // Start is called before the first frame update
    void Start()
    {
        lp = GetComponentInParent<LocalProjectile>();
        outline = GetComponent<Outline>();
    }

    // Update is called once per frame
    void Update()
    {
        if (lp != null)
        {
            if (Player.LocalPlayer != null && lp.callInfo.caster == Player.LocalPlayer.character)
            {
                outline.enabled = true;
            }
            else
            {
                outline.enabled = false;
            }
        }
    }
}
