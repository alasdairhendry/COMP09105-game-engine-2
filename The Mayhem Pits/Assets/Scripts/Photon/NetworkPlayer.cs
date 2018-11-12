using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkPlayer : MonoBehaviourPunCallbacks, IPunObservable {

    private List<NetworkSpawnPoint> spawnPoints = new List<NetworkSpawnPoint>();
    private Dictionary<int, Player> spawnPointsInUse = new Dictionary<int, Player>();

    [SerializeField] private GameObject networkLobbyPlayerPrefab;
    [SerializeField] private GameObject networkGamePlayerPrefab;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        gameObject.name = "NetworkPlayer_" + photonView.Owner.NickName;
        if (!photonView.IsMine && PhotonNetwork.IsConnected) return;
        CreateNetworkLobbyPlayer();
        SceneManager.sceneLoaded += OnSceneChange;
    }

    private void OnSceneChange(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "Game")
        {
            CreateNetworkGamePlayer();
        }
    }

    private void CreateNetworkLobbyPlayer()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            PhotonNetwork.Instantiate(networkLobbyPlayerPrefab.name, transform.position, transform.rotation, 0);
        }
    }

    private void CreateNetworkGamePlayer()
    {
        NetworkSpawnPoint mySpawnPoint = GetSpawnPoint(GameObject.FindObjectsOfType<NetworkSpawnPoint>());

        PhotonNetwork.Instantiate(networkGamePlayerPrefab.name, mySpawnPoint.transform.position, mySpawnPoint.transform.rotation, 0);
    }

    private NetworkSpawnPoint GetSpawnPoint(NetworkSpawnPoint[] spawnPoints)
    {
        NetworkSpawnPoint mySpawn = spawnPoints[0];
        int indexInPlayerList = 0;
        Player[] playerList = PhotonNetwork.PlayerList;               

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

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //throw new System.NotImplementedException();
    }

    public override void OnPlayerLeftRoom (Player otherPlayer)
    {
        if (KillFeed.Instance == null) return;
        KillFeed.Instance.AddInfo ( otherPlayer.NickName + " has left the game.", KillFeed.InfoType.Disconnect );
    }
}
