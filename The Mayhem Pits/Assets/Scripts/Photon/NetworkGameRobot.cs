using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkGameRobot : MonoBehaviourPunCallbacks {

	// Use this for initialization
	void Start () {        
        SetDisplayName ();
        KillFeed.Instance.AddInfo ( photonView.Owner.NickName + " has joined the game.", KillFeed.InfoType.Joined );

        gameObject.name = "NetworkGameRobot_" + photonView.Owner.NickName;

        if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

        this.gameObject.tag = "LocalGamePlayer";
        SpawnGraphics();
        SetCamera();        
    }

    private void SetDisplayName()
    {
        if (PhotonNetwork.OfflineMode) return;

        //TextMesh[] texts = GetComponentsInChildren<TextMesh>();

        Text text = GetComponentInChildren<Text> ();
        text.text = photonView.Owner.NickName;

        //for (int i = 0; i < texts.Length; i++)
        //{
        //    texts[i].text = photonView.Owner.NickName;
        //}
    }

    private void SpawnGraphics()
    {
        MyRobotData myData = MyRobot.singleton.GetMyRobotData;        

        GameObject body = PhotonNetwork.Instantiate(myData.BodyData.prefab.name, transform.position, transform.rotation, 0);
        GameObject weapon = PhotonNetwork.Instantiate(myData.WeaponData.prefab.name, transform.position, transform.rotation, 0);

        GameObject emblemSpring = PhotonNetwork.Instantiate ( "EmblemSpring_Prefab", transform.position, Quaternion.identity, 0 );
        GameObject emblem = PhotonNetwork.Instantiate ( "Emblem_Trophy_Prefab", transform.position, Quaternion.identity, 0 );

        photonView.RPC("RpcSetupGraphics", RpcTarget.AllBuffered, body.GetPhotonView().ViewID, weapon.GetPhotonView ().ViewID, emblemSpring.GetPhotonView ().ViewID, emblem.GetPhotonView ().ViewID, myData.WeaponMountPosition, myData.WeaponMountRotation, myData.BodyData.mass);
    }

    [PunRPC]
    private void RpcSetupGraphics(int bodyID, int weaponID, int springID, int emblemID, Vector3 weaponMountPosition, Vector3 weaponMountRotation, float mass)
    {
        Debug.Log ( "RpcSetupGraphics" );
        GameObject body = PhotonView.Find(bodyID).gameObject;
        GameObject weapon = PhotonView.Find(weaponID).gameObject;

        GameObject emblemSpring = PhotonView.Find ( springID ).gameObject;
        GameObject emblem = PhotonView.Find ( emblemID ).gameObject;

        body.transform.SetParent(transform.Find("Graphics"));
        body.transform.localPosition = Vector3.zero;
        body.transform.localEulerAngles = Vector3.zero;
        body.name = "Body";

        weapon.transform.SetParent(transform.Find("Graphics"));
        weapon.transform.localPosition = weaponMountPosition;
        weapon.transform.localEulerAngles = weaponMountRotation;
        weapon.name = "Weapon";



        emblemSpring.transform.SetParent ( body.transform.Find ( "Emblem_Mount" ) );
        emblemSpring.transform.localPosition = Vector3.zero;
        emblemSpring.transform.localEulerAngles = Vector3.zero;
        //emblemSpring.GetComponent<HingeJoint> ().connectedBody = GetComponent<Rigidbody> ();

        emblem.transform.SetParent ( emblemSpring.transform.Find ( "Root" ).Find ( "Mount" ) );
        emblem.transform.localPosition = Vector3.zero;
        emblem.transform.localEulerAngles = Vector3.zero;

        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody> ().mass = mass;

        Heatable heatable = GetComponent<Heatable> ();
        heatable.AddRenderers ( body.GetComponent<MeshRenderer> () );
        heatable.AddRenderers ( weapon.GetComponentsInChildren<MeshRenderer> ());
    }

    private void SetCamera()
    {
        if (PhotonNetwork.OfflineMode) return;
        GameObject.FindObjectOfType<Test_SmoothCamera>().SetTarget(this.transform);
    }
}
