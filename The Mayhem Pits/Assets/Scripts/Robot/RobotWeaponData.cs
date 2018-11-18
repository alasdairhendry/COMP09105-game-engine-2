using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RobotWeaponData : CustomRobotData {

    public enum ResourceType { Electricity, Fuel }

    //public string weaponName;
    //public GameObject prefab;
    //public Sprite sprite;

    public float baseDamage;

    public ResourceType resourceType;
    public float baseResourceMax;
    public float baseResourceDepletion;
    public float baseResourceRegeneration;
}
