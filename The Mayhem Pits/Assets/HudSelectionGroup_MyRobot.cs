using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    protected override void OnMoveHorizontal(float direction)
    {
        if (direction == 0) return;
        InvokeIndex();
    }
}
