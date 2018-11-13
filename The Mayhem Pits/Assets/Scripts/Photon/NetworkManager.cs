using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks {

    public static NetworkManager singleton;

    private readonly string gameVersion;
    private byte selectedRoomSize = 2;
    [SerializeField] private Text status_Label;

    [SerializeField] private GameObject networkPlayerPrefab;

    private GameObject localNetworkPlayer;
    private Player[] playerList;
    public Action<Player[]> onPlayerListChanged;

    public Action onJoinedRoom;
    public Action onLeftRoom;

    private float retryConnectionDelay = 7.0f;
    private float currentRetryDelay = 0.0f;

    private bool failedToConnect = false;

    private void Awake()
    {
        if (singleton == null)
            singleton = this;
        else if (singleton != this)
            Destroy(gameObject);

        DontDestroyOnLoad(this.gameObject);

        // Allows scenes to be synced over the network
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Use this for initialization
    void Start () {
        ConnectToMasterServer();	
	}

    private void Update ()
    {
        if (failedToConnect)
        {
            currentRetryDelay += Time.deltaTime;

            if(currentRetryDelay >= 2.0f)
            {
                status_Label.text = "Retrying connection in " + (retryConnectionDelay - currentRetryDelay).ToString ( "00" ) + "...";
            }

            if(currentRetryDelay >= retryConnectionDelay)
            {
                currentRetryDelay = 0.0f;
                ConnectToMasterServer ();
            }
        }
    }

    // Tries to connect the player to the master server
    private void ConnectToMasterServer()
    {
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    // Called when the player successfully connects to the master server
    public override void OnConnectedToMaster()
    {
        failedToConnect = false;
        if (SceneLoader.Instance.CurrentScene() == "NetworkConnection")
        {
            SceneLoader.Instance.LoadScene("ModeSelect");
        }
    }    

    // Called when the player disconnects from the master server
    public override void OnDisconnected(DisconnectCause cause)
    {
        failedToConnect = true;

        if(SceneLoader.Instance.CurrentScene() == "NetworkConnection")
        {
            status_Label = GameObject.Find ( "ConnectionStatus_Label" ).GetComponent<Text>();
            status_Label.color = new Color(250.0f / 256.0f, 138.0f / 256.0f, 138.0f / 256.0f);
            status_Label.text = "Failed to connect: " + cause.ToString();
        }
        else
        {
            if (cause == DisconnectCause.DisconnectByClientLogic) return;
            SceneLoader.Instance.LoadScene ( "NetworkConnection" );
            ConnectToMasterServer ();
        }
    }

    // Called by the player to start a game
    public void FindGame(byte _selectedRoomSize)
    {
        if (PhotonNetwork.IsConnected)
        {
            selectedRoomSize = _selectedRoomSize;
            PhotonNetwork.JoinRandomRoom(null, selectedRoomSize, MatchmakingMode.FillRoom, null, null, null);
        }
        else
        {
            Debug.Log("You are not connected to a server");
        }
    }

    // Called when the player successfully joins a room
    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room.");

        UpdatePlayerList();
        SpawnNetworkPlayer();

        if (onJoinedRoom != null)
            onJoinedRoom();
    }

    // Called when the player fails to join a room
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.        
        CreateGame();
    }

    private void CreateGame()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = selectedRoomSize;
        PhotonNetwork.CreateRoom(null, roomOptions, null, null);
    }

    public void DisconnectFromRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        if (onLeftRoom != null)
            onLeftRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }

    private void UpdatePlayerList()
    {
        playerList = PhotonNetwork.PlayerList;
        playerList = playerList.OrderBy(x => x.ActorNumber).ToArray();
        if (onPlayerListChanged != null)
        {
            onPlayerListChanged(playerList);
        }
    }

    private void SpawnNetworkPlayer()
    {
        NetworkSpawnPoint[] spawns = GameObject.FindObjectsOfType<NetworkSpawnPoint>();

        NetworkSpawnPoint mySpawn = spawns[0];
        int indexInPlayerList = 0;

        for (int i = 0; i < playerList.Length; i++)
        {
            if (playerList[i].ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                indexInPlayerList = i;
                break;
            }
        }

        for (int i = 0; i < spawns.Length; i++)
        {
            if (spawns[i].index == indexInPlayerList)
            {
                mySpawn = spawns[i];
                break;
            }
        }

        //Debug.Log("IndexInPlayerList = " + indexInPlayerList);
        //Debug.Log("IndexInSpawnPointList = " + mySpawn.index);

        Vector3 _pos = mySpawn.transform.position;
        Quaternion _rot = mySpawn.transform.rotation;
        localNetworkPlayer = PhotonNetwork.Instantiate(networkPlayerPrefab.name, _pos, _rot, 0);
    }

    GUIStyle style = new GUIStyle();
    private void OnGUI()
    {
        if (PhotonNetwork.IsConnected)
        {
            style.fontSize = 25;
            style.fontStyle = FontStyle.BoldAndItalic;            
            GUILayout.Label(PhotonNetwork.CountOfPlayers + " players online", style);
        }
    }
}
