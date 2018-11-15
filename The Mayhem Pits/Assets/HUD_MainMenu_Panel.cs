using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD_MainMenu_Panel : MonoBehaviour {

    [SerializeField] private GameObject body;
    [SerializeField] private GameObject mainButtons;
    [SerializeField] private GameObject findGameButtons;

	public void Open()
    {
        body.SetActive(true);
        mainButtons.SetActive(true);
        findGameButtons.SetActive(false);
        mainButtons.GetComponent<HUDSelectionGroup>().SetActiveGroup();
    }

    public void Close()
    {
        body.SetActive(false);
        mainButtons.SetActive(false);
        findGameButtons.SetActive(false);
    }
}
