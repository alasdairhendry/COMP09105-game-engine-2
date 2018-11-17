using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkGameRobot : MonoBehaviourPunCallbacks {

    // Use this for initialization
    void Start()
    {
        SetDisplayName();
        RegisterSpotlight(true);
        KillFeed.Instance.AddInfo(photonView.Owner.NickName + " has joined the game.", KillFeed.InfoType.Joined);

        gameObject.name = "NetworkGameRobot_" + photonView.Owner.NickName;

        if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

        this.gameObject.tag = "LocalGamePlayer";
        SpawnGraphics();
        SetCamera();
    }

    private void SetDisplayName()
    {
        if (PhotonNetwork.OfflineMode) return;
        
        Text text = GetComponentInChildren<Text> ();
        if (photonView.Owner == null) return;
        text.text = photonView.Owner.NickName;        
    }

    private void RegisterSpotlight(bool add)
    {
        if (add)
        {
            FindObjectOfType<SpotlightController>().Add(this);
        }
        else
        {
            if (FindObjectOfType<SpotlightController>() != null)
                FindObjectOfType<SpotlightController>().Remove(this);
        }
    }

    private void SpawnGraphics()
    {
        MyRobotData myData = MyRobot.Instance.GetMyRobotData;        

        GameObject body = PhotonNetwork.Instantiate(myData.BodyData.prefab.name, transform.position, transform.rotation, 0);

        GetComponent<ReplayInvoker>().SetReplayable(body.GetComponent<Replayable>());

        GameObject weapon = PhotonNetwork.Instantiate(myData.WeaponData.prefab.name, transform.position, transform.rotation, 0);

        GameObject emblemSpring = PhotonNetwork.Instantiate ( "EmblemSpring_Prefab", transform.position, Quaternion.identity, 0 );
        GameObject emblem = PhotonNetwork.Instantiate ( myData.EmblemData.prefab.name, transform.position, Quaternion.identity, 0 );

        int skinIndex = MyRobot.Instance.SkinDatas.IndexOf(MyRobot.Instance.GetMyRobotData.SkinData);

        photonView.RPC("RpcSetupGraphics", RpcTarget.AllBuffered, body.GetPhotonView().ViewID, weapon.GetPhotonView ().ViewID, emblemSpring.GetPhotonView ().ViewID, emblem.GetPhotonView ().ViewID, myData.WeaponMountPosition, myData.WeaponMountRotation, myData.BodyData.mass, skinIndex);
    }

    [PunRPC]
    private void RpcSetupGraphics(int bodyID, int weaponID, int springID, int emblemID, Vector3 weaponMountPosition, Vector3 weaponMountRotation, float mass, int skinIndex)
    {        
        GameObject body = PhotonView.Find(bodyID).gameObject;
        GameObject weapon = PhotonView.Find(weaponID).gameObject;

        GameObject emblemSpring = PhotonView.Find ( springID ).gameObject;
        GameObject emblem = PhotonView.Find ( emblemID ).gameObject;

        body.transform.SetParent(transform.Find("Graphics"));
        body.transform.localPosition = Vector3.zero;
        body.transform.localEulerAngles = Vector3.zero;
        body.name = "Body";

        weapon.transform.SetParent(transform.Find("Graphics"));
        weapon.transform.localPosition = weaponMountPosition;
        weapon.transform.localEulerAngles = weaponMountRotation;
        weapon.name = "Weapon";

        emblemSpring.transform.SetParent ( body.GetComponentInChildren<EmblemMount>().transform );
        emblemSpring.transform.localPosition = Vector3.zero;
        emblemSpring.transform.localEulerAngles = Vector3.zero;        

        emblem.transform.SetParent ( emblemSpring.transform.Find ( "Root" ).Find ( "Mount" ) );
        emblem.transform.localPosition = Vector3.zero;
        emblem.transform.localEulerAngles = Vector3.zero;

        body.GetComponent<MeshRenderer>().material.SetTexture("_Albedo", MyRobot.Instance.SkinDatas[skinIndex].texture);

        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody> ().mass = mass;

        Heatable heatable = GetComponent<Heatable> ();
        heatable.AddRenderers ( body.GetComponent<MeshRenderer> () );
        heatable.AddRenderers ( weapon.GetComponentsInChildren<MeshRenderer> ());

        if (PhotonView.Find(bodyID).Owner.IsLocal)
        {
            GameObject go = GameObject.FindGameObjectWithTag("LocalNetworkPlayer");

            if (go != null)
                go.GetComponent<NetworkPlayer>().SetGameReady();
        }
    }

    private void SetCamera()
    {
        if (PhotonNetwork.OfflineMode) return;
        GameObject.FindObjectOfType<Test_SmoothCamera>().SetTarget(this.transform);
    }

    private void OnDestroy()
    {
        RegisterSpotlight(false);
    }
}
