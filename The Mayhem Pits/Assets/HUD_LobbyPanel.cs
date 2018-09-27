using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_LobbyPanel : MonoBehaviour {

    [SerializeField] private GameObject playerLobbyItem;
    [SerializeField] private GameObject playerLobbyList;

	public GameObject AddPlayerToLobby(string playerName)
    {
        return Instantiate(playerLobbyItem, playerLobbyList.transform, false);
        //_playerLobbyItem.transform.localScale = Vector3.one;

        //_playerLobbyItem.transform.Find("PlayerName_Text").GetComponent<Text>().text = playerName;
        //_playerLobbyItem.transform.Find("PlayerStatus_Text").GetComponent<Text>().text = "Not Ready";
    }
}
