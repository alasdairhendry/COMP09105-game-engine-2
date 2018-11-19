using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Overloads_Panel : MonoBehaviour {

    [SerializeField] private GameObject horizontalView;
    [SerializeField] private GameObject hudElementPrefab;
    List<GameObject> overloadObjects = new List<GameObject> ();
    List<Overload> overloads = new List<Overload> ();

    public bool IsFull { get { return overloads.Count >= 3; } }

    private int selectionIndex = 0;
    private float inputHold = 0.5f;

    private void Update ()
    {
        //CheckCooldowns ();
    }

    public void AddAbility (Overload overload, List<Overload> overloads)
    {       
        GameObject go = Instantiate ( hudElementPrefab );
        go.transform.SetParent ( horizontalView.transform );
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = Vector3.zero;
        go.transform.localEulerAngles = Vector3.zero;
        go.transform.name = overload.OverloadName;
        go.transform.Find ( "AbilitySprite" ).GetComponent<Image> ().sprite = overload.Sprite;
        go.transform.Find ( "AbilityName_Text" ).GetComponent<Text> ().text = overload.OverloadName;
        go.transform.Find ( "AbilityName_Text" ).gameObject.SetActive ( false );
        go.transform.Find ( "ControllerButton_Image" ).gameObject.SetActive ( false );
        go.transform.SetAsFirstSibling ();
        overloadObjects.Add ( go );
        
        this.overloads = overloads;

        if (overloadObjects.Count == 1)
        {
            selectionIndex = 0;
            Select(selectionIndex);
        }
        else
        {
            Deselect(selectionIndex);
            selectionIndex = overloadObjects.IndexOf(go);
            Select(selectionIndex);
        }
    }

    public void RemoveAbility (Ability ability)
    {
        for (int i = 0; i < overloadObjects.Count; i++)
        {
            if (ability == overloadObjects[i].GetComponent<Ability> ())
            {
                Destroy ( overloadObjects[i] );
                overloadObjects.RemoveAt ( i );
            }
        }
    }

    public void OnTap ()
    {
        if (overloads.Count <= 0) return;
        if (overloadIsActive)
        {
            overloads[selectionIndex].Use ();
            return;
        }

        Deselect ( selectionIndex );
        selectionIndex++;
        if (selectionIndex >= overloadObjects.Count) selectionIndex = 0;        

        Select ( selectionIndex );
    }

    private bool activatedCurrent = false;

    private bool overloadIsActive = false;
    private Overload activatedOverload;

    public void OnCancelAbility ()
    {
        if (overloadIsActive)
        {
            if (activatedOverload != null)
            {
                overloadObjects[selectionIndex].transform.Find ( "Cancel_Panel" ).gameObject.GetComponent<CanvasGroup> ().alpha = 0.0f;
                activatedOverload.Cancel ();
                overloadIsActive = false;
                activatedOverload = null;
            }
        }
    }

    public void OnFinishAbility ()
    {
        int index = overloads.IndexOf ( activatedOverload );

        overloadObjects[selectionIndex].transform.Find ( "Cancel_Panel" ).gameObject.GetComponent<CanvasGroup> ().alpha = 0.0f;
        Destroy ( overloadObjects[index] );
        overloads.RemoveAt ( index );
        overloadObjects.RemoveAt ( index );

        selectionIndex = 0;
        Select ( selectionIndex );
        overloadIsActive = false;
        activatedOverload = null;
    }

    public bool OnHold (float holdTime)
    {
        if (overloads.Count <= 0) return false;
        if (activatedCurrent) { return false; }

        if (overloadIsActive)
        {
            return false;
        }

        overloadObjects[selectionIndex].transform.Find ( "SelectionSprite" ).GetComponent<Image> ().fillAmount = holdTime / inputHold;
        if (holdTime >= inputHold)
        {
            activatedCurrent = true;

            Overload.OverloadActivationStatus s = overloads[selectionIndex].Activate ();

            if (s.status)
            {
                overloadIsActive = true;
                activatedOverload = overloads[selectionIndex];
                overloadObjects[selectionIndex].transform.Find ( "Cancel_Panel" ).gameObject.GetComponent<CanvasGroup> ().alpha = 1.0f;
            }

            overloadObjects[selectionIndex].transform.Find ( "SelectionSprite" ).GetComponent<Image> ().fillAmount = 0.0f;
        }

        return true;
    }

    public void OnReleaseHold ()
    {
        if (overloads.Count <= 0) return;
        overloadObjects[selectionIndex].transform.Find ( "SelectionSprite" ).GetComponent<Image> ().fillAmount = 0.0f;
        activatedCurrent = false;
    }

    private void Select (int index)
    {
        if (overloads.Count <= 0) return;
        overloadObjects[index].transform.Find ( "AbilityName_Text" ).gameObject.SetActive ( true );
        overloadObjects[index].transform.Find ( "ControllerButton_Image" ).gameObject.SetActive ( true );
    }

    private void Deselect (int index)
    {
        if (overloads.Count <= 0) return;
        if (overloadObjects[index] == null) return;

        overloadObjects[index].transform.Find ( "AbilityName_Text" ).gameObject.SetActive ( false );
        overloadObjects[index].transform.Find ( "ControllerButton_Image" ).gameObject.SetActive ( false );
    }  
}
