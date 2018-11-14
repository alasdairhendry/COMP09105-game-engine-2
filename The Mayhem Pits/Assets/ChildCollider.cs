using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildCollider : MonoBehaviour {

    public Action<Collider> triggerEnter;
    public Action<Collider> triggerStay;
    public Action<Collider> triggerExit;

    private void OnTriggerEnter (Collider other)
    {
        if (triggerEnter != null)
            triggerEnter (other);             
    }

    private void OnTriggerStay (Collider other)
    {
        if (triggerStay != null)
            triggerStay ( other );
    }

    private void OnTriggerExit (Collider other)
    {
        if (triggerExit != null)
            triggerExit ( other );
    }
}
