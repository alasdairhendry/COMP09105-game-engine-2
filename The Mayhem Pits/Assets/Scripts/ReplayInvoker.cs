﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayInvoker : MonoBehaviour {

    [SerializeField] private Replayable replayable;
    [SerializeField] private Vector3 offset;

    public void RequestReplay(float captureDelay)
    {
        if (replayable.ReplayID == -1) return;
        FindObjectOfType<ReplayPlayer>().RequestReplay(captureDelay, replayable.ReplayID, offset);        
    }

    public void SetReplayable(Replayable replayable)
    {
        this.replayable = replayable;
    }

    public void AddFramedAction(System.Action action)
    {
        if (action != null)
        {
            if (replayable.ReplayID == 177)
                Debug.Log("ReplayInvoker - Add Framed Action");
            replayable.AddFramedAction(action);
        }
    }
}
