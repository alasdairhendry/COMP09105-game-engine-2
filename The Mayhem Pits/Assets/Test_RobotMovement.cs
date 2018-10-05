using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_RobotMovement : MonoBehaviour {

    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;
    Rigidbody rb;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        Movement();
        Rotate();
	}

    private void Movement()
    {
        rb.AddForce(transform.forward * Input.GetAxis("Vertical") * moveSpeed * Time.fixedDeltaTime);
    }

    private void Rotate()
    {
        Vector3 newRotation = transform.localEulerAngles + (new Vector3(0.0f, rotateSpeed * Input.GetAxis("Horizontal") * Time.fixedDeltaTime));
        rb.MoveRotation(Quaternion.Euler(newRotation));
    }


}
