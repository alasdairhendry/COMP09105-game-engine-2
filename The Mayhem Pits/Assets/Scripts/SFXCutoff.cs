using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SFXCutoff : MonoBehaviour {

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float duckMin = 333.0f;
    [SerializeField] private float duckMax = 22000.0f;
    [SerializeField] private float regenerationRate;

    private float currentDuck = 22000.0f;

	void Start () {
        currentDuck = duckMax;
	}
	
	void Update () {
        LerpDuck();
	}

    private void LerpDuck()
    {
        if (currentDuck < duckMax)
        {
            currentDuck += regenerationRate * Time.deltaTime;
            SetMix();
        }

        currentDuck = Mathf.Clamp(currentDuck, duckMin, duckMax);
    }

    private void SetMix()
    {
        audioMixer.SetFloat("Cutoff", currentDuck);
    }

    public void Cutoff(float v)
    {
        currentDuck = Mathf.Lerp(duckMax, duckMin, v);
        SetMix();
    }
}
