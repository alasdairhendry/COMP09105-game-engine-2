using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviourPunCallbacks {

    [SerializeField] private float lifetime = 5.0f;
    [SerializeField] private bool network = false;
    private float currLifetime = 0.0f;   

    private void Update ()
    {
        if (network)
        {
            if (!photonView.IsMine && PhotonNetwork.IsConnected)
                return;
        }

        if (network)
        {
            currLifetime += Time.deltaTime;
            if (currLifetime >= lifetime) { PhotonNetwork.Destroy ( this.gameObject ); }
        }
        else
        {
            currLifetime += Time.deltaTime;
            if (currLifetime >= lifetime) Destroy ( this.gameObject );
        }
        
    }

    public void SetLifetime (float length)
    {
        lifetime = length;
    }
}
