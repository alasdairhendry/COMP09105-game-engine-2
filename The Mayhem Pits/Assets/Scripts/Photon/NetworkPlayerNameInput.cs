using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkPlayerNameInput : MonoBehaviour
{
    const string playerNamePrefKey = "PlayerName";
    [SerializeField] private Text usernameText;
    
    private void Start()
    {
        string defaultName = "Default User";    
        
        if (PlayerPrefs.HasKey(playerNamePrefKey))
        {
            defaultName = PlayerPrefs.GetString(playerNamePrefKey);
            usernameText.text = defaultName;
        }
    
        PhotonNetwork.NickName = defaultName;
    }

    private void Update ()
    {
        if (Input.GetButtonDown ( "XBO_Y" ))
        {
            //if (PhotonNetwork.InRoom) return;
            FindObjectOfType<Keyboard> ().Open ( (s) => { SetPlayerName ( s ); } );
        }
    }

    public void SetPlayerName(string value)
    {        
        if (string.IsNullOrEmpty(value))
        {
            Debug.LogError("Player Name is null or empty");
            return;
        }

        usernameText.text = value;
        PhotonNetwork.NickName = value;
        PlayerPrefs.SetString(playerNamePrefKey, value);
    }
}