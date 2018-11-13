using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pickup : MonoBehaviourPunCallbacks {

    public enum Type { Ability, Overload }
    private Type type = Type.Ability;

    private GameObject prefab;
    private NetworkGameRobot collidedRobot;

    private bool pickedUp = false;

    public void Setup (GameObject _prefab, Type _type)
    {
        this.prefab = _prefab;
        this.type = _type;

        if (_type == Type.Overload)
            GetComponentInChildren<Text> ().text = _prefab.GetComponent<Overload> ().OverloadName.ToUpper ();
        else GetComponentInChildren<Text> ().text = _prefab.GetComponent<Ability> ().AbilityName.ToUpper ();
    }

    private void OnTriggerEnter (Collider other)
    {
        NetworkGameRobot robot = other.GetComponentInParent<NetworkGameRobot> ();
        if (robot == null) return;

        PhotonView view = robot.GetComponent<PhotonView> ();
        if (view == null) return;

        if (view.Owner.IsLocal)
        {            
            photonView.RPC ( "RPCPickupMaster", RpcTarget.MasterClient, view.ViewID );            
        }        
    }

    [PunRPC]
    private void RPCPickupMaster (int viewID)
    {
        if (pickedUp) return;

        pickedUp = true;
        photonView.RPC ( "RPCPickup", RpcTarget.All, viewID );
    }

    [PunRPC]
    private void RPCFailedPickUp ()
    {
        pickedUp = false;
    }

    [PunRPC] 
    private void RPCPickup(int viewID)
    {
        PhotonView view = PhotonView.Find ( viewID );
        if (view.Owner.IsLocal)
        {
            // Pickup logic
            if (type == Type.Overload)
            {
                bool result = view.GetComponent<RobotOverloads> ().AddAbility ( prefab );

                if (!result)
                {
                    photonView.RPC ( "RPCFailedPickUp", RpcTarget.MasterClient );
                    return;
                }
            }
            else
            {
                view.GetComponent<RobotAbilities> ().AddAbility ( prefab );
            }

            if (type == Type.Overload)
                KillFeed.Instance.AddInfo ( view.Owner.NickName + " picked up an Overload!", KillFeed.InfoType.Overload, RpcTarget.All );
            else KillFeed.Instance.AddInfo ( view.Owner.NickName + " picked up an Ability!", KillFeed.InfoType.Ability, RpcTarget.All );

            photonView.RPC ( "RPCDestroy", RpcTarget.MasterClient );            
        }
    }

    [PunRPC]
    private void RPCDestroy ()
    {
        PhotonNetwork.Destroy ( this.gameObject );
    }
}
