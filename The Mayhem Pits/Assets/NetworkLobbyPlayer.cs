using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkLobbyPlayer : NetworkBehaviour {

    [SyncVar] [SerializeField] private string playerName = "";
    public string GetPlayerName { get { return playerName; } }

    [SerializeField] private List<GameObject> robotBodies = new List<GameObject>();

    private int currentCountdown = 6;

    // Use this for initialization
    void Start () {        
        if (!isLocalPlayer) return;
        SetPlayerName();
        CmdSpawnLobbyGraphics();

        if(isServer)
        {
            StartCoroutine(Countdown());
        }
    }

    private void SetPlayerName()
    {
        if (isServer)
        {
            CmdSetPlayerName("Host");
        }
        else
        {
            CmdSetPlayerName("Client");
        }
    }

    [Command]
    private void CmdSetPlayerName(string _playerName)
    {
        playerName = _playerName;        
    }
    
    [Command]
    private void CmdSpawnLobbyGraphics()
    {
        GameObject graphics = Instantiate(robotBodies[Random.Range(0, robotBodies.Count)], GameObject.Find("LobbyRobots").transform.Find("Ground").transform);
        graphics.transform.position = transform.position;
        graphics.transform.rotation = transform.rotation;
        NetworkServer.SpawnWithClientAuthority(graphics, connectionToClient);
    }

    private IEnumerator Countdown()
    {
        while(true)
        {
            if (NetworkController.singleton.numPlayers >= 2)
            {
                currentCountdown--;
                RpcSetCountdown(currentCountdown);

                if(currentCountdown <= 0)
                {
                    RpcStartGame();
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

    [ClientRpc]
    private void RpcStartGame()
    {
        SceneManager.LoadScene("Game");
    }
}
