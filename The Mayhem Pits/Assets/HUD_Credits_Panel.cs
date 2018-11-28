using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HUD_Credits_Panel : MonoBehaviour {

    float counter = 0.0f;

    private void Update ()
    {
        counter += Time.deltaTime;

        if(counter>= 2.5f)
        {
            SceneManager.LoadScene ( "NetworkConnection" );
        }
    }
}
