using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkPlayer : NetworkBehaviour {

    [SerializeField] private GameObject lobbyPlayer;
    [SerializeField] private GameObject localPlayer;

    [SerializeField] [SyncVar] private string playerName;
    [SerializeField] [SyncVar] private string playerHealth;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        if (!isLocalPlayer) return;
        DontDestroyOnLoad(this.gameObject);
        Debug.Log("Network Player: Start() + isLocalPlayer " + isLocalPlayer);        
        CmdSpawnLobbyPlayer();
        NetworkController.singleton.onServerSceneChange += OnSceneLoaded;
    }

    [Command]
    private void CmdSpawnLobbyPlayer()
    {        
        GameObject _go = Instantiate(lobbyPlayer, transform.position, transform.rotation);
        _go.name = "Player_" + connectionToClient.connectionId;
        NetworkServer.SpawnWithClientAuthority(_go, connectionToClient);        
    }

    [Command]
    private void CmdSpawnLobbyPlayer_Graphics(GameObject go)
    {
        NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
    }

    [Command]
    private void CmdSpawnLocalPlayer()
    {
        GameObject _localPlayer = Instantiate(localPlayer, transform.position, Quaternion.identity);
        _localPlayer.gameObject.name = "Robot_" + connectionToClient.connectionId;
        NetworkServer.SpawnWithClientAuthority(_localPlayer, connectionToClient);
    }

    [Command] 
    public void CmdDestroyLobbyPlayer(GameObject go)
    {
        NetworkServer.Destroy(go);
    }

    private void OnSceneLoaded(string sceneName)
    {
        if (sceneName == "Game")
        {
            //CmdSpawnLocalPlayer();
            Debug.Log("Scene Loaded " + sceneName);
        }
    }

    private void OnDestroy()
    {
        NetworkController.singleton.onServerSceneChange -= OnSceneLoaded;
    }
}
