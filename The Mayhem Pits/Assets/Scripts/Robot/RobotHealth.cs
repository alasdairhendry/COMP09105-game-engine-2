using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotHealth : MonoBehaviourPunCallbacks {

    [SerializeField] private float maximumHealth = 100.0f;
    [SerializeField] private float currentHealth = 0.0f;
    [SerializeField] private Slider healthSlider;
    private bool deathCalled = false;

    private ParticleSystem.MainModule particlesModule;
    private ParticleSystem particles;

	// Use this for initialization
	void Start () {
        currentHealth = maximumHealth;
        particles = GetComponentInChildren<ParticleSystem> ();
        particlesModule = particles.main;
	}

    private void Update ()
    {
        UpdateParticles ();
        if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

        if (!deathCalled)
            Die ();
    }

    private void UpdateParticles ()
    {
        float healthPercent = Mathf.Lerp ( 0.0f, 100.0f, currentHealth / maximumHealth );

        if(healthPercent <= 75.0f)
        {
            if (!particles.isPlaying)
                particles.Play ();

            particlesModule.startSizeMultiplier = Mathf.Lerp ( 3.0f, 0.5f, healthPercent / 75.0f );
        }
    }

    public void TakeDamage(float damage)
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected) { Debug.Log ( "Cant apply damage as this component does not belong to my client" ); return; }
        //Debug.Log("You have taken " + damage + " damage");

        currentHealth -= damage;
        healthSlider.value = Mathf.Lerp(0.0f, 1.0f, currentHealth / maximumHealth);

        photonView.RPC("RpcSetHealth", RpcTarget.OthersBuffered, currentHealth);
    }

    /// <summary>
    /// This should only be called when one player is damaging another, not for local damages such as local collisions etc
    /// </summary>
    public void ApplyDamageToOtherPlayer (float damage)
    {
        currentHealth -= damage;
        healthSlider.value = Mathf.Lerp ( 0.0f, 1.0f, currentHealth / maximumHealth );

        photonView.RPC ( "RpcSetHealth", RpcTarget.OthersBuffered, currentHealth );
    }

    [PunRPC] private void RpcSetHealth(float health)
    {
        currentHealth = health;
        healthSlider.value = Mathf.Lerp(0.0f, 1.0f, currentHealth / maximumHealth);
    }

    private void Die()
    {
        if(currentHealth <= 0)
        {            
            FindObjectOfType<HUD_GameOver_Panel> ().Open (true);
            GetComponent<Test_RobotMovement> ().enabled = false;
            GetComponent<RobotAbilities> ().enabled = false;
            GetComponent<RobotOverloads> ().enabled = false;
            GetComponentInChildren<Weapon> ().enabled = false;

            FindObjectOfType<HUD_Ability_Panel>().gameObject.SetActive(false);
            FindObjectOfType<HUD_Crosshair_Panel>().gameObject.SetActive(false);
            FindObjectOfType<HUD_Overloads_Panel>().gameObject.SetActive(false);
            FindObjectOfType<HUD_Weapon_Panel>().gameObject.SetActive(false);

            FindObjectOfType<Test_SmoothCamera>().SetMode(Test_SmoothCamera.TargetType.Overview);
            FindObjectOfType<Test_SmoothCamera>().DisableModeSwitch();

            GetComponent<RobotSound> ().StopAudio ();
            deathCalled = true;
            FindObjectOfType<Test_SmoothCamera> ().SetMode (Test_SmoothCamera.TargetType.Overview);
            KillFeed.Instance.AddInfo ( photonView.Owner.NickName + " has been eliminated", KillFeed.InfoType.Killed, RpcTarget.AllBuffered );
            photonView.RPC ( "RPCDie", RpcTarget.AllBuffered );
        }
    }

    [PunRPC]
    private void RPCDie ()
    {
        deathCalled = true;

        if (PhotonNetwork.IsMasterClient)
        {
            CheckWinner ();
        }
    }

    private void CheckWinner ()
    {
        RobotHealth[] allPlayers = FindObjectsOfType<RobotHealth> ();
        int leftAlive = 0;
        int winnerIndex = -1;        

        for (int i = 0; i < allPlayers.Length; i++)
        {
            if (!allPlayers[i].deathCalled)
            {
                leftAlive++;
                winnerIndex = i;
            }
        }

        if (leftAlive == 1)
        {
            // A player has won the match
            KillFeed.Instance.AddInfo ( allPlayers[winnerIndex].photonView.Owner.NickName + " has won the tournament!", KillFeed.InfoType.Winner, RpcTarget.AllBuffered );
            photonView.RPC ( "RPCOnPlayerWin", RpcTarget.AllBuffered, allPlayers[winnerIndex].photonView.Owner.ActorNumber, allPlayers[winnerIndex].photonView.Owner.NickName );            
        }
    }

    [PunRPC]
    private void RPCOnPlayerWin (int playerID, string playerName)
    {
        NetworkGameRobot[] robots = FindObjectsOfType<NetworkGameRobot> ();
        NetworkGameRobot winningRobot = robots[0];
        NetworkGameRobot myRobot = robots[0];

        for (int i = 0; i < robots.Length; i++)
        {
            if (robots[i].photonView.Owner.ActorNumber == playerID)
            {
                // Find the winning robot
                winningRobot = robots[i];
            }

            if (robots[i].photonView.Owner.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                // Find the local robot
                myRobot = robots[i];
            }
        }

        if (playerID == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            // This client has won            
            FindObjectOfType<HUD_GameOver_Panel> ().Open ( false );
            FindObjectOfType<HUD_GameOver_Panel> ().SetWinner ( playerName );
            FindObjectOfType<Test_SmoothCamera> ().SetMode ( Test_SmoothCamera.TargetType.Overview );
        }
        else
        {
            // This client has lost
            FindObjectOfType<Test_SmoothCamera> ().SetTarget ( winningRobot.transform );
            FindObjectOfType<HUD_GameOver_Panel> ().Open ( false );
            FindObjectOfType<HUD_GameOver_Panel> ().SetWinner ( playerName );

            FindObjectOfType<Test_SmoothCamera> ().SetMode ( Test_SmoothCamera.TargetType.Overview );
            FindObjectOfType<Test_SmoothCamera> ().DisableModeSwitch ();
            FindObjectOfType<Test_SmoothCamera> ().GetComponentInChildren<Camera> ().fieldOfView = 40.0f;

        }

        FindObjectOfType<HUD_Ability_Panel>().gameObject.SetActive(false);
        FindObjectOfType<HUD_Crosshair_Panel>().gameObject.SetActive(false);
        FindObjectOfType<HUD_Overloads_Panel>().gameObject.SetActive(false);
        FindObjectOfType<HUD_Weapon_Panel>().gameObject.SetActive(false);

        winningRobot.GetComponent<RobotHealth> ().enabled = false;
        FindObjectOfType<CelebrationEffects> ().Activate ();
    }
}
