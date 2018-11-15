using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchStartController : MonoBehaviourPunCallbacks {

    private float currentCountdown = 4.0f;
    private bool ready = false;

    [SerializeField] private GameObject startPanel;
    [SerializeField] private Text countdownText;
	
    public void SetReady()
    {
        ready = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SetCamera();
            Begin();
            return;
        }
        if (ready)
        {
            if (currentCountdown <= 0) return;
            photonView.RPC("RPCCountdown", RpcTarget.AllBuffered, currentCountdown - (Time.deltaTime * 0.5f));
        }
    }

    [PunRPC]
    private void RPCCountdown(float time)
    {
        currentCountdown = time;

        if (currentCountdown >= 3)
            countdownText.text = "GET READY";
        else if(currentCountdown <= 1.0f)
        {
            countdownText.text = "FIGHT";
            SetCamera();
        }
        else
        {
            countdownText.text = currentCountdown.ToString("00");
        }

        if(currentCountdown <= 0)
        {
            Begin();
        }
    }

    private void SetCamera()
    {
        FindObjectOfType<Test_SmoothCamera>().SetMode(Test_SmoothCamera.TargetType.Robot);
        FindObjectOfType<Test_SmoothCamera>().SetAllowTarget(true);
    }

    private void Begin()
    {
        startPanel.SetActive(false);
        GameObject.FindGameObjectWithTag("LocalGamePlayer").GetComponent<Test_RobotMovement>().enabled = true;
        GameObject.FindGameObjectWithTag("LocalGamePlayer").GetComponent<RobotAbilities>().SetAllowUse(true);
        GameObject.FindGameObjectWithTag("LocalGamePlayer").GetComponent<RobotOverloads>().SetAllowUse(true);
    }
}
