using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HUDSelectionGroup : MonoBehaviour {

    [SerializeField] protected List<Button> children = new List<Button> ();
    [SerializeField] protected bool autoDetectChildren = false;

    [SerializeField] protected bool isActiveGroup = false;
    public bool IsActiveGroup { get { return isActiveGroup; } }

    protected bool setActiveFrameYield = false;

    protected int index;
    protected int previousIndex;

    protected bool movedHorizontal = false;
    protected bool movedVertical = false;
	
	protected virtual void Start () {
        if (autoDetectChildren)
            children = GetComponentsInChildren<Button> (true).ToList ();

        if (!children[0].IsActive ()) IncrementIndex ();

        if (isActiveGroup) HUDSelectionManager.SetActiveGroup ( this );
	}
		
	protected virtual void Update () {
        if (!isActiveGroup) return;
        if (setActiveFrameYield)
        {
            setActiveFrameYield = false;
            return;
        }
        DetectInput ();
	}

    public virtual void SetActiveGroup ()
    {
        HUDSelectionManager.SetActiveGroup ( this );
    }

    public virtual void Enable ()
    {
        if (!children[index].IsActive ()) IncrementIndex ();
        SelectIndex ();
        isActiveGroup = true;
        setActiveFrameYield = true;
    }

    public virtual void Disable ()
    {
        DeselectIndex ( index );
        isActiveGroup = false;
    }

    protected virtual void DetectInput ()
    {
        if (Input.GetAxis ( "XBO_DPAD_Vertical" ) != 0)
        {
            if (!movedVertical)
            {
                movedVertical = true;

                // Change Vertically
                if (Input.GetAxis ( "XBO_DPAD_Vertical" ) < 0) IncrementIndex ();
                else DecrementIndex ();

            }
        }
        else
        {
            if (movedVertical)
            {
                movedVertical = false;
            }
        }

        if (Input.GetAxis ( "XBO_DPAD_Horizontal" ) != 0)
        {
            if (!movedHorizontal)
            {
                movedHorizontal = true;

                // Change horizontally

            }
        }
        else
        {
            if (movedHorizontal)
            {
                movedHorizontal = false;
            }
        }

        if (Input.GetButtonUp ( "XBO_A" ))
        {
            InvokeIndex ();
        }
    }

    protected virtual void OnMoveVertical(float direction)
    {

    }

    protected virtual void OnMoveHorizontal (float direction)
    {

    }

    protected virtual void InvokeIndex ()
    {
        Debug.Log ( "Invoking on " + children[index].name );
        children[index].onClick.Invoke ();
        children[index].GetComponent<Animator> ().SetTrigger ( "Pressed" );
    }

    protected virtual void SelectIndex ()
    {
        DeselectIndex ( previousIndex );
        children[index].GetComponent<Animator> ().SetTrigger ( "Highlighted" );
    }

    protected virtual void DeselectIndex (int index)
    {
        children[index].GetComponent<Animator> ().SetTrigger ( "Normal" );
    }

    protected virtual void IncrementIndex ()
    {
        previousIndex = index;
        do
        {

            index++;
            if (index >= children.Count) index = 0;

        } while (!children[index].IsActive ());
        
        SelectIndex ();
    }

    protected virtual void ChangeIndex(int newIndex)
    {
        previousIndex = index;
        index = newIndex;
        SelectIndex ();
    }

    protected virtual void DecrementIndex ()
    {
        previousIndex = index;
        do
        {

            index--;
            if (index < 0) index = children.Count - 1;

        } while (!children[index].IsActive ());

        SelectIndex ();
    }
}
