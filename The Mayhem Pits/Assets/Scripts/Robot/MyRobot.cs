using System;
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

    [SerializeField] private List<RobotBodyData> bodyDatas = new List<RobotBodyData> ();
    public List<RobotBodyData> BodyDatas { get { return bodyDatas; } }

    [SerializeField] private List<RobotWeaponData> weaponDatas = new List<RobotWeaponData>();
    public List<RobotWeaponData> WeaponDatas { get { return weaponDatas; } }

    [SerializeField] private List<RobotEmblemData> emblemDatas = new List<RobotEmblemData>();
    public List<RobotEmblemData> EmblemDatas { get { return emblemDatas; } }

    [SerializeField] private List<RobotSkinData> skinDatas = new List<RobotSkinData>();
    public List<RobotSkinData> SkinDatas { get { return skinDatas; } }

    [SerializeField] private MyRobotData myRobotData;
    [SerializeField] private int DEBUG_ROBOT_DATA_INDEX;
    [SerializeField] private int DEBUG_WEAPON_DATA_INDEX;
    [SerializeField] private int DEBUG_EMBLEM_DATA_INDEX;
    [SerializeField] private int DEBUG_SKIN_DATA_INDEX;

    public MyRobotData GetMyRobotData { get { return myRobotData; } }

    // Use this for initialization
    void Start () {        
        myRobotData.SetBodyData ( bodyDatas[DEBUG_ROBOT_DATA_INDEX] );
        myRobotData.SetWeaponData(weaponDatas[DEBUG_WEAPON_DATA_INDEX], myRobotData.BodyData.prefab.GetComponentInChildren<WeaponMount>());
        myRobotData.SetEmblemData(emblemDatas[DEBUG_EMBLEM_DATA_INDEX], myRobotData.BodyData.prefab.GetComponentInChildren<EmblemMount>());
        myRobotData.SetSkinData(skinDatas[DEBUG_SKIN_DATA_INDEX]);
    }
}

[System.Serializable]
public class MyRobotData
{
    [SerializeField] private RobotBodyData bodyData;
    [SerializeField] private RobotWeaponData weaponData;
    [SerializeField] private RobotEmblemData emblemData;
    [SerializeField] private RobotSkinData skinData;

    private Vector3 weaponMountPosition;
    private Vector3 weaponMountRotation;
    private Vector3 emblemMountPosition;

    public RobotBodyData BodyData { get { return bodyData; } }
    public RobotWeaponData WeaponData { get { return weaponData; } }
    public RobotEmblemData EmblemData { get { return emblemData; } }
    public RobotSkinData SkinData { get { return skinData; } }

    public Vector3 WeaponMountPosition { get { return weaponMountPosition; } }    
    public Vector3 WeaponMountRotation { get { return weaponMountRotation; ; } }

    public Vector3 EmblemMountPosition { get { return emblemMountPosition; } }

    public void SetBodyData(RobotBodyData robotBodyData)
    {
        bodyData = robotBodyData;
    }

    public void SetWeaponData(RobotWeaponData data, WeaponMount mountUsed)
    {
        weaponData = data;
        weaponMountPosition = mountUsed.transform.localPosition;
        weaponMountRotation = mountUsed.transform.localEulerAngles;
    }

    public void SetEmblemData(RobotEmblemData data, EmblemMount mount)
    {
        emblemData = data;
        emblemMountPosition = mount.transform.localPosition;
    }

    public void SetSkinData(RobotSkinData data)
    {
        skinData = data;
    }
}
