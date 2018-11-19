using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraIntersectCuller : MonoBehaviour {

    private void Trigger (CameraIntersectCullable c, bool show)
    {
        if (show) c.Show ();
        else c.Hide ();     
    }

    private void OnTriggerEnter (Collider other)
    {     
        CameraIntersectCullable c = other.GetComponent<CameraIntersectCullable> ();
        if(c != null) { Trigger ( c, false ); }

        c = other.GetComponentInParent<CameraIntersectCullable> ();
        if (c != null) { Trigger ( c, false ); }

        c = other.GetComponentInChildren<CameraIntersectCullable> ();
        if (c != null) { Trigger ( c, false ); }
    }

    private void OnTriggerExit (Collider other)
    {        
        CameraIntersectCullable c = other.GetComponent<CameraIntersectCullable> ();
        if (c != null) { Trigger ( c, true ); }

        c = other.GetComponentInParent<CameraIntersectCullable> ();
        if (c != null) { Trigger ( c, true ); }

        c = other.GetComponentInChildren<CameraIntersectCullable> ();
        if (c != null) { Trigger ( c, true ); }
    }
}
