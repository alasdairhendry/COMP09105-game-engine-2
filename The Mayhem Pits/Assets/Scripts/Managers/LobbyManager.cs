using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviourPunCallbacks {

    [SerializeField] private bool DEBUG_SOLOSTART;
    [SerializeField] private int countdownTime = 5;
    [SerializeField] private TextMesh countdownText;

    private bool countdownCanRun = false;
    [SerializeField] private int currentCountdown = 5;

    private void Start()
    {
        currentCountdown = countdownTime;
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        CheckPlayerCount();
        photonView.RPC("SetCurrentCountdown", RpcTarget.AllBuffered, currentCountdown, countdownCanRun);
    }

    public override void OnJoinedRoom()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        CheckPlayerCount();
        photonView.RPC("SetCurrentCountdown", RpcTarget.AllBuffered, currentCountdown, countdownCanRun);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        CheckPlayerCount();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        CheckPlayerCount();
    }

    private void CheckPlayerCount()
    {
        Debug.Log("Current Player Count = " + PhotonNetwork.CurrentRoom.PlayerCount + "  //  MaxPlayers = " + PhotonNetwork.CurrentRoom.MaxPlayers);

        if (DEBUG_SOLOSTART)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers - 1)
            {
                countdownCanRun = transform;
                StartCoroutine(Countdown());
            }
            else
            {
                countdownCanRun = false;
                currentCountdown = countdownTime;
            }
        }
        else
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                countdownCanRun = transform;
                StartCoroutine(Countdown());
            }
            else
            {
                countdownCanRun = false;
                currentCountdown = countdownTime;
            }
        }
    }

    private IEnumerator Countdown()
    {
        while (true)
        {
            if (countdownCanRun)
            {
                currentCountdown--;
            }

            photonView.RPC("SetCurrentCountdown", RpcTarget.AllBuffered, currentCountdown, countdownCanRun);

            if(currentCountdown <= 0)
            {
                PhotonNetwork.LoadLevel("Game");
            }

            yield return new WaitForSeconds(1.0f);
        }
    }

    [PunRPC] private void SetCurrentCountdown(int current, bool isRunning)
    {
        currentCountdown = current;

        if (isRunning)
        {
            // Update HUD
            countdownText.text = current.ToString("00");
        }
        else
        {
            // Set hud "Waiting"
            countdownText.text = current.ToString("Waiting...");
        }
    }

}
