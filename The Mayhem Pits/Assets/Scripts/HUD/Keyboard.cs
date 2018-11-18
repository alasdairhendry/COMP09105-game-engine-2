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

    private Button showButton;
    private bool show = false;

    private bool active = false;

    private string currentInput = "";

    private System.Action<string> OnFinish;
    private System.Action OnCancel;

    private int maxCharacters = 0;
    private bool hiddenMode = false;

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

            if(t.text.ToLower() == "show")
            {
                showButton = b;

                b.onClick.AddListener ( () =>
                  {
                      show = !show;
                      if (show) { showButton.GetComponentInChildren<Text> ().text = "hide"; }
                      else { showButton.GetComponentInChildren<Text> ().text = "show"; }
                      UpdateText ();
                  } );

                continue;
            }

            if (t.text.ToLower () == "cancel")
            {
                b.onClick.AddListener ( () => { CancelInput (); } );
                continue;
            }

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
        if (hiddenMode && !show)
        {
            string s = "";

            for (int i = 0; i < currentInput.Length; i++)
            {
                s += "*";
            }

            visualInput.text = s;
        }
        else visualInput.text = currentInput;
    }

    private void CancelInput()
    {
        if (OnCancel != null)
            OnCancel();

        OnFinish = null;
        OnCancel = null;
        Close();
    }
    
    private void FinishInput ()
    {
        if (OnFinish != null)
            OnFinish (currentInput);

        OnFinish = null;
        OnCancel = null;
        Close ();
    }

    public void Open (Mode mode, System.Action<string> onFinish, System.Action onCancel, HUDSelectionGroup returnGroup, int maxCharacters, bool hidden = false)
    {
        OnFinish = onFinish;
        OnCancel = onCancel;

        this.returnGroup = returnGroup;
        this.maxCharacters = maxCharacters;
        this.hiddenMode = hidden;

        active = true;
        selectionGroup.SetActiveGroup ();
        SetBody ();
        SwitchMode ( mode );
        currentInput = "";

        show = false;
        showButton.GetComponentInChildren<Text> ().text = "show";

        if (hidden) showButton.gameObject.SetActive ( true );
        else showButton.gameObject.SetActive ( false );

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
