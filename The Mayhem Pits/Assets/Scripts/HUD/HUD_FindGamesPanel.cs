using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HUD_FindGamesPanel : MonoBehaviour {

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Toggle()
    {
        anim.SetBool("Active", !anim.GetBool("Active"));        
    }

    public void Hide()
    {
        anim.SetBool("Active", false);
    }
}
