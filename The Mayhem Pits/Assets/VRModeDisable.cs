using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRModeDisable : MonoBehaviour {

    [SerializeField] private List<Behaviour> toDisable = new List<Behaviour>();

    private void Start()
    {
        if(ClientMode.Instance.GetMode == ClientMode.Mode.VR)
        {
            for (int i = 0; i < toDisable.Count; i++)
            {
                toDisable[i].enabled = false;
            }
        }
    }
    
}
