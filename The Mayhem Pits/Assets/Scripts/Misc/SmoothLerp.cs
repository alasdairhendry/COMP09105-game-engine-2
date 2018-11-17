using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SmoothLerp {

    public static void Instantiate(string path, Vector3 position, string layer = "Default")
    {
        GameObject resource = Resources.Load<GameObject>(path);
        if (resource == null) { Debug.LogError("Cant Instantiate as the gameobject does not exist - " + path); }
        if (string.IsNullOrEmpty(layer)) layer = "Default";

        GameObject go = GameObject.Instantiate(resource);

        go.transform.position = position;
        go.layer = LayerMask.NameToLayer(layer);
    }

	public static float Lerp(float value, float target, float damp)
    {
        if (value < target)
            value += damp;
        else if (value > target)
            value -= damp;

        return value;
    }

    public static Vector3 Lerp3(Vector3 value, Vector3 target, float damp, bool clamp = false)
    {
        float x = value.x;
        float y = value.y;
        float z = value.z;

        x = Lerp(x, target.x, damp);
        y = Lerp(y, target.y, damp);
        z = Lerp(z, target.z, damp);

        if (!clamp) return new Vector3(x, y, z);

        x = Mathf.Clamp(x, Mathf.Min(value.x, target.x), Mathf.Max(value.x, target.x));
        y = Mathf.Clamp(y, Mathf.Min(value.y, target.y), Mathf.Max(value.y, target.y));
        z = Mathf.Clamp(z, Mathf.Min(value.z, target.z), Mathf.Max(value.z, target.z));

        return new Vector3(x, y, z);
    }

    public static string GetIndexString()
    {
        return "";
    }

}
