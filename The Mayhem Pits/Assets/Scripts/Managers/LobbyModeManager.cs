using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyModeManager : MonoBehaviour {

    [SerializeField] private List<GameObject> normalLobbyObjects = new List<GameObject>();
    [SerializeField] private List<GameObject> vrLobbyObjects = new List<GameObject>();

    // Use this for initialization
    void Start () {
        CreateLobbyObjects();
	}

    private void CreateLobbyObjects()
    {
        if (ClientMode.singleton.GetMode == ClientMode.Mode.Normal)
        {
            foreach (GameObject obj in normalLobbyObjects)
            {
                obj.SetActive(true);
            }
            UnityEngine.XR.XRSettings.enabled = false;
        }
        else if (ClientMode.singleton.GetMode == ClientMode.Mode.VR)
        {
            foreach (GameObject obj in vrLobbyObjects)
            {
                obj.SetActive(true);
            }
        }
    }
}
