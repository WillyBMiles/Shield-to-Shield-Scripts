using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateYFade : MonoBehaviour
{
    public bool Animate = true;

    float startingHeight;
    float startingFade;
    Renderer r;

    // Start is called before the first frame update
    void Start()
    {
        r = GetComponent<Renderer>();
        startingHeight = transform.position.y;
        startingFade = r.material.color.a;
    }

    // Update is called once per frame
    void Update()
    {
        if (Animate)
        {
            Color c = r.material.color;
            float fade = Mathf.Lerp(1f, startingFade, transform.position.y / startingHeight);
            r.material.color = new Color(c.r, c.g, c.b, fade);
        }
    }
}
