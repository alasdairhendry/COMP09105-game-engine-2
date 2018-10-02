using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HUD_ModeSelectCanvas : MonoBehaviour {

    private bool hasSelected = false;
	
	void Update () {
        if (hasSelected) return;
        CheckInput();
	}

    private void CheckInput()
    {
        if (Input.GetButtonDown("XBO_A"))
        {
            hasSelected = true;
            ClientMode.singleton.SetModeNormal();
            SceneManager.LoadScene("Menu");
        }
        else if (Input.GetButtonDown("XBO_B"))
        {
            hasSelected = true;
            ClientMode.singleton.SetModeVR();
            SceneManager.LoadScene("Menu");
        }
    }
}
