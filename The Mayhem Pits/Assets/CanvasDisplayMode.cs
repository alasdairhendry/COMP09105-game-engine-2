using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasDisplayMode : MonoBehaviour {

    public Vector3 normalPosition = new Vector3();
    public Vector3 vrPosition = new Vector3();
    public bool doRotation = false;
    public Vector3 normalRotation = new Vector3();
    public Vector3 vrRotation = new Vector3();

    private RectTransform originalRect;
    private Vector3 originalPosition = new Vector3();


	// Use this for initialization
	void Start () {
        originalRect = GetComponent<RectTransform>();
        originalPosition = originalRect.anchoredPosition3D;

		if(ClientMode.Instance.GetMode == ClientMode.Mode.Normal)
        {
            SetupNormal();
        }
        else
        {
            SetupVR();
        }
	}

    private void SetupNormal()
    {
        originalRect.anchoredPosition3D = normalPosition;

        if (doRotation)
            originalRect.localEulerAngles = normalRotation;
    }

    private void SetupVR()
    {
        originalRect.anchoredPosition3D = vrPosition;

        if (doRotation)
            originalRect.localEulerAngles = vrRotation;
    }

    // Update is called once per frame
    void Update () {
	if(this.gameObject.name == "GameReady_Panel")
        {
            Debug.Log ( GetComponent<RectTransform> ().anchoredPosition );
        }	
	}
}
