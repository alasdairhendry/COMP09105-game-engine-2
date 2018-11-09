using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HUDSelectionManager {

    private static HUDSelectionGroup activeGroup;

    public static void SetActiveGroup(HUDSelectionGroup group)
    {
        if (activeGroup != null)
            activeGroup.Disable ();

        activeGroup = group;

        if (group.IsActiveGroup) return;

        group.Enable ();
    }
}
