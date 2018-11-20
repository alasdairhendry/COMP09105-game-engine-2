using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkPlayerNameInput : MonoBehaviour
{    
    [SerializeField] private Text usernameText;    
    [SerializeField] private InfoButton yButton;
    [SerializeField] private Image userImage;
    [SerializeField] private Sprite[] userSprites;
    
    private void Start()
    {
        if (DatabaseManager.Instance.UserIsLoggedIn)
        {
            PhotonNetwork.NickName = DatabaseManager.Instance.AccountUsername;
            usernameText.text = DatabaseManager.Instance.AccountUsername;
        }
        else
        {
            OnLogout ();
        }

        DatabaseManager.Instance.onLogin += OnLogin;
        DatabaseManager.Instance.onLogout += OnLogout;
    }

    public void OnHold_Y ()
    {
        if (DatabaseManager.Instance.UserIsLoggedIn)
        {
            DatabaseManager.Instance.LogOut ();            
        }
        else
        {
            //DatabaseManager.Instance.CheckStoredLogin ();
            FindObjectOfType<HUD_DatabaseLogin_Panel> ().Open ();
            yButton.SetEnabled ( false );          
        }
    }

    private void OnLogin(string username)
    {
        PhotonNetwork.NickName = username;
        usernameText.text = username;
        yButton.SetText ( "Log out" );
        userImage.sprite = userSprites[1];

        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom ();
    }

    private void OnLogout ()
    {
        float value = Random.Range ( 1000.0f, 9999.0f );
        PhotonNetwork.NickName = "Guest-" + value.ToString ("0000");
        usernameText.text = "Guest-" + value.ToString("0000");
        yButton.SetText ( "Log in" );
        userImage.sprite = userSprites[0];

        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom ();
    }

    private void OnDestroy ()
    {
        DatabaseManager.Instance.onLogin -= OnLogin;
        DatabaseManager.Instance.onLogout -= OnLogout;
    }
}