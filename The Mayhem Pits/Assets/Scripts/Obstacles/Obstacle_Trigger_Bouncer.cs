using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_Trigger_Bouncer : Obstacle_Trigger {

    [SerializeField] private Rigidbody bouncerRigidbody;

    public override void Activate ()
    {
        GetComponentInChildren<Animator> ().SetTrigger ( "Bounce" );
        Debug.Log ( "boooo" );
    }

}
