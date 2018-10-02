using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class NetworkLobbyManager : MonoBehaviour {

    private NetworkManager networkManager;

    private int matchListTargetIndex = 0;
    private List<MatchInfoSnapshot> matchesList = new List<MatchInfoSnapshot>();

    [SerializeField] private Text lobbyStatusText;

    private void Start()
    {
        networkManager = NetworkController.singleton;

        if (networkManager.matchMaker == null)
            networkManager.StartMatchMaker();
    }

    private void OnPlayerConnected(NetworkMessage netMsg)
    {
        Debug.Log(netMsg + ": A player has connected");
    }

    public void FindMatch()
    {
        if (networkManager.matchMaker == null)
            networkManager.StartMatchMaker();

        networkManager.matchMaker.ListMatches(0, 10, "", true, 0, 0, OnMatchListReturned);
        lobbyStatusText.text = "Searching For Suitable Match...";        
    }

    private void OnMatchListReturned(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        if(!success)
        {     
            CreateMatch();
            return;
        }

        for (int i = 0; i < matches.Count; i++)
        {
            if (matches[i].currentSize == 0)
                matches.RemoveAt(i);
        }

        if (matches.Count <= 0)
        {
            CreateMatch();
            return;
        }

        lobbyStatusText.text = "Match List Returned: Found " + matches.Count.ToString("00") + " Matches.";
        matchListTargetIndex = 0;
        matchesList = matches;
        TryJoinMatchList();
    }

    private void TryJoinMatchList()
    {
        if(matchListTargetIndex >= matchesList.Count)
        {
            lobbyStatusText.text = "No Eligible Matches Found.";
            CreateMatch();
            return;
        }

        lobbyStatusText.text = "Checking Match: " + matchListTargetIndex.ToString("00") + " / " + matchesList.Count.ToString("00") + "...";

        MatchInfoSnapshot targetMatch = matchesList[matchListTargetIndex];
        networkManager.matchMaker.JoinMatch(targetMatch.networkId, "", "", "", 0, 0, OnMatchListJoined);
    }

    private void OnMatchListJoined(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (!success)
        {
            Debug.Log("OnMatchListJoined_Failure");
            matchListTargetIndex++;
            TryJoinMatchList();
            return;
        }

        Debug.Log("OnMatchListJoined_Success");
        networkManager.OnMatchJoined(success, extendedInfo, matchInfo);
        lobbyStatusText.text = "Joining Match...";
    }

    private void CreateMatch()
    {
        lobbyStatusText.text = "Creating New Match...";
        networkManager.matchMaker.CreateMatch("Match-" + Random.value, 4, true, "", "", "", 0, 0, networkManager.OnMatchCreate);
    }
}
