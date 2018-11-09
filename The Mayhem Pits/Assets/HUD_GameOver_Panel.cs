using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD_GameOver_Panel : MonoBehaviour {

    [SerializeField] private GameObject body;

    private void Start ()
    {
        Close ();
    }

    public void Open ()
    {
        body.SetActive ( true );
    }

    public void OnPress_Spectate ()
    {
        Close ();
        KillFeed.Instance.AddInfo ( PhotonNetwork.NickName + " is now spectating.", KillFeed.InfoType.Spectate, RpcTarget.AllBuffered );
    }

    public void OnPress_Menu ()
    {
        Debug.Log ( "BOOOOO" );
        return;
        PhotonNetwork.LeaveRoom ();
        SceneLoader.singleton.LoadScene ( "Menu" );
    }
   
    public void Close ()
    {
        body.SetActive ( false );
    }
}
