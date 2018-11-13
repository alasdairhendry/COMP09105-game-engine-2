﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Wheelsaw : Weapon {

    private float lerpedInput = 0.0f;
    private float inputDamp = 1.0f;

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
        if (currentResourceLeft <= 0) lerpedInput = SmoothLerp.Lerp ( lerpedInput, 0.0f, inputDamp * Time.deltaTime );
        else lerpedInput = SmoothLerp.Lerp ( lerpedInput, Input.GetAxis ( "XBO_LT" ), inputDamp * Time.deltaTime );

        if (!isAttacking)
        {
            lerpedInput = SmoothLerp.Lerp ( lerpedInput, 0, inputDamp * Time.deltaTime );
        }

        lerpedInput = Mathf.Clamp ( lerpedInput, 0.0f, 0.99f );

        animator.SetFloat ( "Blend", lerpedInput );
    }    

    public override void OnChildCollisionStay (Collider collision)
    {
        if (!isAttacking) return;

        //Debug.Log ( "Sawing " + collision.gameObject.name );

        RobotHealth health = collision.gameObject.GetComponentInParent<RobotHealth> ();
        if (health == null) { Debug.Log ( "Health doesnt exist" ); return; }
        health.ApplyDamageToOtherPlayer ( data.baseDamage * Input.GetAxis ( "XBO_LT" ) * Time.deltaTime );
        Debug.Log ( "Applying damage", this );
    }
}