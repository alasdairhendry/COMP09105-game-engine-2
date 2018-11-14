using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spotlight : MonoBehaviour {

    private Transform target;
    private bool active = false;
	
	private void Update () {
        if (target == null) { DisableLight(); }
        else { EnableLight(); Track(); }
	}

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    private void Track()
    {
        //transform.LookAt(target);
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion look = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * 7.5f);
    }

    private void EnableLight()
    {
        if (active) return;
        active = true;
        GetComponentInChildren<Light>().enabled = true;
    }

    private void DisableLight()
    {
        if (!active) return;
        active = false;
        GetComponentInChildren<Light>().enabled = false;
    }
}
