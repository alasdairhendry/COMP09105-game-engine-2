using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Wheelsaw : Weapon {

    [SerializeField] private GameObject replayParticle;
    private Replayable replayable;

    private float lerpedInput = 0.0f;
    private float inputDamp = 1.0f;

    private ParticleSystem particles;
    private bool isColliding = false;

    protected override void Start()
    {
        particles = GetComponentInChildren<ParticleSystem>();
        replayable = GetComponentInChildren<Replayable>();

        base.Start();        
    }

    protected override void Attack ()
    {
        base.Attack ();
        Animate ();

        if (!isAttacking)
        {
            if (currentResourceLeft <= data.baseResourceMax * 0.20f)
            {
                if (weaponPanel != null)
                    weaponPanel.SetSliderState ( false );
                allowedAttack = false;
            }
            else
            {
                if (weaponPanel != null)
                    weaponPanel.SetSliderState ( true );
                allowedAttack = true;
            }
        }

        if (Input.GetAxis ( "XBO_LT" ) > 0)
        {            
            if (currentResourceLeft > 0)
            {
                if (allowedAttack)
                {
                    currentResourceLeft -= Input.GetAxis ( "XBO_LT" ) * Time.deltaTime * data.baseResourceDepletion;
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

        currentResourceLeft = Mathf.Clamp ( currentResourceLeft, 0.0f, data.baseResourceMax );
    }

    protected override void Animate ()
    {
        photonView.RPC("RPCAnimate", RpcTarget.All, isAttacking, isColliding);

        if (!isAttacking)
        {
            //Debug.Log("!ISATACKING");
            lerpedInput = SmoothLerp.Lerp(lerpedInput, 0, inputDamp * Time.deltaTime);
            animator.SetFloat("Blend", lerpedInput);
            return;
        }

        if (currentResourceLeft <= 0) lerpedInput = SmoothLerp.Lerp ( lerpedInput, 0.0f, inputDamp * Time.deltaTime );
        else lerpedInput = SmoothLerp.Lerp ( lerpedInput, Input.GetAxis ( "XBO_LT" ), inputDamp * Time.deltaTime );

        lerpedInput = Mathf.Clamp ( lerpedInput, 0.0f, 0.99f );

        animator.SetFloat ( "Blend", lerpedInput );
    }    

    [PunRPC]
    private void RPCAnimate(bool isAttacking, bool isColliding)
    {
        if (isAttacking && isColliding)
        {
            if (!replayable.AddedActionThisUpdate)
            {
                Vector3 pos = particles.gameObject.transform.position;
                Quaternion rot = particles.gameObject.transform.rotation;

                replayable.AddFramedAction(() =>
                {
                    if (this.transform == null) return;
                    //if (this.transform.Find("Particle System") == null) return;

                    GameObject go = Instantiate(replayParticle);
                    go.transform.position = pos;
                    go.transform.rotation = rot;
                });
            }

            if (particles.isPlaying) return;
            particles.Play();
        }
        else { if (particles != null) particles.Stop(); }
    }

    public override void OnChildCollisionStay (Collider collision)
    {
        if (!isAttacking) return;
        isColliding = true;
        //Debug.Log ( "Sawing " + collision.gameObject.name );

        RobotHealth health = collision.gameObject.GetComponentInParent<RobotHealth> ();

        if (health == GetComponentInParent<RobotHealth>()) return;
        if (health == null) { Debug.Log ( "Health doesnt exist" ); return; }
        if (damagesThisFrame.Contains(health)) return;

        float damage = data.baseDamage * Input.GetAxis("XBO_LT") * Time.deltaTime;
        localRobot.damageInflicted += damage;

        health.ApplyDamageToOtherPlayer ( damage );
        damagesThisFrame.Add(health);
    }

    public override void OnChildCollisionExit(Collider collision)
    {
        isColliding = false;
    }
}
