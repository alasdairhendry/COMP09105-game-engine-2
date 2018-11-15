using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotAbilities : MonoBehaviourPunCallbacks
{
    private HUD_Ability_Panel abilityPanel;
    private List<Ability> abilities = new List<Ability> ();
    public System.Action<List<Ability>> OnAbilitiesChanged;

    [SerializeField] private GameObject ability_FlipLeftPrefab;
    [SerializeField] private GameObject ability_FlipRightPrefab;
    [SerializeField] private GameObject ability_BoostPrefab;

    private int abilitySelectionIndex = 0;

    private bool isInput = false;

    private float inputDelay = 0.20f;
    private float currentDelay = 0.0f;

    //private float inputHold = 0.5f;
    private float currentHold = 0.0f;

    private bool allowUse = false;

    // Use this for initialization
    void Start ()
    {
        //return
        if (!photonView.IsMine && PhotonNetwork.IsConnected) return;
        abilityPanel = FindObjectOfType<HUD_Ability_Panel> ();        
        CreateInitialAbilities ();
	}
	
	// Update is called once per frame
	void Update () {
        if (!photonView.IsMine && PhotonNetwork.IsConnected) return;        
        CheckInput ();
	}

    public void SetAllowUse(bool state)
    {
        allowUse = state;
    }

    private void CheckInput ()
    {
        if (Input.GetButtonUp ( "XBO_Y" ))
        {
            if (currentDelay < inputDelay)
            {
                // User has "tapped"
                currentDelay = 0.0f;
                currentHold = 0.0f;
                abilityPanel.OnTap ();                
                isInput = false;
            }
            else
            {
                // User has "held"
                currentDelay = 0.0f;
                currentHold = 0.0f;
                abilityPanel.OnReleaseHold ();
                isInput = false;
            }
        }

        if (Input.GetButton ( "XBO_Y" ))
        {
            isInput = true;
            currentDelay += Time.deltaTime;

            if(currentDelay>= inputDelay)
            {
                if (allowUse)
                {
                    currentHold += Time.deltaTime;
                    if (!abilityPanel.OnHold(currentHold))
                    {
                        currentHold = 0.0f;
                    }
                }
            }
        }
    }

    private void CreateInitialAbilities ()
    {     
        AddAbility ( ability_FlipLeftPrefab );
        AddAbility ( ability_FlipRightPrefab );
        AddAbility ( ability_BoostPrefab );
        AddAbility ( ability_BoostPrefab );
        AddAbility ( ability_BoostPrefab );
        AddAbility ( ability_BoostPrefab );
    }

    public void AddAbility (GameObject prefab)
    {
        GameObject go = Instantiate ( prefab, Vector3.zero, Quaternion.identity, this.transform.Find ( "Abilities" ) );
        Ability ability = go.GetComponent<Ability> ();

        abilities.Add ( ability );
        ability.SetTargetRobot ( this.gameObject );
        abilityPanel.AddAbility ( ability, abilities );   
    }

    //public void ActivateAbility(string abilityName)
    //{
    //    for (int i = 0; i < abilities.Count; i++)
    //    {
    //        if(abilityName == abilities[i].AbilityName)
    //        {
    //            abilities[i].Activate ();
    //            return;
    //        }
    //    }
    //}
}
