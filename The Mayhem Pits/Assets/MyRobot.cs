using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyRobot : MonoBehaviour {

    public static MyRobot singleton;

    private void Awake()
    {
        if (singleton == null)
            singleton = this;
        else if (singleton != this)
            Destroy(gameObject);
    }

    [SerializeField] private List<GameObject> robotBodyPrefabs = new List<GameObject>();
    public List<GameObject> RobotBodyPrefabs { get { return robotBodyPrefabs; } }

    [SerializeField] private List<GameObject> robotWeaponPrefabs = new List<GameObject>();
    public List<GameObject> RobotWeaponPrefabs { get { return robotWeaponPrefabs; } }

    [SerializeField] private MyRobotData myRobotData;
    public MyRobotData GetMyRobotData { get { return myRobotData; } }
    //[SerializeField] private RobotBodyData myRobotData;
    //public RobotBodyData GetMyRobotData { get { return myRobotData; } }

    // Use this for initialization
    void Start () {
        //myRobotData = robotBodyDataList[0];
	}
}

[System.Serializable]
public class MyRobotData
{
    [SerializeField] private GameObject bodyPrefab;
    public GameObject BodyPrefab { get { return bodyPrefab; } }

    [SerializeField] private GameObject weaponPrefab;
    public GameObject WeaponPrefab { get { return weaponPrefab; } }

    [SerializeField] private Vector3 weaponMountPosition;
    public Vector3 WeaponMountPosition { get { return weaponMountPosition; } }

    [SerializeField] private Vector3 weaponMountRotation;
    public Vector3 WeaponMountRotation { get { return weaponMountRotation; } }

    public void SetBodyPrefab(GameObject prefab)
    {
        bodyPrefab = prefab;
    }

    public void SetWeaponPrefab(GameObject prefab, WeaponMount mount)
    {
        weaponPrefab = prefab;
        weaponMountPosition = mount.transform.localPosition;
        weaponMountRotation = mount.transform.localEulerAngles;
    }
}
