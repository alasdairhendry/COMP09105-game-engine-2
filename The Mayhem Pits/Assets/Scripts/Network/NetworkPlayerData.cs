using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayerData {

    private short playerID;
    private string playerName;
    private NetworkLobbyPlayer lobbyPlayer;

    public NetworkPlayerData(short playerID, string playerName, NetworkLobbyPlayer lobbyPlayer)
    {
        this.playerID = playerID;
        this.playerName = playerName;
        this.lobbyPlayer = lobbyPlayer;
    }
}
