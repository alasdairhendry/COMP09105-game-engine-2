using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviourPunCallbacks {

    [SerializeField] private bool DEBUG_SOLOSTART;
    [SerializeField] private int countdownTime = 5;
    [SerializeField] private TextMesh countdownText;
    [SerializeField] private string roomName;

    private bool countdownIsRunning = false;
    private bool countdownCanRun = false;
    [SerializeField] private int currentCountdown = 5;

    private void Start()
    {
        currentCountdown = countdownTime;
    }

    private void Update ()
    {
        if(PhotonNetwork.CurrentRoom != null)
        {
            roomName = PhotonNetwork.CurrentRoom.Name;
        }
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

    public override void OnLeftRoom ()
    {
        base.OnLeftRoom ();
        countdownIsRunning = false;
        CheckPlayerCount ();
        //Debug.Log ( "Left room" );
    }

    private void CheckPlayerCount()
    {
        if(PhotonNetwork.CurrentRoom == null)
        {
            countdownCanRun = false;           
            currentCountdown = countdownTime;
            return;
        }

        Debug.Log("Current Player Count = " + PhotonNetwork.CurrentRoom.PlayerCount + "  //  MaxPlayers = " + PhotonNetwork.CurrentRoom.MaxPlayers);

        if (DEBUG_SOLOSTART)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers - 1)
            {
                countdownCanRun = true;
                SetRoomLockState ( false );

                if (!countdownIsRunning)
                    StartCoroutine (Countdown());
            }
            else
            {
                countdownCanRun = false;
                currentCountdown = countdownTime;
                SetRoomLockState ( true );
            }
        }
        else
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                countdownCanRun = true;
                SetRoomLockState ( false );

                if (!countdownIsRunning)
                    StartCoroutine ( Countdown () );
            }
            else
            {
                countdownCanRun = false;
                SetRoomLockState ( true );
                currentCountdown = countdownTime;
            }
        }
    }

    private void SetRoomLockState (bool state)
    {
        if (PhotonNetwork.CurrentRoom != null)
            PhotonNetwork.CurrentRoom.IsOpen = state;
    }

    private IEnumerator Countdown()
    {
        countdownIsRunning = true;

        while (true)
        {            
            if (countdownCanRun)
            {
                currentCountdown--;
            }
            else
            {
                countdownIsRunning = false;
                yield break;
            }

            photonView.RPC("SetCurrentCountdown", RpcTarget.AllBuffered, currentCountdown, countdownCanRun);

            if(currentCountdown <= 0)
            {
                SetRoomLockState ( false );
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
