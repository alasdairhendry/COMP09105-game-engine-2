using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverloadAbilitySpawner : MonoBehaviourPunCallbacks {

    [SerializeField] private GameObject pickupPrefab;
    [SerializeField] private List<GameObject> overloads = new List<GameObject> ();
    [SerializeField] private List<GameObject> abilities = new List<GameObject> ();
    [SerializeField] private List<GameObject> spawnPoints = new List<GameObject> ();

    private readonly float defaultDelay = 25.0f;
    [SerializeField] private float spawnDelay = 25.0f;
    private float currentDelay = 0.0f;

	// Use this for initialization
	void Start () {       
        if (!PhotonNetwork.IsMasterClient) return;
        SetSpawnDelay ( PhotonNetwork.PlayerList.Length );
	}

    public override void OnMasterClientSwitched (Player newMasterClient)
    {
        if (photonView.Owner == null) return;
        if (PhotonNetwork.LocalPlayer == null) return;

        if (newMasterClient == PhotonNetwork.LocalPlayer)
        {
            SetSpawnDelay ( PhotonNetwork.PlayerList.Length - 1 );
        }
    }

    private void SetSpawnDelay (int playerCount)
    {        
        spawnDelay = Mathf.Lerp ( defaultDelay, 0.0f, (float)playerCount / 4.0f );
    }

    // Update is called once per frame
    void Update () {
        if (!PhotonNetwork.IsMasterClient) return;
        CheckDelay ();
    }

    private void CheckDelay ()
    {
        Pickup[] pickups = FindObjectsOfType<Pickup> ();
        if (pickups.Length > 0) return;

        currentDelay += Time.deltaTime;

        if(currentDelay >= spawnDelay)
        {
            currentDelay = 0.0f;
            SpawnObject ();
        }
    }

    private void SpawnObject ()
    {
        float chance = UnityEngine.Random.value;

        if(chance >= 0.51f)
        {
            SpawnOverload ();
        }
        else
        {
            SpawnAbility ();
        }
    }

    private void SpawnOverload ()
    {
        GameObject go = PhotonNetwork.InstantiateSceneObject ( pickupPrefab.name, GetSpawnPoint (), Quaternion.identity );
        photonView.RPC ( "RPCSetupOverload", RpcTarget.All, go.GetComponent<PhotonView> ().ViewID, UnityEngine.Random.Range ( 0, overloads.Count ) );
        KillFeed.Instance.AddInfo ( "AN OVERLOAD HAS SPAWNED. FIND AND COLLECT IT.", KillFeed.InfoType.Overload, RpcTarget.All );
    }

    [PunRPC]
    private void RPCSetupOverload (int viewID, int overloadIndex)
    {
        PhotonView view = PhotonView.Find ( viewID );
        if (view == null) return;
        view.GetComponent<Pickup> ().Setup ( overloads[overloadIndex], Pickup.Type.Overload );
    }

    private void SpawnAbility ()
    {
        GameObject go = PhotonNetwork.InstantiateSceneObject ( pickupPrefab.name, GetSpawnPoint (), Quaternion.identity );
        photonView.RPC ( "RPCSetupAbility", RpcTarget.All, go.GetComponent<PhotonView> ().ViewID, UnityEngine.Random.Range ( 0, abilities.Count ) );
        KillFeed.Instance.AddInfo ( "AN ABILITY HAS SPAWNED. FIND AND COLLECT IT.", KillFeed.InfoType.Ability, RpcTarget.All );
    }

    [PunRPC]
    private void RPCSetupAbility (int viewID, int abilityIndex)
    {
        PhotonView view = PhotonView.Find ( viewID );
        if (view == null) return;
        view.GetComponent<Pickup> ().Setup ( abilities[abilityIndex], Pickup.Type.Ability );
    }

    private Vector3 GetSpawnPoint ()
    {
        return spawnPoints[UnityEngine.Random.Range ( 0, spawnPoints.Count )].transform.position;
    }
}
