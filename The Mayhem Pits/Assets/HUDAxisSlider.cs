using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDAxisSlider : MonoBehaviour {

    public float minValue = 0.0f;
    public float maxValue = 1.0f;
    public float speed = 0.1f;

    public bool autoScroll = true;
    private bool hasScrolled = false;

    public System.Action<float> slide;

    public void Slide(float direction)
    {
        if (direction == 0.0f) { hasScrolled = false; return; }

        if (!autoScroll && hasScrolled) return;

        if (slide != null) slide(direction);

        if (direction != 0.0f) { hasScrolled = true; }
    }
}
