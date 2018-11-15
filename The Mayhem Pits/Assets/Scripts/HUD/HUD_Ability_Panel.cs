using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Ability_Panel : MonoBehaviour {

    [SerializeField] private GameObject horizontalView;
    [SerializeField] private GameObject hudElementPrefab;
    List<GameObject> abilityObjects = new List<GameObject> ();
    List<Ability> robotAbilities = new List<Ability> ();
    List<Ability> localAbilities = new List<Ability> ();
    
    private int selectionIndex = 0;
    private float inputHold = 0.5f;

    private void Update ()
    {
        CheckCooldowns ();
    }

    public void AddAbility (Ability ability, List<Ability> abilities)
    {
        for (int i = 0; i < localAbilities.Count; i++)
        {
            if(localAbilities[i].ID == ability.ID)
            {
                //Debug.Log ( "Already contains ability " + ability.name );
                localAbilities[i].IncreaseUses ( 1 );
                return;
            }
        }             

        GameObject go = Instantiate ( hudElementPrefab );
        go.transform.SetParent ( horizontalView.transform );
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = Vector3.zero;
        go.transform.localEulerAngles = Vector3.zero;
        go.transform.name = ability.AbilityName;
        go.transform.Find ( "AbilitySprite" ).GetComponent<Image> ().sprite = ability.Sprite;
        go.transform.Find ( "AbilityName_Text" ).GetComponent<Text> ().text = ability.AbilityName;
        go.transform.Find ( "AbilityName_Text" ).gameObject.SetActive ( false );
        go.transform.Find ( "ControllerButton_Image" ).gameObject.SetActive ( false );
        abilityObjects.Add ( go );

        if (!ability.HasSpecificInput)
            Select ( selectionIndex );
        else
        {
            go.transform.Find ( "AbilityName_Text" ).gameObject.SetActive ( true );
            go.transform.Find ( "ControllerButton_Image" ).gameObject.SetActive ( true );
            go.transform.Find ( "ControllerButton_Image" ).GetComponent<Image> ().sprite = ability.SpecificInputSprite;
        }

        //Debug.Log ( "Added ability " + ability.AbilityName );
        localAbilities.Add ( ability );
        this.robotAbilities = abilities;
    }    

    public void RemoveAbility(Ability ability)
    {
        for (int i = 0; i < robotAbilities.Count; i++)
        {
            if (ability == robotAbilities[i])
            {
                int index = i;

                Deselect ( selectionIndex );
                selectionIndex = 0;
                Select ( selectionIndex );

                Destroy ( abilityObjects[index] );
                abilityObjects.RemoveAt ( index );
                robotAbilities.RemoveAt ( index );
            }
        }     
    }

    public void OnTap ()
    {        
        Deselect ( selectionIndex );

        do
        {
            selectionIndex++;
            if (selectionIndex >= abilityObjects.Count) selectionIndex = 0;
        } while (robotAbilities[selectionIndex].HasSpecificInput);

      
        Select ( selectionIndex );
    }

    private bool activatedCurrent = false;

    public bool OnHold (float holdTime)
    {
        if (activatedCurrent) { return false; } 
        if (robotAbilities[selectionIndex].isOnCooldown) { return false; }

        abilityObjects[selectionIndex].transform.Find ( "SelectionSprite" ).GetComponent<Image> ().fillAmount = holdTime / inputHold;
        if (holdTime >= inputHold)
        {
            activatedCurrent = true;
            robotAbilities[selectionIndex].Activate ();
            abilityObjects[selectionIndex].transform.Find ( "SelectionSprite" ).GetComponent<Image> ().fillAmount = 0.0f;
        }

        return true;
    }

    public void OnReleaseHold ()
    {        
        abilityObjects[selectionIndex].transform.Find ( "SelectionSprite" ).GetComponent<Image> ().fillAmount = 0.0f;
        activatedCurrent = false;
    }

    private void Select(int index)
    {
        if (abilityObjects[index] == null) return;
        abilityObjects[index].transform.Find ( "AbilityName_Text" ).gameObject.SetActive ( true );
        abilityObjects[index].transform.Find ( "ControllerButton_Image" ).gameObject.SetActive ( true );
    }

    private void Deselect(int index)
    {
        if (abilityObjects[index] == null) return;
        abilityObjects[index].transform.Find ( "AbilityName_Text" ).gameObject.SetActive ( false );
        abilityObjects[index].transform.Find ( "ControllerButton_Image" ).gameObject.SetActive ( false );
    }

    private void CheckCooldowns ()
    {
        for (int i = 0; i < abilityObjects.Count; i++)
        {
            if (localAbilities[i].isOnCooldown)
            {
                abilityObjects[i].transform.Find ( "SelectionSprite" ).GetComponent<Image> ().fillAmount = (localAbilities[i].currCooldown / localAbilities[i].CooldownTime);
                abilityObjects[i].transform.Find ( "ControllerButton_Image" ).GetComponent<Image> ().color = new Color ( 1.0f, 0.0f, 0.0f );
                if (localAbilities[i].currCooldown <= 0.05)
                {
                    abilityObjects[i].transform.Find ( "SelectionSprite" ).GetComponent<Image> ().fillAmount = 0.0f;
                    abilityObjects[i].transform.Find ( "ControllerButton_Image" ).GetComponent<Image> ().color = new Color ( 0.0f, 1.0f, 0.0f );
                }
            }

            //Debug.Log ( i );
            Text text = abilityObjects[i].transform.Find ( "UsesLeft_Panel" ).Find ( "UsesLeft_Text" ).GetComponent<Text> ();

            if (localAbilities[i].GetUsesLeft >= 1)
            {
                text.text = localAbilities[i].GetUsesLeft.ToString ( "00" );
                text.fontSize = 18;
            }
            else
            {
                text.text = "∞";
                text.fontSize = 25;
            }
        }
    }
}
