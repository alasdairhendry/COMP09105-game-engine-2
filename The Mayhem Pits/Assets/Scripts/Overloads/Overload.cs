using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overload : MonoBehaviourPunCallbacks {

    [SerializeField] protected string overloadName = "New Overload";
    [SerializeField] protected Sprite sprite;    

    [SerializeField] protected bool hasSpecificInput;
    [SerializeField] protected string specificInput;
    [SerializeField] protected Sprite specificInputSprite;

    public string OverloadName { get { return overloadName; } }
    public Sprite Sprite { get { return sprite; } }

    protected bool isInUse = false;
    
    protected GameObject localRobot;
    protected HUD_Crosshair_Panel crosshairPanel;
    protected LockableTarget lockableTarget;

    public struct OverloadActivationStatus
    {
        public string message;
        public bool status;
    }

    protected virtual void Start ()
    {
        crosshairPanel = FindObjectOfType<HUD_Crosshair_Panel> ();
    }

    protected virtual void Update ()
    {
        
    }

    public virtual void SetLocalRobot (GameObject robot)
    {
        localRobot = robot;
    }

    // Called from elsewhere when the player tries to use this overload
    public virtual OverloadActivationStatus Activate ()
    {
        if (isInUse) return new OverloadActivationStatus { message = "Ability already in use.", status = false };        
        if (!crosshairPanel.Show (SetLockableTarget)) return new OverloadActivationStatus { message = "Another ability is in use", status = false };

        isInUse = true;        
        return new OverloadActivationStatus { message = "Success.", status = true }; ;
    }

    protected virtual void SetLockableTarget(LockableTarget target)
    {
        lockableTarget = target;
        photonView.RPC ( "RPCOnTargeted", RpcTarget.All, target.GetComponent<PhotonView> ().ViewID );
    }

    [PunRPC]
    protected void RPCOnTargeted(int targetedViewID)
    {
        PhotonView view = PhotonView.Find ( targetedViewID );
        if (view.Owner.IsLocal)
        {
            KillFeed.Instance.AddInfo ( "A player has targeted you.", KillFeed.InfoType.Overload );
        }
    }

    public void Cancel ()
    {
        isInUse = false;
        FindObjectOfType<HUD_Crosshair_Panel> ().Hide ();
    }

    // The actual logic for the overload will go here
    public virtual void Use ()
    {

    }    

    // Called by the overload itself when it has finished doing its thang.
    protected virtual void Finish ()
    {
        FindObjectOfType<HUD_Overloads_Panel> ().OnFinishAbility ();
        FindObjectOfType<HUD_Crosshair_Panel> ().Hide ();
        Destroy ( this.gameObject );
    }

    public Overload FindAbility (List<Overload> overloads, string _overloadName)
    {
        for (int i = 0; i < overloads.Count; i++)
        {
            if (overloads[i].overloadName == _overloadName)
                return overloads[i];
        }

        return null;
    }
}
