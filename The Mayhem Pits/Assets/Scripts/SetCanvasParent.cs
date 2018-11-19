using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCanvasParent : MonoBehaviour {

    [SerializeField] Transform parent;

	// Use this for initialization
	void Start () {
        transform.SetParent(parent);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
