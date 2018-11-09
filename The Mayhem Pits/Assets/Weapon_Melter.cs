using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Melter : Weapon {

    private ParticleSystem particles;

    protected override void Start ()
    {
        particles = GetComponentInChildren<ParticleSystem> ();
        base.Start ();
    }

    protected override void Attack ()
    {
        base.Attack ();
        Animate ();

        if (Input.GetAxis ( "XBO_LT" ) > 0)
        {
            if (currentResourceLeft > 0)
            {
                currentResourceLeft -= Input.GetAxis ( "XBO_LT" ) * Time.deltaTime * data.baseResourceDepletion;
                isAttacking = true;
            }
            else isAttacking = false;
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
        base.Animate ();

        if (isAttacking) { if (particles.isPlaying) return; particles.Play (); }
        else { particles.Stop (); }

        photonView.RPC ( "RPCAnimate", RpcTarget.OthersBuffered, isAttacking );
    }

    [PunRPC]
    private void RPCAnimate (bool isAttacking)
    {
        if (isAttacking) { if (particles.isPlaying) return; particles.Play (); }
        else { particles.Stop (); }
    }

    public override void OnChildCollisionStay (Collider collision)
    {
        base.OnChildCollisionStay ( collision );

        if (!isAttacking) return;        

        RobotHealth health = collision.gameObject.GetComponentInParent<RobotHealth> ();
        if (health == GetComponentInParent<RobotHealth> ()) return;
        if (health != null)
        {
            health.ApplyDamageToOtherPlayer ( data.baseDamage * Input.GetAxis ( "XBO_LT" ) * Time.deltaTime );
        }

        Robot robot = collision.gameObject.GetComponent<Robot> ();

        if (robot != null)
        {
            robot.AddHeat ( data.baseDamage * Time.deltaTime * 0.5f );
            return;
        }

        Heatable heatable = collision.gameObject.GetComponentInParent<Heatable> ();

        if (heatable != null)
        {
            Debug.Log ( "Heating Gameobject " + heatable.gameObject.name );            
            heatable.Add ( data.baseDamage * Time.deltaTime * 0.5f );
        }
        else
        {
            Debug.Log ( "Hitting item that isnt heatable" );
        }        
    }
}
