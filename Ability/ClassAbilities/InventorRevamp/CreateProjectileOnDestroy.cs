using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateProjectileOnDestroy : MonoBehaviour
{
    public GameObject NewProjectile;

    private void OnDestroy()
    {
        Instantiate(NewProjectile, transform.position, Quaternion.identity);
    }
}
