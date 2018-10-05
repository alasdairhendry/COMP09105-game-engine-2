using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkLobbyPlayer : NetworkBehaviour {       

    [SerializeField] private List<GameObject> robotBodies = new List<GameObject>();
    public NetworkPlayer networkPlayer;

    private int currentCountdown = 2;

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        if (!hasAuthority) return;
        DontDestroyOnLoad(this.gameObject);

        Debug.Log("Network Lobby Player: Start() + hasAuthority " + hasAuthority);

        FindNetworkPlayer();                
        CmdSpawnLobbyGraphics(networkPlayer.GetComponent<NetworkIdentity>());

        if (isServer)
        {
            StartCoroutine(Countdown());
        }
    }

    private void FindNetworkPlayer()
    {
        NetworkPlayer[] players = GameObject.FindObjectsOfType<NetworkPlayer>();

        foreach (NetworkPlayer player in players)
        {
            if (player.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                networkPlayer = player;
                //Debug.Log("true");
            }
        }
    }

    [Command]
    private void CmdSpawnLobbyGraphics(NetworkIdentity _conn)
    {        
        MyRobotData myData = MyRobot.singleton.GetMyRobotData;
        GameObject bodyGraphics = Instantiate(myData.BodyPrefab, this.transform.Find("Graphics"));
        bodyGraphics.transform.localPosition = Vector3.zero;
        bodyGraphics.transform.localEulerAngles = Vector3.zero;
        bodyGraphics.transform.name = "Body";
        NetworkServer.SpawnWithClientAuthority(bodyGraphics, _conn.connectionToClient);

        GameObject weaponGraphics = Instantiate(myData.WeaponPrefab, this.transform.Find("Graphics"));
        weaponGraphics.transform.localPosition = myData.WeaponMountPosition;
        weaponGraphics.transform.localEulerAngles = myData.WeaponMountRotation;
        NetworkServer.SpawnWithClientAuthority(weaponGraphics, _conn.connectionToClient);
    }

    private IEnumerator Countdown()
    {
        while (true)
        {
            if (NetworkController.singleton.numPlayers >= 1)
            {
                currentCountdown--;
                RpcSetCountdown(currentCountdown);

                if (currentCountdown <= 0)
                {
                    LoadGameScene();
                    yield break;
                }
            }
            else
            {
                currentCountdown = 6;
                RpcSetCountdown(-1);
            }
            yield return new WaitForSeconds(1.0f);
        }
    }

    [ClientRpc]
    private void RpcSetCountdown(int timeLeft)
    {
        if (timeLeft != -1)
            GameObject.Find("Countdown_TextMesh").GetComponent<TextMesh>().text = timeLeft.ToString("00");
        else
            GameObject.Find("Countdown_TextMesh").GetComponent<TextMesh>().text = "Waiting...";
    }

    private void LoadGameScene()
    {
        NetworkController.singleton.ServerChangeScene("Game");        
        //CmdDestroyLobbyPlayers();
    }

    //[SerializeField] private GameObject robotRootPrefab;
    //[SerializeField] private int sceneReadyStatus = 0;

    //private void OnSceneLoaded(string scene)
    //{
    //    Debug.Log("On Scene Loaded");
    //    if (scene == "Game")
    //    {            
    //        CmdSendReadyStatus();
    //    }
    //}

    //[Command]
    //private void CmdSendReadyStatus()
    //{
    //    //ClientScene.Ready(connectionToClient);
    //    NetworkServer.SetClientReady(connectionToClient);
    //    sceneReadyStatus++;
    //    if (sceneReadyStatus >= NetworkController.singleton.numPlayers)
    //    {            
    //        RpcSpawnRobots();
    //        Debug.Log("Scene ready status " + sceneReadyStatus + ", Players: " + NetworkController.singleton.numPlayers);
    //    }
    //}

    //[ClientRpc]
    //private void RpcSpawnRobots()
    //{
    //    Debug.Log("RpcSpawnRobots");
    //    CmdSpawnRobot();
    //}

    //[Command]
    //private void CmdSpawnRobot()
    //{
    //    Debug.Log("CmdSpawnRobot");
    //    GameObject go = Instantiate(robotRootPrefab);
    //    go.transform.position = Vector3.zero;
    //    go.transform.eulerAngles = Vector3.zero;
    //    go.transform.name = "NetworkPlayer";
    //    NetworkServer.SpawnWithClientAuthority(go, connectionToClient);

    //    NetworkServer.Destroy(this.gameObject);
    //}


    //private void OnDestroy()
    //{
    //    NetworkController.singleton.onServerSceneChange -= OnSceneLoaded;
    //}
}
