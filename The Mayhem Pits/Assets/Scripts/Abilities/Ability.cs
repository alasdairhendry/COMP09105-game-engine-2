using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviourPunCallbacks {

    [SerializeField] protected string abilityName = "New Ability";
    [SerializeField] protected float cooldown = 0.0f;
    [SerializeField] protected int uses = 1;

    protected bool isInUse = false;

    protected float currCooldown = 0.0f;
    protected bool isOnCooldown = false;

    protected int currentUses = 0;

    protected Robot targetRobot;

    public struct AbilityActivationStatus
    {
        public string message;
        public bool status;
    }

    protected virtual void Update()
    {
        MonitorCooldown();    
    }

    public virtual void SetTargetRobot(Robot robot)
    {
        targetRobot = robot;
    }

    // Called from elsewhere when the player tries to use this ability
    public virtual AbilityActivationStatus Activate()
    {
        if (isInUse) return new AbilityActivationStatus { message = "Ability already in use.", status = false };
        if (isOnCooldown) return new AbilityActivationStatus { message = "Ability not ready yet.", status = false };

        isInUse = true;
        OnActivate();
        return new AbilityActivationStatus { message = "Success.", status = true }; ;
    }

    // The actual logic for the ability will go here
    protected virtual void OnActivate()
    {

    }

    // Called by the ability itself when it has finished doing its thang.
    protected virtual void Finish()
    {        
        if(currentUses >= uses && uses >= 1)
        {
            Destroy(this);
        }
        else
        {
            isInUse = false;
            isOnCooldown = true;
            currCooldown = cooldown;
        }
    }

    protected virtual void MonitorCooldown()
    {
        if (isOnCooldown)
        {
            currCooldown -= Time.deltaTime;

            if(currCooldown <= 0.0f)
            {
                isOnCooldown = false;
                currCooldown = cooldown;
            }
        }
    }

    public Ability FindAbility(List<Ability> abilities, string _abilityName)
    {
        for (int i = 0; i < abilities.Count; i++)
        {
            if (abilities[i].abilityName == _abilityName)
                return abilities[i];
        }

        return null;
    }
}
