using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD_PauseMenu_Panel : MonoBehaviour {

    [SerializeField] private GameObject body;
    private bool isPaused = false;
    public bool IsPaused { get { return isPaused; } }

    private void Update()
    {
        if (Input.GetButtonDown("XBO_Start"))
        {
            if (isPaused)
            {
                Close();
                FindObjectOfType<HUD_Settings_Panel>().Close();
                isPaused = false;
            }
            else
            {
                Open();
            }
        }
    }

    public void Open()
    {
        if (!FindObjectOfType<MatchStartController>().gameHasBegun) return;
        if (body.activeSelf) return;

        body.SetActive(true);
        body.GetComponent<HUDSelectionGroup>().SetActiveGroup();
        isPaused = true;
    }

    public void Close()
    {
        body.SetActive(false);
    }

    public void OnClick_Resume()
    {
        body.SetActive(false);
        isPaused = false;
    }

    public void OnClick_Quit()
    {
        PhotonNetwork.LeaveRoom();
        SceneLoader.Instance.LoadScene("Menu");
    }
}
