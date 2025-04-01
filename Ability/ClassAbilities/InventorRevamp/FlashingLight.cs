using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashingLight : MonoBehaviour
{

    Light light;
    // Start is called before the first frame update
    void Start()
    {
        light = GetComponentInChildren<Light>();
        Invoke(nameof(TurnOnLight), .45f);
        Invoke(nameof(TurnOffLight), .55f);
        Invoke(nameof(TurnOnLight), .95f);
        Invoke(nameof(TurnOffLight), 1.05f);
    }

    // Update is called once per frame
    void Update()
    {
         
    }

    void TurnOffLight()
    {
        light.intensity = 0f;
    }

    void TurnOnLight()
    {
        light.intensity = 100f;
    }
}
