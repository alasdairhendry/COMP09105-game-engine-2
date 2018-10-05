using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NetworkController : NetworkManager {

    public static new NetworkController singleton;

    public Action<string> onServerSceneChange;

    private void Awake()
    {
        if (singleton == null)
            singleton = this;
        else if (singleton != this)
            Destroy(gameObject);

        //SceneManager.sceneLoaded += OnSceneLoaded;
    }    

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);

        Debug.Log("A new player has connected");
        Debug.Log(conn);        
    }

    public override void ServerChangeScene(string newSceneName)
    {
        base.ServerChangeScene(newSceneName);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
        if (onServerSceneChange != null)
            onServerSceneChange(sceneName);

        Debug.Log("Scene change");
    }

    //private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    if (scene.name == "Game")
    //    {
    //        CmdSendReadyStatus();
    //    }
    //}

    //[Command]
    //private void CmdSendReadyStatus()
    //{
    //    sceneReadyStatus++;
    //    if (sceneReadyStatus >= NetworkController.singleton.numPlayers)
    //    {
    //        RpcSpawnRobots();
    //    }
    //}

    //[ClientRpc]
    //private void RpcSpawnRobots()
    //{
    //    CmdSpawnRobot();
    //}

    //[Command]
    //private void CmdSpawnRobot()
    //{
    //    GameObject go = Instantiate(robotRootPrefab);
    //    go.transform.position = Vector3.zero;
    //    go.transform.eulerAngles = Vector3.zero;
    //    go.transform.name = "Body";
    //    NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
    //}


    //private void OnDestroy()
    //{
    //    SceneManager.sceneLoaded -= OnSceneLoaded;
    //}
}
