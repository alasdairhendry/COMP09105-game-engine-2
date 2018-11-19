using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildOffset : MonoBehaviour {

    [SerializeField] private GameObject root;
    [SerializeField] private Vector3 initialOffset;

    float initialY = 0;


	// Use this for initialization
	void Start () {
        //initialOffset = root.transform.localPosition - transform.localPosition;    
	}
	
	// Update is called once per frame
	void Update () {
        if (root == null || transform == null) { Destroy(this.gameObject); return; }

        transform.SetParent ( null );
        transform.position = root.transform.position + initialOffset;
	}
}
