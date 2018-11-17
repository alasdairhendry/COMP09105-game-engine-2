using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayInvoker : MonoBehaviour {

    [SerializeField] private Replayable replayable;
    [SerializeField] private Vector3 offset;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RequestReplay();
        }
	}

    public void RequestReplay()
    {
        if (replayable.ReplayID == -1) return;
        FindObjectOfType<ReplayPlayer>().ReplayWithCanvas();
        FindObjectOfType<ReplayPlayer>().SetTarget(replayable.ReplayID, offset);
    }

    public void SetReplayable(Replayable replayable)
    {
        this.replayable = replayable;
    }

    public void AddFramedAction(System.Action action)
    {
        if (action != null)
            replayable.AddFramedAction(action);
    }
}
