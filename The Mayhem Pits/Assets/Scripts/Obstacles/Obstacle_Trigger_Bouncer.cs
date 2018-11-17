using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_Trigger_Bouncer : Obstacle_Trigger {    

    [SerializeField] private float force;
    [SerializeField] private GameObject replayParticlePrefab;

    public override void Activate ()
    {
        //targetCollider.GetComponentInParent<Rigidbody> ().AddForce ( Vector3.up * force * Time.fixedDeltaTime, ForceMode.VelocityChange );
        targetCollider.GetComponentInParent<Rigidbody> ().AddForceAtPosition ( Vector3.up * force * Time.fixedDeltaTime, transform.position, ForceMode.VelocityChange );
        photonView.RPC ( "RPCParticles", RpcTarget.All, null );
        GameSoundEffectManager.Instance.PlayNetworkSound ( GameSoundEffectManager.Effect.SteamBurst, 1.0f, 1.0f, true, transform.position );
    }

    [PunRPC]
    private void RPCParticles ()
    {
        GetComponentInChildren<ParticleSystem> ().Play ();
        GetComponentInChildren<Replayable>().AddFramedAction(() => { GameObject go = Instantiate(replayParticlePrefab, transform.position, Quaternion.identity); });
    }

}
