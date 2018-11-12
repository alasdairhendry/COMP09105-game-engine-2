using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour {

    [SerializeField] private float lifetime = 5.0f;
    private float currLifetime = 0.0f;

    private void Update ()
    {
        currLifetime += Time.deltaTime;
        if (currLifetime >= lifetime) Destroy ( this.gameObject );
    }

    internal void SetLifetime (float length)
    {
        lifetime = length;
    }
}
