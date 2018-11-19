using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSizeLocker : MonoBehaviour {

    private ParticleSystem p;
    private ParticleSystem.MainModule m;
    [SerializeField] private Vector2 minMax = new Vector2();

	// Use this for initialization
	void Start () {
        p = GetComponent<ParticleSystem>();
        m = p.main;
	}
	
	// Update is called once per frame
	void Update () {
        if (p == null) return;
        m.startSize = new ParticleSystem.MinMaxCurve(minMax.x, minMax.y);
	}
}
