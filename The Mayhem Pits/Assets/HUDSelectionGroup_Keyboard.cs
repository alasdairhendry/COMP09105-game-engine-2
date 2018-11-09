using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HUDSelectionGroup_Keyboard : HUDSelectionGroup {

    AxisEventData currentAxis;

    protected override void DetectInput ()
    {
        currentAxis = new AxisEventData ( EventSystem.current );

        if (Input.GetAxis ( "XBO_DPAD_Vertical" ) != 0)
        {
            if (!movedVertical)
            {
                movedVertical = true;

                // Change Vertically
                OnMoveVertical (Input.GetAxis("XBO_DPAD_Vertical"));

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
                OnMoveHorizontal ( Input.GetAxis ( "XBO_DPAD_Horizontal" ) );

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

    protected override void OnMoveVertical (float direction)
    {
        if (Input.GetAxis ( "XBO_DPAD_Vertical" ) < 0)
        {
            currentAxis.moveDir = MoveDirection.Down;
            ExecuteEvents.Execute ( children[index].gameObject, currentAxis, ExecuteEvents.moveHandler );
            Debug.Log ( EventSystem.current.currentSelectedGameObject );

            GameObject newTarget = EventSystem.current.currentSelectedGameObject;

            if(newTarget.GetComponent<Button> () == null)
            {
                return;
            }

            if (children.Contains ( newTarget.GetComponent<Button> () ))
            {
                base.ChangeIndex ( children.IndexOf ( newTarget.GetComponent<Button>() ) );
                EventSystem.current.SetSelectedGameObject ( null );
            }
            else
            {
                EventSystem.current.SetSelectedGameObject ( null );
            }
        }
        else
        {
            currentAxis.moveDir = MoveDirection.Up;
            ExecuteEvents.Execute ( children[index].gameObject, currentAxis, ExecuteEvents.moveHandler );
            Debug.Log ( EventSystem.current.currentSelectedGameObject );

            GameObject newTarget = EventSystem.current.currentSelectedGameObject;

            if (newTarget.GetComponent<Button> () == null)
            {
                return;
            }

            if (children.Contains ( newTarget.GetComponent<Button> () ))
            {
                base.ChangeIndex ( children.IndexOf ( newTarget.GetComponent<Button> () ) );
                EventSystem.current.SetSelectedGameObject ( null );
            }
            else
            {
                EventSystem.current.SetSelectedGameObject ( null );
            }
        }
    }

    protected override void OnMoveHorizontal (float direction)
    {
        if (Input.GetAxis ( "XBO_DPAD_Horizontal" ) < 0)
        {
            currentAxis.moveDir = MoveDirection.Left;
            ExecuteEvents.Execute ( children[index].gameObject, currentAxis, ExecuteEvents.moveHandler );
            Debug.Log ( EventSystem.current.currentSelectedGameObject );

            GameObject newTarget = EventSystem.current.currentSelectedGameObject;

            if (newTarget.GetComponent<Button> () == null)
            {
                return;
            }

            if (children.Contains ( newTarget.GetComponent<Button> () ))
            {
                base.ChangeIndex ( children.IndexOf ( newTarget.GetComponent<Button> () ) );
                EventSystem.current.SetSelectedGameObject ( null );
            }
            else
            {
                EventSystem.current.SetSelectedGameObject ( null );
            }
        }
        else
        {
            currentAxis.moveDir = MoveDirection.Right;
            ExecuteEvents.Execute ( children[index].gameObject, currentAxis, ExecuteEvents.moveHandler );
            Debug.Log ( EventSystem.current.currentSelectedGameObject );

            GameObject newTarget = EventSystem.current.currentSelectedGameObject;

            if (newTarget.GetComponent<Button> () == null)
            {
                return;
            }

            if (children.Contains ( newTarget.GetComponent<Button> () ))
            {
                base.ChangeIndex ( children.IndexOf ( newTarget.GetComponent<Button> () ) );
                EventSystem.current.SetSelectedGameObject ( null );
            }
            else
            {
                EventSystem.current.SetSelectedGameObject ( null );
            }
        }
    }

    protected override void SelectIndex ()
    {
        DeselectIndex ( previousIndex );
        children[index].GetComponent<Image> ().color = children[index].colors.highlightedColor;
    }

    protected override void DeselectIndex (int index)
    {
        children[index].GetComponent<Image> ().color = children[index].colors.normalColor;
    }

    protected override void InvokeIndex ()
    {
        Debug.Log ( "Invoking on " + children[index].name );
        children[index].onClick.Invoke ();
        children[index].GetComponent<Image> ().color = children[index].colors.pressedColor;
    }

    public override void Enable ()
    {
        //if (!children[index].IsActive ()) IncrementIndex ();
        index = 0;
        SelectIndex ();
        isActiveGroup = true;
        setActiveFrameYield = true;
    }
}
