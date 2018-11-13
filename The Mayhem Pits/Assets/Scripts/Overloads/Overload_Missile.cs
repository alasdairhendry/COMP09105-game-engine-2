using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Overload_Missile : Overload {

    [SerializeField] private GameObject missilePrefab;

    public override void Use ()
    {
        if (!isInUse) return;

        LockableTarget target = crosshairPanel.GetTarget ();
        if (target == null) { return; }      

        GameObject go = PhotonNetwork.Instantiate ( missilePrefab.name, new Vector3 ( 25.0f, 0.0f, 0.0f ), Quaternion.identity );        
        go.GetComponent<Overload_MissileObject> ().SetTarget ( target.GetComponent<NetworkGameRobot> () );
 
        base.Finish ();
    }
}
