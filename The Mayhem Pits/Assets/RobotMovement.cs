using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class RobotMovement : NetworkBehaviour {

    [SerializeField] private float movementSpeed;

    private Rigidbody rb;
    private Vector3 movementVector = new Vector3();

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    private void Update () {
        if (!hasAuthority) return;
        MovementInput();
	}

    private void MovementInput()
    {
        Vector3 forwardMovement = Input.GetAxis("Vertical") * transform.forward;
        Vector3 sidewaysMovement = Input.GetAxis("Horizontal") * transform.right;

        movementVector = (forwardMovement + sidewaysMovement).normalized;
    }

    private void FixedUpdate()
    {
        if (!hasAuthority) return;
        Movement();
        Rotation();
    }

    private void Movement()
    {
        rb.MovePosition(transform.position + (movementVector * movementSpeed * Time.fixedDeltaTime));
    }

    private void Rotation()
    {

    }
}
