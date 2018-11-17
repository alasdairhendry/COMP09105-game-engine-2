using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HUDSelectionGroup : MonoBehaviour {

    public enum ChildrenType { Animation, ColourTint }
    [SerializeField] protected ChildrenType childrenType;
    [SerializeField] protected ColorBlock colorTintBlock;

    [SerializeField] protected List<Button> children = new List<Button> ();
    [SerializeField] protected bool autoDetectChildren = false;

    [SerializeField] protected bool isActiveGroup = false;

    [SerializeField] protected bool autoScrollV = false;
    [SerializeField] protected bool autoScrollH = false;

    public bool IsActiveGroup { get { return isActiveGroup; } }

    protected bool setActiveFrameYield = false;

    protected int index;
    protected int previousIndex;

    protected bool movedHorizontal = false;
    protected bool movedVertical = false;

    protected bool isIncrementing = false;    
	
	protected virtual void Start () {
        if (autoDetectChildren)
            children = GetComponentsInChildren<Button> (true).ToList ();

        if (!children[0].IsActive ()) IncrementIndex ();

        if (isActiveGroup) { HUDSelectionManager.SetActiveGroup(this); Enable(); }
	}
		
	protected virtual void Update () {
        if (!isActiveGroup) return;
        if (setActiveFrameYield)
        {
            setActiveFrameYield = false;
            return;
        }
        DetectInput ();
        DetectHiddenTarget();
	}

    private void DetectHiddenTarget()
    {
        //return;
        if (isIncrementing) return;

        if (!children[index].IsActive())
        {
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i].IsActive())
                {
                    index = i;
                    break;
                }
            }
            //index = 0;
            SelectIndex();
        }
    }

    public virtual void SetActiveGroup ()
    {
        HUDSelectionManager.SetActiveGroup ( this );
    }

    public virtual void Enable ()
    {
        SelectIndex();

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

            if (autoScrollV) movedVertical = false;
        }
        else
        {
            if (movedVertical)
            {
                movedVertical = false;

                // Didnt change vertically
                
            }
        }

        if (Input.GetAxis ( "XBO_DPAD_Horizontal" ) != 0)
        {
            if (!movedHorizontal)
            {
                movedHorizontal = true;

                // Change horizontally

            }

            if (autoScrollH) movedHorizontal = false;
        }
        else
        {
            if (movedHorizontal)
            {
                movedHorizontal = false;

                //Didnt Change horizontally 

                
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
        if (children[index] == null) return;
        if (!children[index].IsActive()) return;
        //Debug.Log ( "Invoking on " + children[index].name );
        children[index].onClick.Invoke ();        

        switch (childrenType)
        {
            case ChildrenType.Animation:
                children[index].GetComponent<Animator>().SetTrigger("Pressed");
                break;

            case ChildrenType.ColourTint:
                children[index].GetComponent<Button>().targetGraphic.color = colorTintBlock.pressedColor;                
                break;

            default:
                children[index].GetComponent<Button>().targetGraphic.color = colorTintBlock.pressedColor;
                break;
        }
    }

    protected virtual void SelectIndex ()
    {
        DeselectIndex ( previousIndex );

        if (index >= children.Count) return;
        if (children[index] == null) return;

        switch (childrenType)
        {
            case ChildrenType.Animation:                
                    children[index].GetComponent<Animator>().SetTrigger("Highlighted");
                break;

            case ChildrenType.ColourTint:                
                    children[index].GetComponent<Button>().targetGraphic.color = colorTintBlock.highlightedColor;
                break;

            default:                
                    children[index].GetComponent<Button>().targetGraphic.color = colorTintBlock.highlightedColor;
                break;
        }

    }

    protected virtual void DeselectIndex (int index)
    {
        if (index >= children.Count) return;
        if (children[index] == null) return;

        switch (childrenType)
        {
            case ChildrenType.Animation:                
                    children[index].GetComponent<Animator>().SetTrigger("Normal");
                break;

            case ChildrenType.ColourTint:                
                    children[index].GetComponent<Button>().targetGraphic.color = colorTintBlock.normalColor;
                break;

            default:                
                    children[index].GetComponent<Button>().targetGraphic.color = colorTintBlock.normalColor;
                break;
        }
    }

    protected virtual void IncrementIndex ()
    {
        isIncrementing = true;

        previousIndex = index;
        do
        {

            index++;
            if (index >= children.Count) index = 0;

        } while (!children[index].IsActive ());

        //Debug.Log("Found next active child - " + children[index].gameObject.name);

        SelectIndex ();
        isIncrementing = false;
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
