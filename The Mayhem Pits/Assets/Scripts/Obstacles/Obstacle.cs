using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class Obstacle : MonoBehaviourPunCallbacks {

    [SerializeField] protected float delay;
    [SerializeField] protected int cycles;

    protected float currentDelay = 0;
    protected int currentCycle = 0;

    protected virtual void Start ()
    {

    }

    protected virtual void Update ()
    {

    }

	public virtual void Activate ()
    {

    }

    protected virtual void ActivateNetwork ()
    {
        photonView.RPC ( "RPCActivateNetwork", RpcTarget.All, null );
    }

    [PunRPC]
    protected virtual void RPCActivateNetwork ()
    {
        Activate ();
    }

}
