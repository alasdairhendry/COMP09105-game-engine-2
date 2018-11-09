using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RobotBodyData {

    public string name;
    public string description;
    public GameObject prefab;
    //public Vector3 weaponMountPosition;
    //public Vector3 weaponMountRotation;

    public float maxSpeed;
    public float acceleration;

    public float rotationalSpeed;

    public float mass;
}
