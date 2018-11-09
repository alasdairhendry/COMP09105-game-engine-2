using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviourPunCallbacks
{
    protected RobotWeaponData data;
    protected Animator animator;
    protected float currentResourceLeft;
    protected bool isAttacking;

    protected virtual void Start ()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected) return;        
        data = MyRobot.singleton.GetMyRobotData.WeaponData;
        animator = GetComponent<Animator> ();
        currentResourceLeft = data.baseResourceMax;
    }

    protected virtual void Update ()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected) return;
        Attack ();
    }

    protected virtual void Attack()
    {
        //Debug.Log ( "Attacking" );
    }

    protected virtual void Animate ()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected) return;
    }

    public virtual void OnChildCollisionEnter (Collider collision)
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected) return;
    }

    public virtual void OnChildCollisionStay (Collider collision)
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected) return;
    }
}
