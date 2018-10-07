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

        if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

        NetworkManager.singleton.onPlayerListChanged += OnPlayerListChanged;
        SpawnLobbyGraphics();
	}

    private void SpawnLobbyGraphics()
    {
        MyRobotData myData = MyRobot.singleton.GetMyRobotData;

        GameObject body = PhotonNetwork.Instantiate(myData.BodyPrefab.name, transform.position, transform.rotation, 0);
        body.transform.SetParent(transform.Find("Graphics"));
        body.transform.localPosition = Vector3.zero;
        body.transform.localEulerAngles = Vector3.zero;
        body.name = "Body";

        GameObject weapon = PhotonNetwork.Instantiate(myData.WeaponPrefab.name, transform.position, transform.rotation, 0);
        weapon.transform.SetParent(transform.Find("Graphics"));
        weapon.transform.localPosition = myData.WeaponMountPosition;
        weapon.transform.localEulerAngles = myData.WeaponMountRotation;
        weapon.name = "Weapon";

        photonView.RPC("SetupLobbyGraphics", RpcTarget.OthersBuffered, body.GetPhotonView().ViewID, weapon.GetPhotonView().ViewID, myData.WeaponMountPosition, myData.WeaponMountRotation);
    }	

    [PunRPC] private void SetupLobbyGraphics(int bodyID, int weaponID, Vector3 weaponMountPosition, Vector3 weaponMountRotation)
    {
        GameObject body = PhotonView.Find(bodyID).gameObject;
        body.transform.SetParent(transform.Find("Graphics"));
        body.transform.localPosition = Vector3.zero;
        body.transform.localEulerAngles = Vector3.zero;
        body.name = "Body";

        GameObject weapon = PhotonView.Find(weaponID).gameObject;
        weapon.transform.SetParent(transform.Find("Graphics"));
        weapon.transform.localPosition = weaponMountPosition;
        weapon.transform.localEulerAngles = weaponMountRotation;
        weapon.name = "Weapon";
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
