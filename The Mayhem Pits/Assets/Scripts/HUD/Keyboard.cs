using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Keyboard : MonoBehaviour {

    public enum Mode { lower, shift, caps };
    private Mode mode = Mode.lower;

    [SerializeField] private GameObject body;
    [SerializeField] private HUDSelectionGroup selectionGroup;
    [SerializeField] private HUDSelectionGroup returnGroup;
    [SerializeField] private Text visualInput;

    private List<Button> buttons = new List<Button> ();
    private bool active = false;

    private string currentInput = "";
    private System.Action<string> OnFinish;
    private int maxCharacters = 0;

    private void Start ()
    {
        Debug.Log ( "Start" );
        GetButtons ();
        SetButtonListeners ();
        Close ();
    }

    private void GetButtons ()
    {
        buttons = GetComponentsInChildren<Button> ( true ).ToList ();
    }

    private void SetButtonListeners ()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            Text t = buttons[i].GetComponentInChildren<Text> ();
            Button b = buttons[i];

            if (t.text.ToLower() == "del")
            {                
                b.onClick.AddListener ( () => { Delete(); UpdateText (); } );
                continue;
            }

            if (t.text.ToLower() == "shift")
            {
                b.onClick.AddListener ( () => { IterateMode (); } );
                continue;
            }

            if (t.text.ToLower() == "space")
            {
                b.onClick.AddListener ( () => { Add ( " " ); } );
                continue;
            }

            if (t.text.ToLower() == "enter")
            {
                b.onClick.AddListener ( () => { FinishInput (); } );
                continue;
            }

            b.onClick.AddListener ( () => { Add ( t.text ); UpdateText (); } );
        }
    }

    private void SwitchMode(Mode mode)
    {
        this.mode = mode;
        SetModeConditions ();
    }

    private void IterateMode ()
    {
        switch (mode)
        {
            case Mode.lower:
                mode = Mode.shift;
                break;

            case Mode.shift:
                mode = Mode.caps;
                break;

            case Mode.caps:
                mode = Mode.lower;
                break;
        }

        SetModeConditions ();
    }

    private void SetModeConditions ()
    {
        switch (mode)
        {
            case Mode.lower:
                for (int i = 0; i < buttons.Count; i++)
                {
                    Text t = buttons[i].GetComponentInChildren<Text> ();
                    t.text = t.text.ToLower ();

                    if (t.text == "shift") t.text = "shift";
                }
                break;

            case Mode.shift:
                for (int i = 0; i < buttons.Count; i++)
                {
                    Text t = buttons[i].GetComponentInChildren<Text> ();
                    t.text = t.text.ToUpper ();
                    if (t.text == "SHIFT") t.text = "Shift";
                }
                break;

            case Mode.caps:
                for (int i = 0; i < buttons.Count; i++)
                {
                    Text t = buttons[i].GetComponentInChildren<Text> ();
                    t.text = t.text.ToUpper ();
                    if (t.text == "SHIFT") t.text = "SHIFT";
                }
                break;
        }
    }

    private void Add(string s)
    {
        if (currentInput.Length >= maxCharacters) return;

        switch (mode)
        {
            case Mode.lower:
                currentInput += s.ToLower ();
                return;

            case Mode.shift:
                currentInput += s.ToUpper ();
                mode = Mode.lower;
                SetModeConditions ();
                return;

            case Mode.caps:
                currentInput += s.ToUpper ();
                return;
        }
    }

    private void Delete()
    {
        if (currentInput.Length > 0)
            currentInput = currentInput.Remove(currentInput.Length - 1, 1);
    }

    private void UpdateText ()
    {
        visualInput.text = currentInput;
    }

    private void FinishInput ()
    {
        if (OnFinish != null)
            OnFinish (currentInput);

        OnFinish = null;
        Close ();
    }

    public void Open (System.Action<string> onFinish, int maxCharacters)
    {
        OnFinish = onFinish;
        this.maxCharacters = maxCharacters;
        active = true;
        selectionGroup.SetActiveGroup ();
        SetBody ();
        SwitchMode ( Mode.shift );
        currentInput = "";
        UpdateText();
    }

    //public void Open ()
    //{
    //    Debug.Log ( "OpenStart" );
    //    active = true;
    //    //selectionGroup.SetActiveGroup ();
    //    SetBody ();
    //    Debug.Log ( "OpenEnd" );
    //}

    private void SetBody ()
    {
        body.SetActive ( active );
    }

    public void Close ()
    {
        active = false;
        returnGroup.SetActiveGroup ();
        SetBody ();
    }
}
