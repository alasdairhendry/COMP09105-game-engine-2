using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_Timed : Obstacle
{
    protected override void Start ()
    {
        
    }

    protected override void Update ()
    {
        CheckDelay ();
    }

    protected virtual void CheckDelay ()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            currentDelay += Time.deltaTime;

            if (currentDelay >= delay)
            {
                if (currentCycle < cycles || cycles <= 0)
                {
                    base.ActivateNetwork ();
                    currentDelay = 0.0f;
                    currentCycle++;
                }
            }
        }
    }
}
