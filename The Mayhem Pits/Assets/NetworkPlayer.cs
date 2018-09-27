using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkPlayer : NetworkBehaviour {

    [SerializeField] private GameObject localPlayer;

    [SerializeField] [SyncVar] private string playerName;
    [SerializeField] [SyncVar] private string playerHealth;

    private void Start()
    {
        if (!isLocalPlayer) return;
        CmdSpawnLocalPlayer();
    }

    [Command]
    private void CmdSpawnLocalPlayer()
    {
        GameObject _localPlayer = Instantiate(localPlayer, transform.position, Quaternion.identity);
        NetworkServer.SpawnWithClientAuthority(_localPlayer, connectionToClient);
    }

    //private void Start()
    //{
    //    GetComponentInChildren<TextMesh>().text = playerName;

    //    if (!isLocalPlayer) return;
    //    RandomiseName();       
    //}

    //private void Update()
    //{
    //    if (!isLocalPlayer) return;
    //    if (Input.GetKeyDown(KeyCode.R)) RandomiseName();
    //}

    //void RandomiseName()
    //{
    //    gameObject.name = "Player-" + Random.Range(101, 1000);
    //    CmdUpdateName(gameObject.name);
    //}

    //[Command]
    //void CmdUpdateName(string name)
    //{
    //    playerName = name;
    //    RpcUpdateName(name);
    //}

    //[ClientRpc]
    //void RpcUpdateName(string name)
    //{
    //    playerName = name;
    //    GetComponentInChildren<TextMesh>().text = playerName;
    //}

    ////void Hook_OnNameChanged(string name)
    ////{
    ////    playerName = name;
    ////    
    ////}
}
