using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RobotBodyData : CustomRobotData {

    //public string name;
    //public string description;
    //public GameObject prefab;
    //public Vector3 weaponMountPosition;
    //public Vector3 weaponMountRotation;

    public float maxSpeed;
    public float acceleration;

    public float rotationalSpeed;

    public float mass;
}

[System.Serializable]
public class CustomRobotData
{
    public string name;

    [SerializeField] private int DebugID;
    public int ID { get; protected set; }

    public string description;
    public GameObject prefab;
    public Sprite sprite;

    public void SetID(int id)
    {
        ID = id;
        DebugID = id;
    }
}
