using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_GameOver_Panel : MonoBehaviourPunCallbacks {

    [SerializeField] private GameObject body;
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private Text damageInflictedPanel;
    [SerializeField] private Text coinsPanel;
    [SerializeField] private Text coinsWonPanel;
    [SerializeField] private GameObject deadPanel;
    [SerializeField] private GameObject wonPanel;
    [SerializeField] private Text wonText;
    [SerializeField] private GameObject spectateInfoButton;

    private bool givenWinnings = false;
    private float initialCoins = 0;

    private void Start ()
    {
        Close ();
    }

    private void Update()
    {
        if (givenWinnings)
        {
            if (!DatabaseManager.Instance.UserIsLoggedIn) return;

            //int currentCoinsPanel = int.Parse(coinsPanel.text.Remove(0, 1));
            initialCoins = SmoothLerp.Lerp((float)initialCoins, (float)DatabaseManager.Instance.AccountCoins, Time.deltaTime * 20.0f);
            initialCoins = Mathf.Clamp(initialCoins, initialCoins, DatabaseManager.Instance.AccountCoins);            
            coinsPanel.text = "£" + initialCoins.ToString("00");
        }
    }

    public void Open (bool dead)
    {
        body.SetActive ( true );

        if (dead)
        {
            wonPanel.SetActive ( false );
            deadPanel.SetActive ( true );
            spectateInfoButton.SetActive ( true );
        }
        else
        {
            deadPanel.SetActive ( false );
            wonPanel.SetActive ( true );
            spectateInfoButton.SetActive ( false );
        }
    }

    public void SetWinner(string name)
    {
        wonText.text = name;
    }

    public void OnPress_Spectate ()
    {
        spectateInfoButton.SetActive(false);
        deadPanel.SetActive(false);
        infoPanel.SetActive(false);
        KillFeed.Instance.AddInfo ( PhotonNetwork.NickName + " is now spectating.", KillFeed.InfoType.Spectate, RpcTarget.AllBuffered );
    }

    public void OnPress_Menu ()
    {        
        PhotonNetwork.LeaveRoom ();
    }
   
    public void Close ()
    {
        body.SetActive ( false );
    }

    public void CalculateCoins()
    {
        float damageInflicted = GameObject.FindGameObjectWithTag("LocalGamePlayer").GetComponent<NetworkGameRobot>().damageInflicted;
        damageInflictedPanel.text = "Damage Inflicted: " + damageInflicted.ToString("00");

        if (!DatabaseManager.Instance.UserIsLoggedIn) return;
        if (givenWinnings) return;
        givenWinnings = true;

        // Setup for death replay
        RobotHealth[] allPlayers = FindObjectsOfType<RobotHealth>();
        int leftAlive = 0;

        for (int i = 0; i < allPlayers.Length; i++)
        {
            if (!allPlayers[i].DeathCalled)
            {
                leftAlive++;
            }
        }

        //int numPlayers = PhotonNetwork.CurrentRoom.MaxPlayers;
        int coinsWon = 0;
        coinsPanel.text = "£" + DatabaseManager.Instance.AccountCoins.ToString("00");
        initialCoins = (float)DatabaseManager.Instance.AccountCoins;
        Debug.Log("Set initial coins " + initialCoins);

        if (leftAlive == 1)
        {
            // Won
            coinsWon = 75;            
        }
        else if (leftAlive == 2)
        {
            // Finished second
            coinsWon = 50;            
        }
        else if (leftAlive == 3)
        {
            // Finished third
            coinsWon = 30;            
        } 
        else if (leftAlive == 4)
        {
            // Finished last
            coinsWon = 10;
        }

        float coinMultipler = Mathf.Lerp(1.0f, 1.5f, damageInflicted / 200.0f);
        coinsWon = (int)(coinsWon * coinMultipler);

        DatabaseManager.Instance.AddCoins(coinsWon);
        coinsWonPanel.text = "+ £" + coinsWon.ToString("00");
    }
}
