using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Obstacle_Timed_Flamethrower : Obstacle_Timed {

    [SerializeField] private List<ParticleSystem> particles = new List<ParticleSystem> ();
    [SerializeField] private float particleDelay = 0.25f;
    [SerializeField] private float damage = 10.0f;

    [SerializeField] private GameObject replayParticles;

    private float currentParticleDelay = 0.0f;
    private bool obstacleActive = false;

    protected override void Start ()
    {
        RegisterTriggers ();
        GetParticles ();
    }

    private void RegisterTriggers ()
    {
        ChildCollider[] colls = GetComponentsInChildren<ChildCollider> ();

        for (int i = 0; i < colls.Length; i++)
        {
            colls[i].triggerStay += Trigger;
        }
    }

    private void GetParticles ()
    {
        particles = GetComponentsInChildren<ParticleSystem> ().ToList ();
    }

    protected override void Update ()
    {
        base.Update ();
        ParticleDelay ();
    }

    public override void Activate ()
    {
        PlayParticles ();
        obstacleActive = true;

        Replayable r = GetComponentInChildren<Replayable>();

        for (int i = 0; i < particles.Count; i++)
        {
            int x = i;
            r.AddFramedAction(() =>
            {
                Debug.Log(x, this);
                GameObject go = Instantiate(replayParticles);
                go.transform.position = particles[x].transform.position;
                go.transform.rotation = particles[x].transform.rotation;                
            });            
        }
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
                obstacleActive = false;
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

    private List<RobotHealth> damagedThisFrame = new List<RobotHealth>();

    private void LateUpdate()
    {
        damagedThisFrame.Clear();
    }

    private void Trigger (Collider other)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (!obstacleActive) return;

        RobotHealth health = other.gameObject.GetComponentInParent<RobotHealth> ();

        if (health == GetComponentInParent<RobotHealth> ()) return;
        if (damagedThisFrame.Contains(health)) return;

        if (health != null)
        {
            health.ApplyDamageToOtherPlayer ( damage * Time.deltaTime );
            damagedThisFrame.Add(health);
        }

        Heatable heatable = other.gameObject.GetComponentInParent<Heatable> ();

        if (heatable != null)
        {            
            heatable.AddNetwork ( damage * Time.deltaTime * 0.5f );
        }
        else
        {
            Debug.Log ( "Hitting item that isnt heatable" );
        }
    }
}
