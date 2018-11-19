using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleLightController : MonoBehaviour {

    [SerializeField] private Animator animator;
    [SerializeField] private string boolName;
    private ParticleSystem particleSystem;

	// Use this for initialization
	void Start () {
        particleSystem = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
        if (particleSystem.isPlaying)
        {
            animator.SetBool(boolName, true);
        }
        else
        {
            animator.SetBool(boolName, false);
        }
	}
}
