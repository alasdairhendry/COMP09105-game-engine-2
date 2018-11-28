using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotOverloads : MonoBehaviourPunCallbacks {

    private HUD_Overloads_Panel overloadPanel;
    private List<Overload> overloads = new List<Overload> ();
    public System.Action<List<Overload>> OnOverloadsChanged;

    private int selectionIndex = 0;

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
        overloadPanel = FindObjectOfType<HUD_Overloads_Panel> ();
        CreateInitialAbilities ();
    }

    // Update is called once per frame
    void Update ()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected) return;        
        CheckInput ();
    }

    public void SetAllowUse(bool state)
    {
        allowUse = state;
    }

    private void CheckInput ()
    {
        if (Input.GetButtonUp ( "XBO_X" ))
        {
            if (currentDelay < inputDelay)
            {
                // User has "tapped"
                currentDelay = 0.0f;
                currentHold = 0.0f;
                overloadPanel.OnTap ();
                isInput = false;
            }
            else
            {
                // User has "held"
                currentDelay = 0.0f;
                currentHold = 0.0f;
                overloadPanel.OnReleaseHold ();
                isInput = false;
            }
        }

        if (Input.GetButton ( "XBO_X" ))
        {
            isInput = true;
            currentDelay += Time.deltaTime;

            if (currentDelay >= inputDelay)
            {
                if (allowUse)
                {
                    currentHold += Time.deltaTime;

                    if (!overloadPanel.OnHold(currentHold))
                    {
                        currentHold = 0.0f;
                    }
                }
            }
        }

        if (Input.GetButton ( "XBO_B" ))
        {
            overloadPanel.OnCancelAbility ();
        }
    }

    [SerializeField] private GameObject impulsePrefab;

    private void CreateInitialAbilities ()
    {
        //return;
        //AddAbility(impulsePrefab);
        //AddAbility ( impulsePrefab );
        //AddAbility ( impulsePrefab );

        //AddAbility ( impulsePrefab );
        //AddAbility ( impulsePrefab );
        //AddAbility ( impulsePrefab );
    }

    public bool AddAbility(GameObject prefab)
    {
        if (overloadPanel.IsFull) { return false; }

        GameObject go = PhotonNetwork.Instantiate ( prefab.name, Vector3.zero, Quaternion.identity );            
        go.transform.SetParent ( this.transform.Find ( "Overloads" ) );
        go.transform.localPosition = Vector3.zero;
        Overload overload = go.GetComponent<Overload> ();

        overloads.Add ( overload );
        overload.SetLocalRobot ( this.gameObject );
        overloadPanel.AddAbility ( overload, overloads );

        return true;
    }
}
