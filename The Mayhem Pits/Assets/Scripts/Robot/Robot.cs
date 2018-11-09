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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Ground") return;
        float collisionForce = currentVelocity.magnitude - rb.velocity.magnitude;
        
        if(collisionForce > 5)
        {
            GetComponent<RobotHealth>().TakeDamage(Mathf.Lerp(1.0f, 15.0f, collisionForce / 20));
        }

        //if (collision.gameObject.name == "Barriers")
        //{            
        //    Rigidbody rb = GetComponent<Rigidbody> ();           
        //    Vector3 reboundDirection = new Vector3 ( collision.contacts[0].normal.x, 0.0f, collision.contacts[0].normal.z );
        //    rb.AddForce ( reboundDirection * 2.0f * (Mathf.Clamp ( collision.relativeVelocity.magnitude, 0.0f, 10.0f ) / rb.velocity.magnitude), ForceMode.Impulse );            
        //}
    }

    public void AddHeat(float amount)
    {
        GetComponent<Heatable> ().Add ( amount );
        photonView.RPC ( "RPCAddHeat", RpcTarget.OthersBuffered, amount );
    }

    [PunRPC]
    private void RPCAddHeat (float amount)
    {
        GetComponent<Heatable> ().Add ( amount );
    }
}
