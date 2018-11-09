﻿using System;
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

    [SerializeField] private List<RobotWeaponData> weaponDatas = new List<RobotWeaponData> ();
    public List<RobotWeaponData> WeaponDatas { get { return weaponDatas; } }    

    [SerializeField] private MyRobotData myRobotData;
    [SerializeField] private int DEBUG_ROBOT_DATA_INDEX;
    [SerializeField] private int DEBUG_WEAPON_DATA_INDEX;

    public MyRobotData GetMyRobotData { get { return myRobotData; } }

    // Use this for initialization
    void Start () {        
        myRobotData.SetBodyData ( bodyDatas[DEBUG_ROBOT_DATA_INDEX] );
        myRobotData.SetWeaponPrefab ( weaponDatas[DEBUG_WEAPON_DATA_INDEX], myRobotData.BodyData.prefab.transform.Find("Mount").GetComponent<WeaponMount>() );
	}
}

[System.Serializable]
public class MyRobotData
{
    [SerializeField] private RobotBodyData bodyData;
    [SerializeField] private RobotWeaponData weaponData;

    private Vector3 weaponMountPosition;
    private Vector3 weaponMountRotation;

    public RobotBodyData BodyData { get { return bodyData; } }
    public RobotWeaponData WeaponData { get { return weaponData; } }

    public Vector3 WeaponMountPosition { get { return weaponMountPosition; } }    
    public Vector3 WeaponMountRotation { get { return weaponMountRotation; ; } }

    public void SetWeaponPrefab(RobotWeaponData data, WeaponMount mountUsed)
    {
        weaponData = data;
        weaponMountPosition = mountUsed.transform.localPosition;
        weaponMountRotation = mountUsed.transform.localEulerAngles;
    }

    public void SetBodyData (RobotBodyData robotBodyData)
    {
        bodyData = robotBodyData;        
    }
}
