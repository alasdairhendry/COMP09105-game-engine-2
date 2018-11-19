using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientMode : MonoBehaviour {

    public static ClientMode Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {        
        if (mode == Mode.Normal/* && SceneManager.GetActiveScene().name != "NetworkConnection" && SceneManager.GetActiveScene().name != "DatabaseConnection" && SceneManager.GetActiveScene().name != "ModeSelect"*/)
            UnityEngine.XR.XRSettings.enabled = false;
    }

    public enum Mode { Normal, VR }
    [SerializeField] private Mode mode = Mode.Normal;
    public Mode GetMode { get { return mode; } }    

	public void SetModeNormal()
    {        
        mode = Mode.Normal;
        UnityEngine.XR.XRSettings.enabled = false;
    }

    public void SetModeVR()
    {
        mode = Mode.VR;
        UnityEngine.XR.XRSettings.enabled = true;
    } 
}
