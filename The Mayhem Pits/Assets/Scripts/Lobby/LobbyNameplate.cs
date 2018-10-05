using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyNameplate : MonoBehaviour {

    [SerializeField] private NetworkLobbyPlayer player;
    private TextMesh textMesh;

	// Use this for initialization
	void Start () {
        textMesh = GetComponent<TextMesh>();

	}
	
	// Update is called once per frame
	void Update () {
        //textMesh.text = player.GetPlayerName;
    }
}
