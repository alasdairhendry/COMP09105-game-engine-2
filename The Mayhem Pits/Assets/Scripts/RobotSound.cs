using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotSound : MonoBehaviourPunCallbacks {

    private AudioSource audioSource;
    [SerializeField] private float idlePitch = 0.4f;
    [SerializeField] private float constantSpeedPitch = 0.1f;
    [SerializeField] private float wheelConnectionModifier = 0.05f;

    private float currentPitch;
    private float targetPitch;
    [SerializeField] private float pitchIncreaseLerp = 0.1f;
    [SerializeField] private float pitchDecreaseLerp = 0.1f;

    void Start () {
        audioSource = GetComponent<AudioSource> ();
	}

    private void Update ()
    {
        SetCurrentPitch ();        
    }

    public void SetAudio(float input, float wheelConnections)
    {
        targetPitch = Mathf.Lerp ( idlePitch, constantSpeedPitch, input );
        targetPitch += (4 - wheelConnections) * wheelConnectionModifier;       
        photonView.RPC ( "RPCSetPitch", RpcTarget.Others, targetPitch );
    }

    private void SetCurrentPitch ()
    {
        if(currentPitch< targetPitch)
        {
            currentPitch += pitchIncreaseLerp * Time.deltaTime;

            if (currentPitch > targetPitch) currentPitch = targetPitch;
        }
        else if(currentPitch > targetPitch)
        {
            currentPitch -= pitchDecreaseLerp * Time.deltaTime;

            if (currentPitch < targetPitch) currentPitch = targetPitch;
        }

        audioSource.pitch = currentPitch;
    }

    public void StopAudio ()
    {
        targetPitch = 0.0f;
    }

    [PunRPC]
    private void RPCSetPitch(float pitch)
    {
        targetPitch = pitch;
    }
}
