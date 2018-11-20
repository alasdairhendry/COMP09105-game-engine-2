using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_RobotMovement : MonoBehaviourPunCallbacks {

    [SerializeField] private List<WheelConnection> wheelConnections = new List<WheelConnection> ();
    MyRobotData data;
    Rigidbody rb;
    private int currWheelConnections = 0;

    [System.Serializable]
    class WheelConnection
    {
        public GameObject wheel;
        public bool connection;
    }

	// Use this for initialization
	void Start () {
        if (!photonView.IsMine && PhotonNetwork.IsConnected) return;
        rb = GetComponent<Rigidbody>();
        data = MyRobot.Instance.GetMyRobotData;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!photonView.IsMine && PhotonNetwork.IsConnected) return;
        if(wheelConnections.Count <= 0) { FindWheels (); }
        Movement();
        Rotate();

	}

    private void Update ()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected) return;
        CheckWheels ();
        GetComponent<RobotSound> ().SetAudio (Input.GetAxis("XBO_RT"), currWheelConnections );
    }

    private void FindWheels ()
    {
        Transform wheelsT = transform.Find ( "Graphics" ).Find ( "Body" ).Find ( "Wheels" );
        for (int i = 0; i < wheelsT.childCount; i++)
        {
            wheelConnections.Add ( new WheelConnection () { wheel = wheelsT.GetChild ( i ).gameObject, connection = true } );
        }
    }

    private void Movement()
    {
        // Old - before added wheel mechanics
        //if (rb.velocity.magnitude <= data.BodyData.maxSpeed)
        //{
        //    rb.AddForce ( transform.forward * Input.GetAxis ( "XBO_LT" ) * data.BodyData.acceleration * Time.fixedDeltaTime, ForceMode.Acceleration );
        //}

        if (rb.velocity.magnitude >= data.BodyData.maxSpeed) return;

        for (int i = 0; i < wheelConnections.Count; i++)
        {
            if (wheelConnections[i].connection)
            {
                Vector3 force = (transform.forward * Input.GetAxis ( "XBO_RT" ) * data.BodyData.acceleration * Time.fixedDeltaTime) / wheelConnections.Count;
                rb.AddForceAtPosition ( force, wheelConnections[i].wheel.transform.position, ForceMode.Acceleration );
            }
        }        
    }

    private void Rotate()
    {
        Vector3 newRotation = transform.localEulerAngles + (new Vector3(0.0f, data.BodyData.rotationalSpeed * Input.GetAxis("Horizontal") * Time.fixedDeltaTime));
        rb.MoveRotation(Quaternion.Euler(newRotation));
    }

    private void CheckWheels ()
    {
        int _currConnections = 0;
        for (int i = 0; i < wheelConnections.Count; i++)
        {
            Ray ray = new Ray ( wheelConnections[i].wheel.transform.position + new Vector3(0.0f, 0.05f, 0.0f), -transform.up );
            RaycastHit hit;

            Debug.DrawRay ( wheelConnections[i].wheel.transform.position + new Vector3 ( 0.0f, 0.05f, 0.0f ), -transform.up * 0.1f, Color.red);

            if(Physics.Raycast(ray, out hit, 0.1f ))
            {
                wheelConnections[i].connection = true;
                _currConnections++;
            }
            else
            {
                wheelConnections[i].connection = false;
            }
        }

        currWheelConnections = _currConnections;
    }
}
