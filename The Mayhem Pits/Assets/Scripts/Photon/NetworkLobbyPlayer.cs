using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NetworkLobbyPlayer : MonoBehaviourPunCallbacks {

	// Use this for initialization
	void Start () {
        if (PhotonNetwork.MasterClient.ActorNumber == photonView.Owner.ActorNumber)
        {
            GetComponentInChildren<TextMesh>().text = "<size=30>Host</size>\n" + photonView.Owner.NickName;
        }
        else
        {
            GetComponentInChildren<TextMesh>().text = photonView.Owner.NickName;
        }

        gameObject.name = "NetworkLobbyPlayer_" + photonView.Owner.NickName;

        if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

        NetworkManager.singleton.onPlayerListChanged += OnPlayerListChanged;
        SpawnLobbyGraphics();
	}

    public override void OnMasterClientSwitched (Player newMasterClient)
    {
        if (this == null) return;
        if (this.photonView == null) return;
        if (this.photonView.Owner == null) return;
        if (newMasterClient.ActorNumber == photonView.Owner.ActorNumber)
        {
            GetComponentInChildren<TextMesh> ().text = "<size=30>Host</size>\n" + photonView.Owner.NickName;
        }
        else
        {
            GetComponentInChildren<TextMesh> ().text = photonView.Owner.NickName;
        }
    }

    private void SpawnLobbyGraphics()
    {
        MyRobotData myData = MyRobot.Instance.GetMyRobotData;

        GameObject body = PhotonNetwork.Instantiate(myData.BodyData.prefab.name, transform.position, transform.rotation, 0);
        GameObject weapon = PhotonNetwork.Instantiate(myData.WeaponData.prefab.name, transform.position, transform.rotation, 0);

        GameObject emblemSpring = PhotonNetwork.Instantiate("EmblemSpring_Prefab", transform.position, Quaternion.identity, 0);
        GameObject emblem = PhotonNetwork.Instantiate(myData.EmblemData.prefab.name, transform.position, Quaternion.identity, 0);

        int skinIndex = MyRobot.Instance.SkinDatas.IndexOf(MyRobot.Instance.GetMyRobotData.SkinData);

        photonView.RPC("SetupLobbyGraphics", RpcTarget.AllBuffered, body.GetPhotonView().ViewID, weapon.GetPhotonView().ViewID, emblemSpring.GetPhotonView().ViewID, emblem.GetPhotonView().ViewID, myData.WeaponMountPosition, myData.WeaponMountRotation, skinIndex);
    }	

    [PunRPC] private void SetupLobbyGraphics(int bodyID, int weaponID, int springID, int emblemID, Vector3 weaponMountPosition, Vector3 weaponMountRotation, int skinIndex)
    {
        GameObject body = PhotonView.Find(bodyID).gameObject;
        GameObject weapon = PhotonView.Find(weaponID).gameObject;

        GameObject emblemSpring = PhotonView.Find(springID).gameObject;
        GameObject emblem = PhotonView.Find(emblemID).gameObject;

        body.transform.SetParent(transform.Find("Graphics"));
        body.transform.localPosition = Vector3.zero;
        body.transform.localEulerAngles = Vector3.zero;
        body.name = "Body";
        
        weapon.transform.SetParent(transform.Find("Graphics"));
        weapon.transform.localPosition = weaponMountPosition;
        weapon.transform.localEulerAngles = weaponMountRotation;
        weapon.name = "Weapon";

        emblemSpring.transform.SetParent(body.GetComponentInChildren<EmblemMount>().transform);
        emblemSpring.transform.localPosition = Vector3.zero;
        emblemSpring.transform.localEulerAngles = Vector3.zero;

        emblem.transform.SetParent(emblemSpring.transform.Find("Root").Find("Mount"));
        emblem.transform.localPosition = Vector3.zero;
        emblem.transform.localEulerAngles = Vector3.zero;

        body.GetComponent<MeshRenderer>().material.SetTexture( "_MainTex", MyRobot.Instance.SkinDatas[skinIndex].texture);
    }

    private void OnPlayerListChanged(Player[] _playerList)
    {
        playerList = _playerList;
        ResetPositionToSpawn();
    }

    private void ResetPositionToSpawn()
    {
        NetworkSpawnPoint[] spawnPoints = GameObject.FindObjectsOfType<NetworkSpawnPoint>();
        NetworkSpawnPoint mySpawn = GetSpawnPoint(spawnPoints);

        transform.position = mySpawn.transform.position;
        transform.rotation = mySpawn.transform.rotation;
    }

    private NetworkSpawnPoint GetSpawnPoint(NetworkSpawnPoint[] spawnPoints)
    {
        NetworkSpawnPoint mySpawn = spawnPoints[0];
        int indexInPlayerList = 0;

        for (int i = 0; i < playerList.Length; i++)
        {
            if (playerList[i].ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                indexInPlayerList = i;
                break;
            }
        }

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i].index == indexInPlayerList)
            {
                mySpawn = spawnPoints[i];
                break;
            }
        }

        return mySpawn;
    }

    //public override void OnMasterClientSwitched(Player newMasterClient)
    //{
    //    if(newMasterClient.ActorNumber == photonView.Owner.ActorNumber)
    //    {
    //        GetComponentInChildren<TextMesh>().text = "<size=30>Host</size>\n" + photonView.Owner.NickName;
    //    }
    //}

    private void OnDestroy()
    {
        NetworkManager.singleton.onPlayerListChanged -= OnPlayerListChanged;
    }

    Player[] playerList;
    private void OnGUI()
    {
        if (playerList == null) return;
        for (int i = 0; i < playerList.Length; i++)
        {
            GUI.Label(new Rect(16, (64 + (i * 32)), 200, 32), playerList[i].NickName + " - PlayerID: " + playerList[i].ActorNumber);
        }
    }
}
