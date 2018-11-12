using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_GameOver_Panel : MonoBehaviour {

    [SerializeField] private GameObject body;
    [SerializeField] private GameObject deadPanel;
    [SerializeField] private GameObject wonPanel;
    [SerializeField] private Text wonText;
    [SerializeField] private GameObject spectateInfoButton;

    private void Start ()
    {
        Close ();
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
        Close ();
        KillFeed.Instance.AddInfo ( PhotonNetwork.NickName + " is now spectating.", KillFeed.InfoType.Spectate, RpcTarget.AllBuffered );
    }

    public void OnPress_Menu ()
    {
        Debug.Log ( "BOOOOO" );
        PhotonNetwork.LeaveRoom ();
        SceneLoader.singleton.LoadScene ( "Menu" );
    }
   
    public void Close ()
    {
        body.SetActive ( false );
    }
}
