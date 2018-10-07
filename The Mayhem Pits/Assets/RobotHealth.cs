using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotHealth : MonoBehaviourPunCallbacks {

    [SerializeField] private float maximumHealth = 100.0f;
    [SerializeField] private float currentHealth = 0.0f;
    [SerializeField] private Slider healthSlider;

	// Use this for initialization
	void Start () {
        currentHealth = maximumHealth;

        if (!photonView.IsMine && PhotonNetwork.IsConnected) return;
	}
	
	public void TakeDamage(float damage)
    {
        Debug.Log("You have taken " + damage + " damage");

        currentHealth -= damage;
        healthSlider.value = Mathf.Lerp(0.0f, 1.0f, currentHealth / maximumHealth);

        photonView.RPC("RpcSetHealth", RpcTarget.OthersBuffered, currentHealth);

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    [PunRPC] private void RpcSetHealth(float health)
    {
        currentHealth = health;
        healthSlider.value = Mathf.Lerp(0.0f, 1.0f, currentHealth / maximumHealth);
    }

    private void Die()
    {
        Debug.LogError("You are dead");
    }
}
