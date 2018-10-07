using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineRobotCreator : MonoBehaviour {

    [SerializeField] private bool active;
    [SerializeField] private GameObject networkGamePlayerPrefab;

    private void Awake()
    {
        if (!active) return;
        PhotonNetwork.OfflineMode = true;
        
    }

    private void Start()
    {
        if (!active) return;
        GameSpawnLocations spawn = GameObject.FindObjectOfType<GameSpawnLocations>();
        GameObject root = PhotonNetwork.Instantiate(networkGamePlayerPrefab.name, spawn.transform.position, spawn.transform.rotation, 0);

        MyRobotData myData = MyRobot.singleton.GetMyRobotData;

        GameObject body = PhotonNetwork.Instantiate(myData.BodyPrefab.name, transform.position, transform.rotation, 0);
        GameObject weapon = PhotonNetwork.Instantiate(myData.WeaponPrefab.name, transform.position, transform.rotation, 0);
        
        body.transform.SetParent(root.transform.Find("Graphics"));
        body.transform.localPosition = Vector3.zero;
        body.transform.localEulerAngles = Vector3.zero;
        body.name = "Body";
        
        weapon.transform.SetParent(root.transform.Find("Graphics"));
        weapon.transform.localPosition = myData.WeaponMountPosition;
        weapon.transform.localEulerAngles = myData.WeaponMountRotation;
        weapon.name = "Weapon";

        root.GetComponent<Rigidbody>().useGravity = true;

        SetCamera(root);
    }

    private void SetCamera(GameObject root)
    {
        GameObject.FindObjectOfType<Test_SmoothCamera>().SetTarget(root.transform);
    }
}
