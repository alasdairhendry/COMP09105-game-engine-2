﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Replayable : MonoBehaviour {
    
    private ReplayController controller;
    [SerializeField] private int replayID = -1;
    public int ReplayID { get { return replayID; } }

    public System.Action ActionThisUpdate;
    public bool AddedActionThisUpdate { get { if (ActionThisUpdate == null) return false; else return true; } }

    private void Start()
    {
        controller = FindObjectOfType<ReplayController>();
        if (controller == null) return;
        replayID = controller.RegisterAsReplayable(this);
    }

    public void OnUpdate()
    {
        if(replayID == 177)
        {
            if(ActionThisUpdate != null)
            {
                Debug.Log("Action doesnt == null");
            }
        }
        controller.CollectData(this, ActionThisUpdate);
        ActionThisUpdate = null;
    }

    public void AddFramedAction(System.Action action)
    {
        if (action != null)
        {
            if (replayID == 177)
                Debug.Log("Replayable - Add Framed Action");
            ActionThisUpdate += action;
        }
    }

    private void OnDestroy()
    {
        if (controller != null)
            controller.UnRegisterAsReplayable(this);
    }

}
