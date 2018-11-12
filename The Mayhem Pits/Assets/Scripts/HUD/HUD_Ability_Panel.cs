using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Ability_Panel : MonoBehaviour {

    [SerializeField] private GameObject horizontalView;
    [SerializeField] private GameObject hudElementPrefab;
    List<GameObject> abilityObjects = new List<GameObject> ();
    List<Ability> abilities = new List<Ability> ();
    
    private int selectionIndex = 0;
    private float inputHold = 0.5f;

    private void Update ()
    {
        CheckCooldowns ();
    }

    public void AddAbility (Ability ability, List<Ability> abilities)
    {
        GameObject go = Instantiate ( hudElementPrefab );
        go.transform.SetParent ( horizontalView.transform );
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = Vector3.zero;
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

        this.abilities = abilities;
    }

    public void RemoveAbility(Ability ability)
    {
        for (int i = 0; i < abilityObjects.Count; i++)
        {
            if (ability == abilityObjects[i].GetComponent<Ability> ())
            {
                Destroy ( abilityObjects[i] );
                abilityObjects.RemoveAt ( i );
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
        } while (abilities[selectionIndex].HasSpecificInput);

      
        Select ( selectionIndex );
    }

    private bool activatedCurrent = false;

    public bool OnHold (float holdTime)
    {
        if (activatedCurrent) { return false; } 
        if (abilities[selectionIndex].isOnCooldown) { return false; }

        abilityObjects[selectionIndex].transform.Find ( "SelectionSprite" ).GetComponent<Image> ().fillAmount = holdTime / inputHold;
        if (holdTime >= inputHold)
        {
            activatedCurrent = true;
            abilities[selectionIndex].Activate ();
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
        abilityObjects[index].transform.Find ( "AbilityName_Text" ).gameObject.SetActive ( true );
        abilityObjects[index].transform.Find ( "ControllerButton_Image" ).gameObject.SetActive ( true );
    }

    private void Deselect(int index)
    {
        abilityObjects[index].transform.Find ( "AbilityName_Text" ).gameObject.SetActive ( false );
        abilityObjects[index].transform.Find ( "ControllerButton_Image" ).gameObject.SetActive ( false );
    }

    private void CheckCooldowns ()
    {
        for (int i = 0; i < abilities.Count; i++)
        {
            if (abilities[i].isOnCooldown)
            {
                abilityObjects[i].transform.Find ( "SelectionSprite" ).GetComponent<Image> ().fillAmount = (abilities[i].currCooldown / abilities[i].CooldownTime);
                abilityObjects[i].transform.Find ( "ControllerButton_Image" ).GetComponent<Image> ().color = new Color ( 1.0f, 0.0f, 0.0f );
                if (abilities[i].currCooldown <= 0.05)
                {
                    abilityObjects[i].transform.Find ( "SelectionSprite" ).GetComponent<Image> ().fillAmount = 0.0f;
                    abilityObjects[i].transform.Find ( "ControllerButton_Image" ).GetComponent<Image> ().color = new Color ( 0.0f, 1.0f, 0.0f );
                }
            }
        }
    }
}
