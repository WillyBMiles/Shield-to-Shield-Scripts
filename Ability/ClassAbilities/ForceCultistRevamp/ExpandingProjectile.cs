using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandingProjectile : MonoBehaviour
{
    LocalProjectile local;
    // Start is called before the first frame update
    void Start()
    {
        local = GetComponentInParent<LocalProjectile>();
        UpdateScale();
    }

    void UpdateScale()
    {
        float scale = 1 - local.currentDuration / local.maxDuration;
        transform.localScale = new Vector3(scale, scale, scale);
    }
    // Update is called once per frame
    void Update()
    {
        UpdateScale();
    }
}
