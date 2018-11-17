using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRModeDisable : MonoBehaviour {

    [SerializeField] private List<Behaviour> disableVR = new List<Behaviour>();
    [SerializeField] private List<Behaviour> disableNormal = new List<Behaviour>();

    private void Start()
    {
        if (ClientMode.Instance.GetMode == ClientMode.Mode.VR)
        {
            for (int i = 0; i < disableVR.Count; i++)
            {
                disableVR[i].enabled = false;
            }
        }
        else
        {
            for (int i = 0; i < disableNormal.Count; i++)
            {
                disableNormal[i].enabled = false;
            }
        }
    }
    
}
