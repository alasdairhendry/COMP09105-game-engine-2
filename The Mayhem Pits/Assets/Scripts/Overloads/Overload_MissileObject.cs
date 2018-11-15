using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overload_MissileObject : MonoBehaviourPunCallbacks {

    [SerializeField] private Vector3 targetPeak;
    [SerializeField] private float targetPeakY = 10.0f;
    [SerializeField] private float boostedSpeed = 25.0f;
    [SerializeField] private float ascendingSpeed = 10.0f;
    [SerializeField] private float damage = 20.0f;
    [SerializeField] private GameObject explosionParticles;

    private NetworkGameRobot targetRobot;
    private Vector3 initialRobotPosition;
    private float initialDistance;
    
    private bool ascending = true;    
    private Vector3 direction = new Vector3 ();

    public void SetTarget(NetworkGameRobot robot)
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

        targetRobot = robot;
        initialRobotPosition = targetRobot.transform.position;
        initialDistance = Vector3.Distance ( transform.position, targetRobot.transform.position );

        targetPeak = transform.position + targetRobot.transform.position;
        targetPeak = targetPeak / 2.0f;
        targetPeak.y = targetPeakY;
    }

    private void Update ()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected) return;
        if (targetRobot == null) return;

        Move ();
    }

    private void Move ()
    {     
        // In case the target disconnects, destroy ourselves.
        if(targetRobot== null)
        {
            PhotonNetwork.Destroy ( this.gameObject );
        }

        float distanceFromPeak = Vector3.Distance ( transform.position, targetPeak );

        if (distanceFromPeak <= 1.0f)
        {
            ascending = false;
        }

        float distanceFromTarget = Vector3.Distance ( transform.position, targetRobot.transform.position );

        if (distanceFromTarget <= 1.0f)
        {
            Explode ();
        }
        
        if (ascending)
        {            
            direction = (targetPeak - transform.position).normalized;
            transform.position += direction * ascendingSpeed * Time.deltaTime;
        }
        else
        {
            direction = (targetRobot.transform.position - transform.position).normalized;
            transform.position += direction * boostedSpeed * Time.deltaTime;
        }  
    }

    private void Explode ()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

        targetRobot.GetComponent<RobotHealth> ().ApplyDamageToOtherPlayer ( damage );
        targetRobot.GetComponent<Heatable>().AddNetwork(5.0f);
        photonView.RPC ( "RPCExplode", RpcTarget.All, targetRobot.GetComponent<PhotonView> ().Owner.ActorNumber );
        PhotonNetwork.Instantiate ( explosionParticles.name, transform.position, Quaternion.identity );        
        PhotonNetwork.Destroy ( this.gameObject );
    }

    [PunRPC] 
    private void RPCExplode(int targetRobot)
    {
        Camera c = FindObjectOfType<Camera>();
        if(Vector3.Distance(c.gameObject.transform.position, transform.position) < 10.0f)
        {
            FindObjectOfType<SFXCutoff>().Cutoff(1.0f);
        }

        GameSoundEffectManager.Instance.PlayLocalSound(GameSoundEffectManager.Effect.Explosion, 1.0f, Random.Range(0.80f, 1.2f), true, transform.position);

        if(targetRobot == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            GameObject.FindGameObjectWithTag ( "LocalGamePlayer" ).GetComponent<Rigidbody> ().AddForceAtPosition ( Vector3.up * 200.0f * Time.fixedDeltaTime, transform.position, ForceMode.VelocityChange );
        }
    }
}
