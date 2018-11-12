using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Robot : MonoBehaviourPunCallbacks {

    Rigidbody rb;
    private Vector3 currentVelocity;

    //private List<Ability> abilities = new List<Ability>();    

    private void Start()
    {
        rb = GetComponent<Rigidbody> ();
        CreateCOMAnchor();
    }

    private void Update()
    {

    }

    private void CreateCOMAnchor()
    {
        GameObject go = new GameObject { name = "COM" };
        go.transform.SetParent(this.transform.Find("Anchors"));
        go.transform.position = rb.worldCenterOfMass;        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawIcon( rb.worldCenterOfMass, "Icon", true);
    }

    private void FixedUpdate ()
    {
        currentVelocity = rb.velocity;
    }

    private void OnCollisionEnter (Collision collision)
    {
        if (collision.gameObject.name == "Ground") return;
        float collisionForce = currentVelocity.magnitude - rb.velocity.magnitude;

        if (collisionForce > 5)
        {
            float volume = Mathf.Lerp ( 0.2f, 1.0f, collisionForce / 10 );
            GameSoundEffectManager.Instance.PlayNetworkSound ( GameSoundEffectManager.Effect.MetalImpact, volume, Random.Range ( 0.75f, 1.25f ), true, collision.contacts[0].point );

            GetComponent<RobotHealth> ().TakeDamage ( Mathf.Lerp ( 1.0f, 15.0f, collisionForce / 20 ) );            
        }        
    }  
}
