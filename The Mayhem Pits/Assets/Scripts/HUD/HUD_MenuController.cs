using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HUD_MenuController : MonoBehaviour {

    [SerializeField] private GameObject findGame_Button;
    [SerializeField] private GameObject myRobot_Button;
    [SerializeField] private GameObject leaveGame_Button;
    public NetworkPlayer localPlayer;

    private void Start()
    {
        NetworkManager.singleton.onJoinedRoom += OnJoinedRoom;
        NetworkManager.singleton.onLeftRoom += OnLeftRoom;
    }

    private void OnJoinedRoom()
    {
        findGame_Button.SetActive(false);
        leaveGame_Button.SetActive(true);
        myRobot_Button.SetActive ( false );
    }

    private void OnLeftRoom()
    {
        leaveGame_Button.SetActive(false);
        myRobot_Button.SetActive ( true );
        findGame_Button.SetActive(true);
    }

    public void OnClick_FindGame(int selectedRoomSize)
    {
        NetworkManager.singleton.FindGame((byte)selectedRoomSize);
    }

    public void OnClick_LeaveLobby()
    {        
        NetworkManager.singleton.DisconnectFromRoom();
    }
    
    public void OnClick_MyRobot()
    {
        SceneManager.LoadScene("MyRobot");
    }

    public void OnClick_Settings()
    {
        SceneManager.LoadScene("MyRobot");
    }

    public void OnClick_Exit()
    {
        Application.Quit();
    }

    private void OnDestroy()
    {
        NetworkManager.singleton.onJoinedRoom -= OnJoinedRoom;
        NetworkManager.singleton.onLeftRoom -= OnLeftRoom;
    }

}
