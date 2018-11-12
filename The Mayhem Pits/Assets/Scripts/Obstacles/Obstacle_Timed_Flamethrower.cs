using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_Timed_Flamethrower : Obstacle_Timed {

    [SerializeField] private List<ParticleSystem> particles = new List<ParticleSystem> ();
    [SerializeField] private float particleDelay = 0.25f;
    private float currentParticleDelay = 0.0f;

    protected override void Update ()
    {
        base.Update ();
        ParticleDelay ();
    }

    public override void Activate ()
    {
        PlayParticles ();   
    }

    private void ParticleDelay ()
    {
        if (particles[0].isPlaying)
        {
            currentParticleDelay += Time.deltaTime;

            if(currentParticleDelay >= particleDelay)
            {
                currentParticleDelay = 0.0f;
                StopParticles ();                
            }
        }
    }

    private void PlayParticles ()
    {
        for (int i = 0; i < particles.Count; i++)
        {
            particles[i].Play ();
        }
    }

    private void StopParticles ()
    {
        for (int i = 0; i < particles.Count; i++)
        {
            particles[i].Stop ();
        }
    }
}
