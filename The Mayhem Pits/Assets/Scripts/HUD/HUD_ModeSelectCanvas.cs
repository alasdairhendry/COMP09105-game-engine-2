﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HUD_ModeSelectCanvas : MonoBehaviour {

    [SerializeField] private bool DEBUG_ALWAYSSELECT = true;

    const string prefKey = "ClientGraphicsMode";
    private bool hasSelected = false;

    private void Start()
    {
        if (DEBUG_ALWAYSSELECT) return;
        if (PlayerPrefs.HasKey(prefKey))
        {
            if (PlayerPrefs.GetString(prefKey) == "Normal")
            {
                ClientMode.Instance.SetModeNormal();
            }
            else
            {
                ClientMode.Instance.SetModeVR();
            }

            hasSelected = true;
            SceneManager.LoadScene("Menu");
        }
    }

    void Update () {
        if (hasSelected) return;
        CheckInput();
	}

    private void CheckInput()
    {
        if (Input.GetButtonDown("XBO_A"))
        {
            hasSelected = true;
            PlayerPrefs.SetString(prefKey, "Normal");
            ClientMode.Instance.SetModeNormal();
            SceneManager.LoadScene("Menu");
        }
        else if (Input.GetButtonDown("XBO_B"))
        {
            hasSelected = true;
            PlayerPrefs.SetString(prefKey, "VR");
            ClientMode.Instance.SetModeVR();
            SceneManager.LoadScene("Menu");
        }
    }
}
