using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmblemSpring : MonoBehaviour {

    [SerializeField] private int sections;
    [SerializeField] private AnimationCurve curve;
    private LineRenderer lr;
    private Transform mount;

	// Use this for initialization
	void Start () {
        lr = GetComponent<LineRenderer> ();
        mount = transform.Find ( "Root" ).Find ( "Mount" );
        lr.positionCount = sections;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        //GetComponent<Rigidbody> ().AddForce ( -GetComponentInParent<Rigidbody> ().velocity );
        GetComponent<LineRenderer> ().SetPosition ( 0, transform.position );
        GetComponent<LineRenderer> ().SetPosition ( 1, transform.Find ( "Root" ).Find ( "Mount" ).transform.position );
        SetPositions ();
    }

    private void SetPositions ()
    {
        
        for (int i = 0; i < sections; i++)
        {            
            Vector3 position = Vector3.Lerp ( transform.position, mount.position, (float)i / (float)sections );
            lr.SetPosition ( i, position );
        }
    }
}
