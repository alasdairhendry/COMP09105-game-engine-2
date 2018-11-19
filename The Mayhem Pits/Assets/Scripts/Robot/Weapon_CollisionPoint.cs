using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_CollisionPoint : MonoBehaviour {

    Weapon weapon;

    private void Start ()
    {
        weapon = GetComponentInParent<Weapon> ();
    }

    private void OnTriggerEnter (Collider other)
    {
        weapon.OnChildCollisionEnter ( other );
    }

    private void OnTriggerStay (Collider other)
    {
        weapon.OnChildCollisionStay ( other );
    }

    private void OnTriggerExit(Collider other)
    {
        weapon.OnChildCollisionExit(other);
    }

    //private void OnCollisionEnter (Collision collision)
    //{
    //    weapon.OnChildCollisionEnter ( collision );
    //}

    //private void OnCollisionStay (Collision collision)
    //{
    //    weapon.OnChildCollisionStay ( collision );
    //}
}
