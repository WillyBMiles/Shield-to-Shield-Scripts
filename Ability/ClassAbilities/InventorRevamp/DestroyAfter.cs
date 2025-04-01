using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    public float Time;

    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(Destroy), Time);
    }

    private void Destroy()
    {
        GameObject.Destroy(gameObject);
    }
}
