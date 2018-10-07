using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSpawnLocations : MonoBehaviour {

    [SerializeField] private List<GameObject> spawnGroups = new List<GameObject>();

	// Use this for initialization
	private void Awake () {
        if (PhotonNetwork.OfflineMode || PhotonNetwork.IsConnected == false)
        {
            spawnGroups[0].SetActive(true);
        }
        else
        {
            if (PhotonNetwork.CurrentRoom.MaxPlayers == 2)
            {
                spawnGroups[0].SetActive(true);
            }
            else if (PhotonNetwork.CurrentRoom.MaxPlayers == 3)
            {
                spawnGroups[1].SetActive(true);
            }
            else if (PhotonNetwork.CurrentRoom.MaxPlayers == 4)
            {
                spawnGroups[2].SetActive(true);
            }
        }
    }
}
