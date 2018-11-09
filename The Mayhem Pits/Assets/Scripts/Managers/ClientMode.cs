﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientMode : MonoBehaviour {

    public static ClientMode singleton;

    private void Awake()
    {
        if (singleton == null)
            singleton = this;
        else if (singleton != this)
            Destroy(gameObject);

        if(mode == Mode.Normal)
            UnityEngine.XR.XRSettings.enabled = false;
    }

    public enum Mode { Normal, VR }
    [SerializeField] private Mode mode = Mode.Normal;
    public Mode GetMode { get { return mode; } }    

	public void SetModeNormal()
    {        
        mode = Mode.Normal;
    }

    public void SetModeVR()
    {
        mode = Mode.VR;
    } 
}