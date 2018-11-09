using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InfoButton : MonoBehaviour {

    [Header("Default")]
    [SerializeField] private string text;
    [SerializeField] private string inputButton;
    [SerializeField] private Image fillImage;
    [SerializeField] private Text label;
    [SerializeField] private UnityEvent action;
    private bool active = true;

    [Header ( "Holding" )]
    [SerializeField] private bool hold;
    [SerializeField] private float holdRequired;
    private bool isHolding = false;
    private float currentHold = 0.0f;

    [Header ( "Pressing" )]
    [SerializeField] private float releaseDelay;    
    private float currentReleaseDelay = 0.0f;

    private void Start ()
    {
        label.text = text;
    }

    private void Update ()
    {
        if (!active)
        {
            currentReleaseDelay = 0.0f;
            currentHold = 0.0f;
            isHolding = false;
            return;
        }

        if (hold) MonitorHold ();
        else MonitorPress ();
    }

    private void MonitorHold ()
    {
        if (Input.GetButtonDown ( inputButton ))
        {
            if (!isHolding)
                isHolding = true;
        }

        if (Input.GetButton ( inputButton ))
        {
            if (!isHolding) return;
            currentHold += Time.deltaTime;
            fillImage.fillAmount = Mathf.Lerp ( 0.0f, 1.0f, currentHold / holdRequired );

            if(currentHold >= holdRequired)
            {
                Invoke ();
                currentHold = 0.0f;
                isHolding = false;
                fillImage.fillAmount = 0.0f;
            }
        }

        if (Input.GetButtonUp ( inputButton ))
        {
            isHolding = false;
            currentHold = 0.0f;
            fillImage.fillAmount = 0.0f;
        }
    }

    private void MonitorPress ()
    {
        if (Input.GetButtonDown ( inputButton ))
        {
            Invoke ();
            currentReleaseDelay = releaseDelay;
            fillImage.fillAmount = 1.0f;
        }

        if (currentReleaseDelay >= 0)
        {
            currentReleaseDelay -= Time.deltaTime;

            if(currentReleaseDelay <= 0)
            {
                currentReleaseDelay = 0.0f;
                fillImage.fillAmount = 0.0f;
            }
        }
    }

    private void Invoke ()
    {
        if(action != null)
        {
            action.Invoke ();
        }
    }
}
