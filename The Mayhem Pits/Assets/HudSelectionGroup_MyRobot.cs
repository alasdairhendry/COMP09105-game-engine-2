using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudSelectionGroup_MyRobot : HUDSelectionGroup {

    protected override void DetectInput()
    {
        if (Input.GetAxis("XBO_DPAD_Vertical") != 0)
        {
            if (!movedVertical)
            {
                movedVertical = true;

                // Change Vertically
                if (Input.GetAxis("XBO_DPAD_Vertical") < 0) IncrementIndex();
                else DecrementIndex();

                if (children[index].GetComponent<Text> ().text.ToLower ().Contains ( "body" ))
                    FindObjectOfType<MyRobotControls> ().SetActionText_Body ();
                else if (children[index].GetComponent<Text> ().text.ToLower ().Contains ( "weapon" ))
                    FindObjectOfType<MyRobotControls> ().SetActionText_Weapon ();
                else if (children[index].GetComponent<Text> ().text.ToLower ().Contains ( "emblem" ))
                    FindObjectOfType<MyRobotControls> ().SetActionText_Emblem ();
                else if (children[index].GetComponent<Text> ().text.ToLower ().Contains ( "skin" ))
                    FindObjectOfType<MyRobotControls> ().SetActionText_Skin ();
                else if (children[index].GetComponent<Text> ().text.ToLower ().Contains ( "back" ))
                    FindObjectOfType<MyRobotControls> ().SetActionText_Back ();

                FindObjectOfType<MyRobotControls> ().ResetGraphics ();

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

        if (Input.GetAxis("XBO_DPAD_Horizontal") != 0)
        {
            if (!movedHorizontal)
            {
                movedHorizontal = true;

                // Change horizontally
                OnMoveHorizontal(Input.GetAxis("XBO_DPAD_Horizontal"));
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
            OnMoveHorizontal(Input.GetAxisRaw("XBO_DPAD_Horizontal"));
        }

        if (Input.GetButtonUp("XBO_A"))
        {
            InvokeIndex();
        }
    }

    protected override void OnMoveHorizontal (float direction)
    {
        if (direction == 0) return;

        if (children[index].GetComponent<Text> ().text.ToLower ().Contains ( "body" ))
            FindObjectOfType<MyRobotControls> ().IncrementBody ( direction );
        else if (children[index].GetComponent<Text> ().text.ToLower ().Contains ( "weapon" ))
            FindObjectOfType<MyRobotControls> ().IncrementWeapon ( direction );
        else if (children[index].GetComponent<Text> ().text.ToLower ().Contains ( "emblem" ))
            FindObjectOfType<MyRobotControls> ().IncrementEmblem ( direction );
        else if (children[index].GetComponent<Text> ().text.ToLower ().Contains ( "skin" ))
            FindObjectOfType<MyRobotControls> ().IncrementSkin ( direction );
    }
}
