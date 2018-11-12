using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Move : MonoBehaviour {

    Rigidbody rb;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        Movement ();
        Rotation ();
    }

    private void Movement ()
    {
        if (rb.velocity.magnitude >= 12) return;

        Vector3 force = (transform.forward * Input.GetAxis ( "XBO_RT" ) * 1250 * Time.fixedDeltaTime);
        rb.AddForceAtPosition ( force, transform.position, ForceMode.Acceleration );
    }

    private void Rotation ()
    {
        Vector3 newRotation = transform.localEulerAngles + (new Vector3 ( 0.0f, 150 * Input.GetAxis ( "Horizontal" ) * Time.fixedDeltaTime ));
        rb.MoveRotation ( Quaternion.Euler ( newRotation ) );
    }
}
