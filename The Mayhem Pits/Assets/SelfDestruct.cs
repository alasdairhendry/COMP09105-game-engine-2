using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour {

    [SerializeField] private float lifetime;
    private float currLifetime = 0.0f;

    private void Update ()
    {
        currLifetime += Time.deltaTime;
        if (currLifetime >= lifetime) Destroy ( this.gameObject );
    }
}
