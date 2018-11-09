using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotHealth : MonoBehaviourPunCallbacks {

    [SerializeField] private float maximumHealth = 100.0f;
    [SerializeField] private float currentHealth = 0.0f;
    [SerializeField] private Slider healthSlider;
    private bool deathCalled = false;

	// Use this for initialization
	void Start () {
        currentHealth = maximumHealth;        
	}

    private void Update ()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

        if (!deathCalled)
            Die ();
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
        Debug.Log ( "Taking damage", this );
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
            FindObjectOfType<HUD_GameOver_Panel> ().Open ();
            GetComponent<Test_RobotMovement> ().enabled = false;
            GetComponent<RobotAbilities> ().enabled = false;
            GetComponentInChildren<Weapon> ().enabled = false;
            deathCalled = true;
            FindObjectOfType<Test_SmoothCamera> ().SetModeOverview ();
            KillFeed.Instance.AddInfo ( photonView.Owner.NickName + " has been eliminated", KillFeed.InfoType.Killed, RpcTarget.AllBuffered );
        }
    }
}
