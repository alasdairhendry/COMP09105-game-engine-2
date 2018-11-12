using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_Trigger : Obstacle {

    protected bool hasTriggered = false;
    protected Collider targetCollider;

    protected override void Update ()
    {
        CheckDelay ();
    }

    protected virtual void CheckDelay ()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (!hasTriggered) return;

            currentDelay += Time.deltaTime;

            if(currentDelay>= delay)
            {
                if (currentCycle < cycles || cycles <= 0)
                {                    
                    currentDelay = 0.0f;
                    currentCycle++;
                    hasTriggered = false;
                }
            }
        }
    }

    protected override void ActivateNetwork ()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (!hasTriggered)
            {
                base.ActivateNetwork ();
                hasTriggered = true;
            }
        }
    }

    protected virtual void OnTriggerEnter (Collider other)
    {
        Debug.Log ( other.gameObject.name );
        targetCollider = other;
        ActivateNetwork ();
    }

}
