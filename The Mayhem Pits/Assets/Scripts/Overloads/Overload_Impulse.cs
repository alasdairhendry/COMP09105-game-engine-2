using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Overload_Impulse : Overload {

    public override void Use ()
    {
        if (!isInUse) return;

        LockableTarget target = crosshairPanel.GetTarget ();
        if (target == null) { base.Finish (); return; }

        photonView.RPC ( "RPCUse", RpcTarget.All, localRobot.GetComponent<PhotonView> ().Owner.ActorNumber, target.GetComponent<PhotonView> ().Owner.ActorNumber );
        KillFeed.Instance.AddInfo ( localRobot.GetComponent<PhotonView> ().Owner.NickName.ToUpper () + " USED IMPULSE ON " + target.GetComponent<PhotonView> ().Owner.NickName.ToUpper (), KillFeed.InfoType.Overload, RpcTarget.All );

        base.Finish ();
    }

    [PunRPC]
    private void RPCUse (int _from, int _to)
    {
        if(PhotonNetwork.LocalPlayer.ActorNumber == _to)
        {
            GameObject localRobot = GameObject.FindGameObjectWithTag ( "LocalGamePlayer" );
            localRobot.GetComponent<Rigidbody> ().AddForceAtPosition ( Vector3.up * 200.0f * Time.fixedDeltaTime, localRobot.transform.position - new Vector3 ( 0.0f, 0.0f, 0.75f ), ForceMode.VelocityChange );
        }        
    }

    protected override void Finish ()
    {
        FindObjectOfType<HUD_Overloads_Panel> ().OnFinishAbility ();
        FindObjectOfType<HUD_Crosshair_Panel> ().Hide ();
        PhotonNetwork.Destroy ( this.gameObject );
    }
}
