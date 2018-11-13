using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_Boost : Ability {

    [SerializeField] private float boostForce;

    // The actual logic for the ability will go here
    protected override void OnActivate ()
    {
        base.OnActivate ();

        targetRobot.GetComponent<Rigidbody> ().AddForce ( targetRobot.transform.forward * boostForce * Time.fixedDeltaTime, ForceMode.Impulse );       

        Finish ();
    }
}
