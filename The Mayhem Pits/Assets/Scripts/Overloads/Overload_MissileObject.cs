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
    [SerializeField] private GameObject localExplosionParticles;
    [SerializeField] private GameObject networkExplosionParticles;

    private NetworkGameRobot targetRobot;
    private Vector3 initialRobotPosition;
    private float initialDistance;
    
    private bool ascending = true;    
    private Vector3 direction = new Vector3 ();

    private bool hasExploded = false;
    private bool hasDestroyed = false;
    private float destroyDelay = 0.25f;

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

    private void Start()
    {
        FindObjectOfType<MatchStartController>().RegisterMatchEnd(OnMatchEnd);
    }

    private void Update ()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected) return;
        if (targetRobot == null) return;

        Move ();

        if(hasExploded && !hasDestroyed)
        {
            destroyDelay -= Time.deltaTime;

            if(destroyDelay<= 0.0f)
            {
                hasDestroyed = true;
                PhotonNetwork.Destroy(this.gameObject);
            }
        }
    }

    private void Move ()
    {
        if (hasExploded) return;

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
        if (hasExploded) return;

        hasExploded = true;

        Vector3 bounceDirection = (transform.position - targetRobot.transform.position).normalized;
        GetComponent<Rigidbody>().velocity = bounceDirection * 4.0f;
        GetComponent<Rigidbody>().useGravity = true;

        GameObject.FindGameObjectWithTag("LocalGamePlayer").GetComponent<NetworkGameRobot>().damageInflicted += damage;
        targetRobot.GetComponent<RobotHealth> ().ApplyDamageToOtherPlayer ( damage );
        //targetRobot.GetComponent<Heatable>().AddNetwork(5.0f);
        photonView.RPC ( "RPCExplode", RpcTarget.All, targetRobot.GetComponent<PhotonView> ().Owner.ActorNumber );
        PhotonNetwork.Instantiate ( networkExplosionParticles.name, transform.position, Quaternion.identity );

        Debug.Log("Explode");
    }

    [PunRPC] 
    private void RPCExplode(int targetRobot)
    {
        // Set sound effects low pass filter
        Camera c = FindObjectOfType<Camera>();
        if(Vector3.Distance(c.gameObject.transform.position, transform.position) < 10.0f)
        {
            FindObjectOfType<SFXCutoff>().Cutoff(1.0f);
        }        

        // Play explosion sound 
        GameSoundEffectManager.Instance.PlayLocalSound(GameSoundEffectManager.Effect.Explosion, 1.0f, Random.Range(0.80f, 1.2f), true, transform.position);

        // Apply explosion force if we're the local player
        if(targetRobot == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            GameObject.FindGameObjectWithTag ( "LocalGamePlayer" ).GetComponent<Rigidbody> ().AddForceAtPosition ( Vector3.up * 200.0f * Time.fixedDeltaTime, transform.position, ForceMode.VelocityChange );
        }

        // Create explosion replay callback
        string path = localExplosionParticles.name;
        Vector3 position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        Debug.Log("Adding framed action");
        GetComponent<ReplayInvoker>().AddFramedAction(() => 
        {
            SmoothLerp.Instantiate(path, position, "Replayable");
            Debug.Log("framed action");
        });

        GetComponent<ReplayInvoker>().RequestReplay(2.0f);
        FindObjectOfType<MatchStartController>().UnRegisterMatchEnd(OnMatchEnd);

        if (!photonView.IsMine && PhotonNetwork.IsConnected) return;
        //

        Debug.Log("RPCExplode");
    }   

    private void OnMatchEnd()
    {
        if (this == null) return;
        GameSoundEffectManager.Instance.PlayLocalSound(GameSoundEffectManager.Effect.Explosion, 1.0f, Random.Range(0.80f, 1.2f), true, transform.position);
        GameObject go = Instantiate(localExplosionParticles, transform.position, Quaternion.identity);        

        if (!photonView.IsMine && PhotonNetwork.IsConnected) return;
        PhotonNetwork.Destroy(this.gameObject);
    }
}
