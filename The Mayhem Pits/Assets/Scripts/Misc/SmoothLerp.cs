using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SmoothLerp {

	public static float Lerp(float value, float target, float damp)
    {
        if (value < target)
            value += damp;
        else if (value > target)
            value -= damp;

        return value;
    }
}
