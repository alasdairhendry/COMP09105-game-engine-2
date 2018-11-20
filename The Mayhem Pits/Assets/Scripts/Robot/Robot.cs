using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Robot : MonoBehaviourPunCallbacks {

    private Rigidbody rb;
    private Vector3 currentVelocity;
    //[SerializeField] private GameObject particlesPrefab;

    //private List<Ability> abilities = new List<Ability>();    

    private void Start()
    {
        rb = GetComponent<Rigidbody> ();
        CreateCOMAnchor();
    }

    private void CreateCOMAnchor()
    {
        GameObject go = new GameObject { name = "COM" };
        go.transform.SetParent(this.transform.Find("Anchors"));
        go.transform.position = rb.worldCenterOfMass;        
    }  

    private void FixedUpdate ()
    {
        currentVelocity = rb.velocity;
    }

    private void OnCollisionEnter (Collision collision)
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected) return;
        float collisionForce = collision.relativeVelocity.magnitude;

        float volume = Mathf.Lerp ( 0.0f, 1.0f, collisionForce / 40 );
        GameSoundEffectManager.Instance.PlayNetworkSound ( GameSoundEffectManager.Effect.MetalImpact, volume, Random.Range ( 0.75f, 1.25f ), true, collision.contacts[0].point );

        if (collision.gameObject.name == "Ground") return;
        float stoppingForce = currentVelocity.magnitude - rb.velocity.magnitude;

        if (stoppingForce > 5)
        {
            GetComponent<RobotHealth> ().TakeDamage ( Mathf.Lerp ( 1.0f, 15.0f, stoppingForce / 20 ) );            
        }        
    }  
}
