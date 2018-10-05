using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HUD_MenuController : MonoBehaviour {
    
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

}
