using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkGameRobot : MonoBehaviourPunCallbacks {

	// Use this for initialization
	void Start () {
        SetDisplayName();

        if (!photonView.IsMine && PhotonNetwork.IsConnected) return;
        
        SpawnGraphics();
        SetCamera();
    }

    private void SetDisplayName()
    {
        if (PhotonNetwork.OfflineMode) return;

        TextMesh[] texts = GetComponentsInChildren<TextMesh>();

        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].text = photonView.Owner.NickName;
        }
    }

    private void SpawnGraphics()
    {
        if (PhotonNetwork.OfflineMode) return;
        MyRobotData myData = MyRobot.singleton.GetMyRobotData;        

        GameObject body = PhotonNetwork.Instantiate(myData.BodyPrefab.name, transform.position, transform.rotation, 0);
        GameObject weapon = PhotonNetwork.Instantiate(myData.WeaponPrefab.name, transform.position, transform.rotation, 0); 

        photonView.RPC("RpcSetupGraphics", RpcTarget.AllBuffered, body.GetPhotonView().ViewID, weapon.GetPhotonView().ViewID, myData.WeaponMountPosition, myData.WeaponMountRotation);
    }

    [PunRPC]
    private void RpcSetupGraphics(int bodyID, int weaponID, Vector3 weaponMountPosition, Vector3 weaponMountRotation)
    {        
        GameObject body = PhotonView.Find(bodyID).gameObject;
        body.transform.SetParent(transform.Find("Graphics"));
        body.transform.localPosition = Vector3.zero;
        body.transform.localEulerAngles = Vector3.zero;
        body.name = "Body";

        GameObject weapon = PhotonView.Find(weaponID).gameObject;
        weapon.transform.SetParent(transform.Find("Graphics"));
        weapon.transform.localPosition = weaponMountPosition;
        weapon.transform.localEulerAngles = weaponMountRotation;
        weapon.name = "Weapon";

        GetComponent<Rigidbody>().useGravity = true;
    }

    private void SetCamera()
    {
        if (PhotonNetwork.OfflineMode) return;
        GameObject.FindObjectOfType<Test_SmoothCamera>().SetTarget(this.transform);
    }
}
