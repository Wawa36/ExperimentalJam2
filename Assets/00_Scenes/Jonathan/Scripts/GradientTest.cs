using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradientTest : MonoBehaviour
{
    public LineRenderer grad;
    public Color col;

    // Update is called once per frame
    void Update()
    {
        var c = grad.colorGradient.colorKeys;
        c[0].color = col;
        grad.colorGradient.colorKeys = c;
        grad.startColor = col;
    }
}
