using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Melter : Weapon {

    [SerializeField] private GameObject replayParticle;

    private ParticleSystem particles;
    private Replayable replayable;

    protected override void Start ()
    {
        particles = GetComponentInChildren<ParticleSystem> ();
        replayable = GetComponentInChildren<Replayable>();
        base.Start ();
    }

    protected override void Attack ()
    {
        base.Attack();
        Animate();

        if (!isAttacking)
        {
            if (currentResourceLeft <= data.baseResourceMax * 0.20f)
            {
                if (weaponPanel != null)
                    weaponPanel.SetSliderState(false);
                allowedAttack = false;
            }
            else
            {
                if (weaponPanel != null)
                    weaponPanel.SetSliderState(true);
                allowedAttack = true;
            }
        }

        if (Input.GetAxis("XBO_LT") > 0)
        {
            if (currentResourceLeft > 0)
            {
                if (allowedAttack)
                {
                    currentResourceLeft -= Input.GetAxis("XBO_LT") * Time.deltaTime * data.baseResourceDepletion;
                    isAttacking = true;
                }
                else
                {
                    isAttacking = false;
                    currentResourceLeft += Time.deltaTime * data.baseResourceRegeneration;
                }
            }
            else
            {
                isAttacking = false;
                currentResourceLeft += Time.deltaTime * data.baseResourceRegeneration;
            }
        }
        else
        {
            currentResourceLeft += Time.deltaTime * data.baseResourceRegeneration;
            isAttacking = false;
        }

        currentResourceLeft = Mathf.Clamp(currentResourceLeft, 0.0f, data.baseResourceMax);
    }

    protected override void Animate ()
    {
        base.Animate ();
        photonView.RPC ( "RPCAnimate", RpcTarget.All, isAttacking );
    }

    [PunRPC]
    private void RPCAnimate (bool isAttacking)
    {
        if (isAttacking)
        {
            if (!replayable.AddedActionThisUpdate)
            {
                Vector3 pos = this.transform.Find("Particle System").position;
                Quaternion rot = this.transform.Find("Particle System").rotation;

                replayable.AddFramedAction(() =>
                {
                    if (this.transform == null) return;
                    if (this.transform.Find("Particle System") == null) return;

                    GameObject go = Instantiate(replayParticle);
                    go.transform.position = pos;
                    go.transform.rotation = rot;
                });
            }

            if (particles.isPlaying) return;
            particles.Play();
        }
        else { particles.Stop(); }
    }

    public override void OnChildCollisionStay (Collider collision)
    {               
        if (!isAttacking) return;        
        
        RobotHealth health = collision.gameObject.GetComponentInParent<RobotHealth> ();        

        if (health == GetComponentInParent<RobotHealth> ()) return;
        if (damagesThisFrame.Contains(health)) { Debug.Log("Already hurt this robot!"); return; }

        if (health != null)
        {
            float damage = data.baseDamage * Input.GetAxis("XBO_LT") * Time.deltaTime;
            localRobot.damageInflicted += damage;

            health.ApplyDamageToOtherPlayer ( damage );
            damagesThisFrame.Add(health);
        }

        //Heatable heatable = collision.gameObject.GetComponentInParent<Heatable> ();

        //if (heatable != null)
        //{
        //    Debug.Log ( "Heating Gameobject " + heatable.gameObject.name );            
        //    heatable.AddNetwork ( data.baseDamage * Time.deltaTime * 0.5f );
        //}
        //else
        //{
        //    Debug.Log ( "Hitting item that isnt heatable" );
        //}        
    }
}
