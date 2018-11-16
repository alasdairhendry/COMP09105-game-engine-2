using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Replayable : MonoBehaviour {
    
    private ReplayController controller;

    private void Start()
    {
        controller = FindObjectOfType<ReplayController>();
        controller.RegisterAsReplayable(this);
        Debug.Log("Booooperino");
    }

    public void OnUpdate()
    {
        controller.CollectData(this);
    }

    private void OnDestroy()
    {
        if (controller != null)
            controller.UnRegisterAsReplayable(this);
    }

}
