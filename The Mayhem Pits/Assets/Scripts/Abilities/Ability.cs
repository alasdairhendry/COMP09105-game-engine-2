using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviourPunCallbacks {

    [SerializeField] protected string abilityName = "New Ability";
    [SerializeField] protected int id;
    [SerializeField] protected Sprite sprite;
    [SerializeField] protected float cooldown = 0.0f;
    [SerializeField] protected int uses = 1;
    [SerializeField] protected bool hasSpecificInput;
    [SerializeField] protected string specificInput;
    [SerializeField] protected Sprite specificInputSprite;

    public string AbilityName { get { return abilityName; } }
    public Sprite Sprite { get { return sprite; } }

    public bool HasSpecificInput { get { return hasSpecificInput; } }
    public string SpecificInput { get { return specificInput; } }
    public Sprite SpecificInputSprite { get { return specificInputSprite; } }

    protected bool isInUse = false;

    public int GetUsesLeft { get { return uses - currentUses; } }
    public float CooldownTime { get { return cooldown; } }
    public float currCooldown { get; protected set; }
    public bool isOnCooldown { get; protected set; }
    public int ID { get { return id; } }

    protected int currentUses = 0;

    protected GameObject targetRobot;

    public struct AbilityActivationStatus
    {
        public string message;
        public bool status;
    }

    protected virtual void Update()
    {
        MonitorCooldown();    
    }

    public virtual void SetTargetRobot(GameObject robot)
    {
        targetRobot = robot;
    }

    // Called from elsewhere when the player tries to use this ability
    public virtual AbilityActivationStatus Activate()
    {
        if (isInUse) return new AbilityActivationStatus { message = "Ability already in use.", status = false };
        if (isOnCooldown) return new AbilityActivationStatus { message = "Ability not ready yet.", status = false };

        currentUses++;
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
            FindObjectOfType<HUD_Ability_Panel> ().RemoveAbility ( this );
            Destroy(this.gameObject);
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

    public void IncreaseUses(int amount)
    {
        uses += amount;
    }
}
