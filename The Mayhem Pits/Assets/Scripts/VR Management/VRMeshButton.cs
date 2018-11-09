using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRStandardAssets.Utils;

public class VRMeshButton : MonoBehaviour {

    private VRInteractiveItem VRII;
    [SerializeField] private UnityEvent onClick;
    [SerializeField] private UnityEvent onOver;
    [SerializeField] private UnityEvent onOut;

    // Use this for initialization
    void Start () {
        VRII = GetComponent<VRInteractiveItem>();
        VRII.OnClick += On_Click;
        VRII.OnOver += On_Over;
        VRII.OnOut += On_Out;
    }

    private void On_Click()
    {
        onClick.Invoke();
    }

    private void On_Over()
    {
        onOver.Invoke();
    }

    private void On_Out()
    {
        onOut.Invoke();
    }
}
